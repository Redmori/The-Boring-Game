using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Reflection;
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
                        return Cart.Place(backpos.X, backpos.Y, map);
                    //    return PlaceCart(map.drivingCart.GetX() - totalWidth, map.tiles[0][map.height - 2].sprite.Position.Y, map);
                }
                else
                {
                    buildingSprite.Position = new Vector2f(mousePos.X, map.tiles[0][map.height - 2].sprite.Position.Y);

                    if (Program.mousePressed)
                        return Cart.Place(mousePos.X, map.tiles[0][map.height - 2].sprite.Position.Y, map);
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
                        return Platform.Place(closestCart.GetX(), platformY, closestCart, map);
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
                        Axle newAxle = Axle.Place(closestAxle, map, closestSide);
                        bore.structures.Add(newAxle);
                        return newAxle;
                    }

                }
            }

            else
            {
                Vector2i pos = bore.FindClosestSupportedSlot(mousePos);
                if (pos.X != -1)
                {
                    buildingSprite.Position = bore.IndextoCoords(pos);

                    if (Program.mousePressed)
                    {
                        MethodInfo methodInfo = InfoType(buildingId).GetMethod("Place", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                        var args = new object[] { pos, bore, buildingId };
                        GameObject newObject = (GameObject)methodInfo.Invoke(null, args);
                        //GameObject newObject = type.Place(pos, bore, buildingId);
                        if (buildingMode == StructureType.Motor) map.axles.Add((Axle)newObject);
                        return newObject;
                    }
                }
            }           
            return null;
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

        public static System.Type InfoType(int id)
        {
            return (System.Type)info[id][0];
        }
    }
}
