using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoringGame
{
    public class Conveyor : Itransport, IStructureFunctionality
    {
        public float conveyorSpeed;
        List<Oitem> items;

        public bool direction = true; //true = left to right, false is opposite
        public Itransport connectionIn;
        public Itransport connectionOut;

        public static float itemDistance = 0.25f; //5 items per belt section

        public Conveyor(int id, Itransport right, Itransport left)
        {
            items = new List<Oitem>();
            conveyorSpeed = 0.3f; // (float)Build.info[id][5]; //5 = conveyor speed information?
            connectionIn = left;    
            connectionOut = right;

        }
        public void Update(float dt, Vector2f something) //what we use the vector2f for?
        {
            MoveItems(dt);

            //attempt to grab from input
            if ( !items.Any() || (items.Any() && items.Last().relativePosition >= itemDistance))
            {
                //Console.WriteLine(connectionIn.ToString());
                Item grabItem = connectionIn.Grab();
                if (grabItem != null)
                    Receive(grabItem);
            }

        }

        public void MoveItems(float dt)
        {
            if (items.Count == 0) return;
            //Moving of items on the belt
            float dx = dt * conveyorSpeed;

            items.First().relativePosition += dx;
            if (items.First().relativePosition >= 1)
                if (!Push(items.First()))    //attempt to push the item, if we cant push
                    items.First().relativePosition = 1; //queue it at the ends

            if (!items.Any()) return;
            Oitem prevItem = items.First();
            foreach (Oitem item in items)
            {
                //Console.WriteLine("item position" + item.relativePosition);
                if (item == items.First()) continue; //skip 1st one since we already done that

                item.relativePosition += dx; //move the item

                if (prevItem.relativePosition - item.relativePosition < itemDistance) //if we get too close to the one in front
                    item.relativePosition = prevItem.relativePosition - itemDistance; //set the item position to the correct spot relative to the item in front

                prevItem = item;

            }
        }
        public void Destroy()
        {
            //TODO: implement what to do when conveyor is destroyed
            // put all items in player inventory
        }
        public Item Grab() //
        {
            //never grab from a conveyor
            return null;

        }
        public void Receive(Item recItem)
        {
            //when receiving a pushed item
            Oitem newItem = new Oitem(recItem); //grab the item
            if (items.Any())
            {
                float lastPos = items.Last().relativePosition;
                if (lastPos <= itemDistance * 1.2f)   //snap to the last item
                    newItem.relativePosition = lastPos - itemDistance;
            }
            else
                newItem.relativePosition = 0;
            items.Add(newItem); //add the item to the list

        }
        public bool Push(Oitem pushItem)
        {
            if (!connectionOut.HasRoom(pushItem.item)) return false;

            connectionOut.Receive(pushItem.item);
            items.Remove(pushItem);
            return true;

        }
        public bool HasRoom(Item newItem)
        {
            return (items.Last().relativePosition >= itemDistance);
        }
    }

    //public class Shute : Itransport
    //{
    //}

    //public class Lift : Itransport
    //{
    //}


    public interface Itransport
    {
        Item Grab();
        //void Push(Oitem itemObject); //do i want this for all transports, or just conveyor belt?
        void Receive(Item item); //method that is ran when you receive a push item
        bool HasRoom(Item item); //check if there is room
    }


    public class Oitem
    {
        public Item item;
        public float relativePosition;
        public GameObject sprite;

        public Oitem(Item newItem)
        {
            item = newItem;
            //sprite = SpriteManager.GetSprite(newItem.id);
            relativePosition = 0f;
        }
    }
}
