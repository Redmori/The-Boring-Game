using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
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

        public static GameObject Place(Vector2i indexLoc, Bore br)
        {
            Build.building = false;

            if (indexLoc.X != -1)
            {
                Structure newObject;

                if (Build.buildingMode == StructureType.Ladder)
                {
                    newObject = Build.CreateStructure(indexLoc, br, 220);
                    //newObject = new Ladder(br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y);
                }
                else if (Build.buildingMode == StructureType.Drill) //TODO this is OLD drill, remove
                {
                    newObject = null;
                    //newObject = new Drill (br.IndextoCoords(indexLoc).X, br.IndextoCoords(indexLoc).Y);
                }
                else if (Build.buildingMode == StructureType.Motor)
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
                newObject.SetSprite(Build.buildingSprite);
                Build.buildingSprite.Color = new Color(255, 255, 255, 255);
                Build.buildingSprite = null;
                return newObject;
            }
            else
            {
                //TODO place error sound to indicate the building slot is full
                Build.buildingSprite = null;
                SoundManager.PlayErrorSound();
                return null;
            }


        }
    }
}
