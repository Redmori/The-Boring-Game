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
        public float position;
        float speed;

        public Bore(Cart firstCart)
        {
            position = firstCart.GetX();
            slotStatus = new SlotStatus[0, 0];
            structureMap = new Structure[0, 0];
            AddCart(firstCart, 2);
            speed = 0.1f;

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

            for (int i = 1; i < size; i++)
                AddStructure(cart, oldLength + i, 0);
            {
                //newStatus[oldLength + i, 0] = SlotStatus.Blocked;
                //newStatus[oldLength + i, 1] = SlotStatus.Supported;
                //newStructureMap[oldLength + i, 0] = cart;

            }
        }

        public void AddStructure(Structure structure, int x, int y)
        {
            slotStatus[x, y] = SlotStatus.Blocked;
            slotStatus[x, y + 1] = SlotStatus.Supported;
            structureMap[x, y] = structure;
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


        public void Move(float dx)
        {
            //move all structures
            position += dx;
        }

        public float CollisionCheckRight(float dx, Map map)
        {
            //loop over front structures to see how far we can move
            float lowestDist = float.MaxValue;
            for (int j = 0; j < structureMap.GetLength(0); j++)
            {
                for (int i = 0; i < structureMap.GetLength(1); i++)
                {
                    if (structureMap[i, j] != null)
                    {
                        float lowdx = structureMap[i, j].CollisionCheckRightN(dx, map);
                        if (lowdx < lowestDist)
                            lowestDist = lowdx;
                        break;
                    }
                }
            }
            return lowestDist;
        }

        public Vector2i CoordsToIndex(Vector2f loc)
        {
            //maps world coordinates to bore slot positions using the x position of the bore

            int ipos = (int)Math.Round((position - loc.X) / Structure.structureSize);
            int jpos = (int)(Math.Round((30 - loc.Y) / Structure.structureSize) + 1); //TEMP 30 is map.height but we dont have access to that here
            return new Vector2i(ipos, jpos);
        }

        public Vector2f IndextoCoords(Vector2i index)
        {
            float x = position - index.X*Structure.structureSize;
            float y = 30 - index.Y*Structure.structureSize - 1;
            return new Vector2f(x, y);
        }


        public Structure StructureAtIndex(Vector2i index)
        {
            return structureMap[index.X, index.Y];
        }
    }
}
