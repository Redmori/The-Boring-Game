using BoringGame;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BoringGame
{
    public enum SlotStatus
    {
        Open,
        Supported,
        Blocked
    }

    public class Bore
    {
        public SlotStatus[,] slotStatus;
        public Structure[,] structureMap;
        public List<Structure> structures;
        public float position;
        float speed;

        public Bore(Cart firstCart)
        {
            position = firstCart.GetX();
            slotStatus = new SlotStatus[0, 30];
            structureMap = new Structure[0, 30];
            structures = new List<Structure>();
            AddCart(firstCart, 2);
            speed = 50f;

        }

        public void AddCart(Cart cart, int size)
        {
            int oldLength = slotStatus.GetLength(0);
            SlotStatus[,] newStatus = new SlotStatus[oldLength + size, slotStatus.GetLength(1)];
            Structure[,] newStructureMap = new Structure[oldLength + size, slotStatus.GetLength(1)];

            for (int i = 0; i < oldLength; i++)
            {
                for (int j = 0; j < slotStatus.GetLength(1); j++)
                {
                    newStatus[i, j] = slotStatus[i, j];
                    newStructureMap[i, j] = structureMap[i, j];
                }
            }

            slotStatus = newStatus;
            structureMap = newStructureMap;

            for (int i = 0; i < size; i++)
                AddStructure(cart, oldLength + i, 0);
            {
                //newStatus[oldLength + i, 0] = SlotStatus.Blocked;
                //newStatus[oldLength + i, 1] = SlotStatus.Supported;
                //newStructureMap[oldLength + i, 0] = cart;

            }
        }

        public Vector2i FindClosestSupportedSlot(Vector2f pos)
        {
            Vector2i closestSlot = new Vector2i(-1, -1);
            float closestDist = float.MaxValue;

            for (int i = 0; i < slotStatus.GetLength(0); i++)
            {
                for (int j = 0; j < slotStatus.GetLength(1); j++)
                {
                    if (slotStatus[i, j] == SlotStatus.Supported)
                    {
                        Vector2f coords = IndextoCoords(new Vector2i(i, j));
                        float dist = (coords.X - pos.X) * (coords.X - pos.X) + (coords.Y - pos.Y) * (coords.Y - pos.Y);
                        if (dist < closestDist)
                        {
                            closestSlot = new Vector2i(i, j);
                            closestDist = dist;
                        }
                    }

                }
            }

            return closestSlot;
        }

        public void AddStructure(Structure structure, int x, int y)
        {
            slotStatus[x, y] = SlotStatus.Blocked;
            if(slotStatus[x, y + 1] != SlotStatus.Blocked)
                slotStatus[x, y + 1] = SlotStatus.Supported;
            structureMap[x, y] = structure;
            if (!structures.Contains(structure))
                structures.Add(structure);
        }

        public void AddPlatform(Cart cart)
        {
            for(int i = 0 ; i < structureMap.GetLength(0); i++)
            {
                int nPlatforms = cart.platforms.Count + 1; //Calculates the hight we need to place, since there are already platforms
                if(structureMap[i, 0] == cart && slotStatus[i,1 + 2* nPlatforms] != SlotStatus.Blocked)
                {
                    slotStatus[i, 1 + 2* nPlatforms] = SlotStatus.Supported;
                }
            }
        }

        public void DetermineSpeed()
        {
            speed = 0.01f;
            //Run every time a structure is added
            //compare structure weights and cart strengths
        }

        public float GetSpeed()
        {
            return speed;
        }

        public void SetSpeed(float spd)
        {
            this.speed = spd; ;
        }


        public void Move(float dx)
        {
            //move all structures
            position += dx;
            foreach (Structure structure in structures)
                structure.MoveX(dx);
        }

        public float CollisionCheckRight(float dx, Map map)
        {
            //loop over front structures to see how far we can move
            float lowestDist = float.MaxValue;
            foreach (Structure structure in structures) {
                float lowdx = structure.CollisionCheckRightN(dx, map);
                if (lowdx < lowestDist)
                    lowestDist = lowdx;
            }
            return lowestDist;
        }

        public Vector2i CoordsToIndex(Vector2f loc)
        {
            //maps world coordinates to bore slot positions using the x position of the bore

            int ipos = (int)Math.Round((position - loc.X) / Structure.structureSize);
            int jpos = (int)(Math.Round(30 - (loc.Y / Structure.structureSize)) + 1); //TEMP 30 is map.height but we dont have access to that here
            return new Vector2i(ipos, jpos);
        }

        public Vector2f IndextoCoords(Vector2i loc)
        {
            float xpos = position - loc.X * Structure.structureSize;
            float ypos = (30 - loc.Y - 2) * Structure.structureSize ;
            return new Vector2f(xpos, ypos);
        }

        public Vector2i GetSize()
        {
            return new Vector2i(slotStatus.GetLength(0),slotStatus.GetLength(1));
        }

        public Structure StructureAtIndex(Vector2i index)
        {
            return structureMap[index.X, index.Y];
        }
    }
}
