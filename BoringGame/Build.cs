using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace BoringGame
{
    public static class Build
    {
        public static Dictionary<int, object[]> info = new Dictionary<int, object[]>
        {
            //ID                       type            StructureType              name                    size    weight      power     example bonus info if needed
            { 100, new object[] { typeof(Structure),   StructureType.Furnace    ,"Burner Furnace"        ,1      ,100        ,0        ,1 } },
            { 101, new object[] { typeof(Structure),   StructureType.Furnace    ,"Electric Furnace"      ,1      ,120        ,1000     ,1 } },
            { 200, new object[] { typeof(Cart),        StructureType.Cart       ,"Cart"                  ,2      ,0          ,200      ,2, 6f} },
            { 210, new object[] { typeof(Platform),    StructureType.Platform   ,"Platform"              ,2      ,100        ,0        ,20, "test"} },
            { 220, new object[] { typeof(Ladder),      StructureType.Ladder     ,"Ladder"                ,2      ,30         ,0        ,true, 6} }

        };

        public static bool building = false;
        public static StructureType buildingMode;
        public static int buildingId;
        public static Sprite buildingSprite;

        public static Structure CreateStructure(Vector2i indexLoc, Bore br, int id)
        {
            var args = new object[] { br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y, id };
            var structure = Activator.CreateInstance((System.Type)info[id][0], args) as Structure;

            Console.WriteLine($"Created {structure.GetType().Name} with id: {id}");

            return structure;
        }

        //public static StructureType GetStructureType(int id)  //transforms the id to enum StructureType using the info dictionary
        //{
        //    System.Type type = (System.Type)info[id][0];
        //    return (StructureType)Enum.Parse(typeof(StructureType), type.Name);
        //}




        public static GameObject UpdateBuilding(Vector2f mousePos, Map map, Bore bore)
        {
            Console.WriteLine("test1 " + buildingMode.ToString());
            if (buildingMode == StructureType.Cart)
            {

                if (bore != null)
                {
                    //float totalWidth = 0;
                    //foreach (Cart cart in map.carts)
                    //{
                    //    totalWidth += cart.halfWidth * 2;
                    //}
                    //totalWidth = totalWidth - map.drivingCart.halfWidth + map.tileSize; //TODO map.tileSize = halfwidth of the placing cart
                    //buildingSprite.Position = new Vector2f(map.drivingCart.GetX() - totalWidth, map.tiles[0][map.height - 2].sprite.Position.Y);

                    Vector2f backpos = bore.IndextoCoords(new Vector2i(bore.GetSize().X, 0));

                    buildingSprite.Position = backpos;

                    if (Program.mousePressed)
                        return PlaceCart(backpos.X, backpos.Y, map);
                    //    return PlaceCart(map.drivingCart.GetX() - totalWidth, map.tiles[0][map.height - 2].sprite.Position.Y, map);
                }
                else
                {
                    buildingSprite.Position = new Vector2f(mousePos.X, map.tiles[0][map.height - 2].sprite.Position.Y);

                    if (Program.mousePressed)
                        return PlaceCart(mousePos.X, map.tiles[0][map.height - 2].sprite.Position.Y, map);
                }
                return null;

            }
            if (bore == null)
            {
                building = false;
                return null;
            }

            if (buildingMode == StructureType.Platform)
            {
                //loop over all carts to see which one is closest x wise and place it on top of there
                Cart closestCart = null;
                float closestDist = float.MaxValue;
                foreach (Cart cart in map.carts)
                {
                    if (Math.Abs(cart.GetX() - mousePos.X) < closestDist)
                    {
                        closestDist = Math.Abs(cart.GetX() - mousePos.X);
                        closestCart = cart;
                    }
                }
                if (closestCart != null)
                {
                    float platformY = closestCart.GetY() - (closestCart.platforms.Count + 1) * (2f * map.tileSize);
                    buildingSprite.Position = new Vector2f(closestCart.GetX(), platformY);

                    if (Program.mousePressed)
                    {
                        bore.AddPlatform(closestCart);
                        return PlacePlatform(closestCart.GetX(), platformY, closestCart, map);
                    }
                }
            }

            else if (buildingMode == StructureType.Axle || buildingMode == StructureType.Drillhead || buildingMode == StructureType.Cog)
            {
                Axle closestAxle = null;
                float closestD = float.MaxValue;
                OrthSlot closestSide = OrthSlot.Top;
                foreach (Axle nearAxle in map.axles)
                {
                    (OrthSlot slot, float dist) = nearAxle.FindClosestOpenSlot(mousePos);
                    if (dist < closestD)
                    {
                        closestD = dist;
                        closestAxle = nearAxle;
                        closestSide = slot;
                    }
                }
                if (closestAxle != null && closestD != float.MaxValue)
                {
                    buildingSprite.Position = closestAxle.GetNeighbourCoordinates(closestSide);
                    // if (buildingMode == StructureType.Cog && closestAxle is Cog)
                    //     buildingSprite.Position = new Vector2f(closestAxle.GetX(), closestAxle.GetY() + Structure.structureSize);

                    //if mouse is clicked, place the actual structure
                    if (Program.mousePressed)
                    {
                        //TODO generalise this to work for all axle types properly, copy what was done for Cogs
                        Axle newAxle = PlaceAxle(closestAxle, map, closestSide);
                        bore.structures.Add(newAxle);
                        return newAxle;
                    }

                }


            }

            else
            {

                Console.WriteLine("test");
                Vector2i pos = bore.FindClosestSupportedSlot(mousePos);
                if (pos.X != -1)
                {
                    //var line = new VertexArray(PrimitiveType.Lines, 2);
                    //line[0] = new Vertex(bore.IndextoCoords(new Vector2i(0,0)));
                    //line[1] = new Vertex(bore.IndextoCoords(pos));
                    //Program.window.Draw(line);aaaaaaaaa


                    buildingSprite.Position = bore.IndextoCoords(pos);

                    if (Program.mousePressed)
                    {
                        GameObject newObject = PlaceBuilding(pos, bore);
                        if (buildingMode == StructureType.Motor) map.axles.Add((Axle)newObject);
                        return newObject;
                    }
                }
            }           
            return null;
        }

        public static Platform PlacePlatform(float x, float y, Cart cart, Map map)
        {
            building = false;
            Platform newPlatform = new Platform(x, y, map.tileSize);

            newPlatform.SetSprite(buildingSprite);
            buildingSprite.Color = new Color(255, 255, 255, 255);
            buildingSprite = null;
            map.platforms.Add(newPlatform);
            cart.platforms.Add(newPlatform);

            return newPlatform;

        }
        public static Cart PlaceCart(float x, float y, Map map)
        {
            building = false;
            Cart newCart = new Cart(x, y, map.tileSize);

            newCart.SetSprite(buildingSprite);
            buildingSprite.Color = new Color(255, 255, 255, 255);
            buildingSprite = null;

            map.carts.Add(newCart);
            map.platforms.Add(newCart);
            if (map.drivingCart == null)
                map.drivingCart = newCart;

            return newCart;
        }

        public static Axle PlaceAxle(Axle connectingAxle, Map map, OrthSlot side)
        {
            building = false;
            Axle newAxle;
            if (buildingMode == StructureType.Drillhead)
                newAxle = new Drillhead(connectingAxle.GetX() + Structure.structureSize, connectingAxle.GetY(), connectingAxle, 2);
            else if (buildingMode == StructureType.Cog)
            {
                if (connectingAxle is Cog)
                    newAxle = new Cog(connectingAxle.GetX(), connectingAxle.GetY(), connectingAxle, side);   //TEMP TODO open side = TOP & RIGHT hardcoded
                else
                    newAxle = new Cog(connectingAxle.GetX(), connectingAxle.GetY(), connectingAxle, side);   //TEMP TODO open side = TOP & RIGHT hardcoded
            }

            else
                newAxle = new Axle(connectingAxle.GetX() + Structure.structureSize, connectingAxle.GetY(), connectingAxle, 2); //TEMP TODO side = 2 is only right side hard coded for now
            newAxle.torque = connectingAxle.torque;
            newAxle.SetSprite(buildingSprite);
            buildingSprite.Color = new Color(255, 255, 255, 255);
            buildingSprite = null;

            map.axles.Add(newAxle);
            if (!(buildingMode == StructureType.Cog && connectingAxle is Cog))
            {
                Console.WriteLine("Right closed");
                connectingAxle.Right = newAxle;
                connectingAxle.rightOpen = false;
            }

            return newAxle;
        }
        public static GameObject PlaceBuilding(Vector2i indexLoc, Bore br)
        {
            building = false;

            if (indexLoc.X != -1)
            {
                Structure newObject;

                if (buildingMode == StructureType.Ladder)
                {
                    newObject = Build.CreateStructure(indexLoc, br, 220);
                    //newObject = new Ladder(br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y);
                }
                else if (buildingMode == StructureType.Drill) //TODO this is OLD drill, remove
                {
                    newObject = null;
                    //newObject = new Drill (br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y);
                }
                else if (buildingMode == StructureType.Motor)
                {
                    newObject = new Motor(br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y, null, 3);
                }
                else //buildingMode == regular structure
                {
                    //newObject = new Structure(br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y);
                    newObject = Build.CreateStructure(indexLoc, br, 100);
                }

                br.AddStructure(newObject, indexLoc.X, indexLoc.Y);
                //platform.BuildStructure(newObject, slot);
                newObject.SetSprite(buildingSprite);
                buildingSprite.Color = new Color(255, 255, 255, 255);
                buildingSprite = null;
                return newObject;
            }
            else
            {
                //TODO place error sound to indicate the building slot is full
                buildingSprite = null;
                SoundManager.PlayErrorSound();
                return null;
            }


        }

        public static void DrawBuildingSprite(RenderWindow window)
        {
            if (buildingSprite != null)
            {
                buildingSprite.Draw(window, RenderStates.Default);
            }
        }

        public static string InfoName(int id)
        {
            return (string)info[id][2];
        }

        public static StructureType InfoStructureType(int id)
        {
            return (StructureType)info[id][1];
        }
    }
}
