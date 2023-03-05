using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BoringGame
{
    public enum StructureType
    {
        Cart,
        Platform,
        Ladder,
        Drill,
        Furnace,
        Axle,
        Motor,
        Drillhead,
        Cog,
        Structure,

        Count
    }

    public class Structure : GameObject
    {
        public static float structureSize = 50f;

        public float weight;

        public Structure(float x, float y, int id) : base(x, y)
        {
            weight = 1000;
        }

        public bool CollisionCheckRight(float dx, Map map)
        {
            if (!map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X / 2, GetY() + this.GetSprite().Texture.Size.Y / 2).passable || !map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X / 2, GetY() - this.GetSprite().Texture.Size.Y / 2).passable)
                return false;

            return true;
        }

        public virtual float CollisionCheckRightN(float dx, Map map)
        {
            //WARNING CHECK DRILL COLLISION WHEN CHANGING THIS WARNING

            //Tile tile = map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X / 2, GetY());
            //if (tile == null)
            //    return 0;

            //if (!tile.passable)
            //    return Math.Max(0f, tile.sprite.Position.X - GetX() - map.tileSize / 2 - this.GetSprite().Texture.Size.X / 2);

            //return dx;

            float rightX = GetSprite().GetGlobalBounds().Left + GetSprite().GetGlobalBounds().Width;

            Tile tile = map.TileAtCoords(rightX + dx , GetY());
            if (tile == null)
                return 0;

            if (!tile.passable)
                return Math.Max(0f, tile.sprite.Position.X - map.tileSize / 2 - rightX);

            return dx;
        }

        public static new GameObject UpdateBuilding(Vector2f mousePos, Map map, Bore bore, int id)
        {
            Vector2i pos = bore.FindClosestSupportedSlot(mousePos);
            if (pos.X != -1)
            {
                SpriteManager.UpdateBuildingPos(bore.IndextoCoords(pos));

                if (Program.mousePressed)
                {
                    System.Type currentType = Build.InfoType(id);
                    MethodInfo methodInfo = null;
                    while (currentType != null)
                    {
                        Console.WriteLine($"cyle {currentType}");
                        methodInfo = currentType.GetMethod("Place", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
                        if (methodInfo != null)
                        {
                            break;
                        }
                        currentType = currentType.BaseType;
                    }
                    var args = new object[] { pos, bore, id };
                    GameObject newObject = (GameObject)methodInfo.Invoke(null, args); //this runs Structure.Place(x,y,id) or one of its derived classes methods
                                                                                      //GameObject newObject = type.Place(pos, bore, buildingId);
                    //if (buildingMode == StructureType.Motor) map.axles.Add((Axle)newObject);
                    return newObject;
                }
            }
            return null;
        }

        public static GameObject Place(Vector2i indexLoc, Bore br, int id)
        {
            Build.building = false;

            if (indexLoc.X != -1)
            {
                Structure newObject;

                if (Build.buildingMode == StructureType.Motor)
                {
                    newObject = new Motor(br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y, null, 3);
                }
                else //buildingMode == regular structure
                {
                    //newObject = new Structure(br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y);
                    newObject = Build.CreateStructure(indexLoc, br, id);
                }

                br.AddStructure(newObject, indexLoc.X, indexLoc.Y);
                //platform.BuildStructure(newObject, slot);
                SpriteManager.Build(newObject);
                //newObject.SetSprite(Build.buildingSprite);
                //Build.buildingSprite.Color = new Color(255, 255, 255, 255);
                //Build.buildingSprite = null;
                return newObject;
            }
            else
            {
                SpriteManager.ResetBuilding();
                SoundManager.PlayErrorSound();
                return null;
            }


        }
    }
}
