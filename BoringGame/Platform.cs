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

        public int slots;

        public Structure[] structures;

        public Platform(float x, float y, float half_width) : base(x, y)
        {
            halfWidth = half_width;
            relativePlatformHeight = 25;

            slots = (int)(halfWidth * 2f / structureSize); //slotsize hardcoded to be 50 (same as tilesize)

            structures = new Structure[slots];
        }


        public float GetPlatformY()
        {
            return GetY() - relativePlatformHeight;
        }

        public void MovePlatform(float dx)
        {
            SetX(GetX() + dx);

            //TODO mvoe all the structures ontop of this
            for (int i = 0; i < slots; i++)
            {
                if (structures[i] != null)
                {
                    if (structures[i] is Ladder ladder)
                    {
                        ladder.previousDX = dx;
                    }

                    structures[i].MoveX(dx);
                }
            }

            previousDX = dx;
        }

        public void BuildStructure(Structure structure, int slot)
        {
            structures[slot] = structure;
        }

        public Vector2f GetSlotPosition(int slot)
        {
            //returns the slot coordinates based on the platform position (slot 0 to n from left to right)
            float slotX = ((float)slot - (float)(slots - 1)/2f) * structureSize;

            return new Vector2f(slotX + this.GetX(), this.GetY() - structureSize);
        }
    }
}
