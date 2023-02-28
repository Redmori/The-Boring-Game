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

        public Bore(Cart firstCart)
        {
            position = firstCart.GetX();
            slotStatus = new SlotStatus[0,0];
            structureMap = new Structure[0,0];
            AddCart(firstCart,2);
            
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
                    newStructureMap[i,j] = structureMap[i,j];
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

        //TODO implement next: returns the indexes of the slots at the position of the mouse or game world coordinates
        public Vector2i GetCoordinates(Vector2f mousePos)
        {
            return new Vector2i(0,0);
        }

    }
}
