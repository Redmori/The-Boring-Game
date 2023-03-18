﻿using SFML.Graphics;
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
        Conveyor,

        Count
    }

    public class Structure : GameObject
    {
        public static float structureSize = 50f;

        public int id;
        public float weight;

        public Crafter crafter; //TEMP crafter, not needed anymore?
        public IStructureFunctionality functionality; //The functionality of this structure, i.e. crafter
        public Itransport transport;

        public Structure(float x, float y, int id) : base(x, y)
        {
            this.id = id;
            weight = 1000;

            //TEMP making a crafter
            if(id == 100) //"furnace"
            {
                crafter = new Crafter();
                crafter.SetRecipe(Craft.GetRecipe(1));
                crafter.AddInput(new Item(1001, 50));
                functionality = crafter;
                transport = crafter;
            }
            if(id == 600) //conveyor
            {
                Vector2i index = Program.bore.CoordsToIndex(new Vector2f(x, y));// + new Vector2i(0,-3);
                Itransport left = Program.bore.StructureAtIndex(index + new Vector2i(1,0))?.transport;
                Itransport right = Program.bore.StructureAtIndex(index - new Vector2i(1,0))?.transport;
                functionality = new Conveyor(600, right, left);
                transport = (Itransport)functionality;

                if (left is Conveyor)
                    ((Conveyor)left).connectionIn = transport;
                if (right is Conveyor)
                    ((Conveyor)right).connectionOut = transport;
            }
            if(transport != null) //whenever a structure is made, connect it to nearby transport structures. TEMP?
            {
                Itransport right = Program.bore.StructureAtIndex(Program.bore.CoordsToIndex(new Vector2f(x, y)) - new Vector2i(1, 0))?.transport;
                if (right is Conveyor)
                    ((Conveyor)right).connectionOut = transport;
            }
        }

        public virtual void Update(float dt)
        {
            functionality?.Update(dt, new Vector2f(GetX(), GetY()));    //Update the functionality, i.e. crafter
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

        public override void Draw(RenderWindow window)
        {
            functionality?.Draw(window, new Vector2f(this.GetX(), this.GetY()));
            base.Draw(window);
        }
        public virtual void Destroy()
        {
            functionality?.Destroy();
        }

        public static new GameObject UpdateBuilding(Vector2f mousePos, Map map, Bore bore, int id)
        {
            if (bore == null)
                return null;
            Vector2i pos = bore.FindClosestSupportedSlot(mousePos);
            if (pos.X != -1)
            {
                SpriteManager.UpdateBuildingPos(bore.IndextoCoords(pos));

                if (Program.mousePressed)
                {
                    var args = new object[] { pos, bore, id };
                    GameObject newObject = (GameObject)Build.GetMethodInfo(id, "Place").Invoke(null, args); //this runs Structure.Place(x,y,id) or one of its derived classes methods
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
