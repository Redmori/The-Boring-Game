using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BoringGame
{
    public class Platform : Structure
    {
        public float halfWidth;
        public float relativePlatformHeight;
        public float previousDX;

        //public int slots;

        //public Structure[] structures;

        public Platform(float x, float y, float half_width) : base(x, y, 0) //TEMP id = 0
        {
            halfWidth = half_width;
            relativePlatformHeight = 25;

            //slots = (int)(halfWidth * 2f / structureSize); //slotsize hardcoded to be 50 (same as tilesize)

            //structures = new Structure[slots];
        }


        public float GetPlatformY()
        {
            return GetY() - relativePlatformHeight;
        }

        public override void MoveX(float dX)
        {
            previousDX = dX;
            base.MoveX(dX);
        }
        public void MovePlatform(float dx)
        {

            ////OLD mvoe all the structures ontop of this
            //for (int i = 0; i < slots; i++)
            //{
            //    if (structures[i] != null)
            //    {
            //        if (structures[i] is Ladder ladder)
            //        {
            //            ladder.previousDX = dx;
            //        }

            //        structures[i].MoveX(dx);
            //    }
            //}

            previousDX = dx;

            MoveX(dx);
        }

        //OLD 
        //public void BuildStructure(Structure structure, int slot)
        //{
        //    structures[slot] = structure;
        //}

        //public Vector2f GetSlotPosition(int slot)
        //{
        //    //returns the slot coordinates based on the platform position (slot 0 to n from left to right)
        //    float slotX = ((float)slot - (float)(slots - 1)/2f) * structureSize;

        //    return new Vector2f(slotX + this.GetX(), this.GetY() - structureSize);
        //}

        public static new GameObject UpdateBuilding(Vector2f mousePos, Map map, Bore bore, int id)
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
                SpriteManager.UpdateBuildingPos(new Vector2f(closestCart.GetX(), platformY));

                if (Program.mousePressed)
                {
                    bore.AddPlatform(closestCart);
                    return Platform.Place(closestCart.GetX(), platformY, closestCart, map);
                }
            }

            return null;
        }

        public static new Platform Place(float x, float y, Cart cart, Map map)
        {
            Build.building = false;
            Platform newPlatform = new Platform(x, y, map.tileSize);

            SpriteManager.Build(newPlatform);

            map.platforms.Add(newPlatform);
            cart.platforms.Add(newPlatform);

            return newPlatform;

        }
    }
}
