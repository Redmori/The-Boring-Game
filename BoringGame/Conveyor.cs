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

        public Conveyor(int id)
        {
            conveyorSpeed = (float)Build.info[id][5]; //5 = conveyor speed information?
        //TODO set the in and output
    }
        public void Update(float dt, Vector2f something) //what we use the vector2f for?
        {
            MoveItems(dt);

            //attempt to grab from input
            //TODO check if there is room on the belt
            float lastPos = items.Last().relativePosition;
    
        if (lastPos >= itemDistance)
            {
                Item grabItem = connectionIn.Grab();
                if (grabItem != null)
                    Receive(grabItem);
            }

        }

        public void MoveItems(float dt)
        {
            //Moving of items on the belt
            float dx = dt * conveyorSpeed;
            foreach (Oitem item in items)
            {
                item.relativePosition += dx;
            }

            if (items.First().relativePosition >= 1)
                Push(items.First());
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
            float lastPos = items.Last().relativePosition;
            Oitem newItem = new Oitem(recItem); //grab the item
            if (lastPos <= itemDistance * 1.2f)   //snap to the last item
                newItem.relativePosition = lastPos - itemDistance;
            items.Add(newItem); //add the item to the list

        }
        public void Push(Oitem pushItem)
        {
            if (!connectionOut.HasRoom(pushItem.item)) return;

            connectionOut.Receive(pushItem.item);
            items.Remove(pushItem);
    
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
        void Push(Oitem itemObject); //do i want this for all transports, or just conveyor belt?
        void Receive(Item item); //method that is ran when you receive a push item
        bool HasRoom(Item item); //check if there is room
    }


    public class Oitem
    {
        public Item item;
        public float relativePosition;
        public Sprite sprite;

        public Oitem(Item newItem)
        {
            item = newItem;
            //sprite = SpriteManager.GetSprite(newItem.id);
            relativePosition = 0f;
        }
    }
}
