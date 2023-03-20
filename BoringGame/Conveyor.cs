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

        public static float itemDistance = 0.24f; //5 items per belt section

        public Conveyor(int id, Itransport right, Itransport left)
        {
            items = new List<Oitem>();
            conveyorSpeed = 0.3f; // (float)Build.info[id][5]; //5 = conveyor speed information?
            connectionIn = right;    
            connectionOut = left;

        }
        public void Update(float dt, Vector2f pos) //what we use the vector2f for?
        {
            MoveItems(dt,pos);

            //attempt to grab from input
            if ( !items.Any() || (items.Any() && items.Last().relativePosition >= itemDistance))
            {
                //Console.WriteLine(connectionIn.ToString());
                Item grabItem = connectionIn?.Grab();
                if (grabItem != null)
                    Receive(grabItem);
            }

        }

        public void MoveItems(float dt, Vector2f pos)
        {
            if (items.Count == 0) return;
            //Moving of items on the belt
            float dx = dt * conveyorSpeed;

            items.First().relativePosition += dx;
            //Console.WriteLine("relative pos: " + items.First().relativePosition);
            if (items.First().relativePosition >= 1)
            {
                if (!Push(items.First()))    //attempt to push the item, if we cant push
                    items.First().relativePosition = 1; //queue it at the ends
            }
            else if (connectionOut != null && items.First().relativePosition >= 1f - itemDistance)
            {
                float dist = connectionOut.HasRoom(items.First().item); //check how much room there is on the next belt
                if (dist < itemDistance && items.First().relativePosition > 1f - dist)
                {
                    items.First().relativePosition = 1f - dist; //queue and the space
                }
            }


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

            newItem.sprite = SpriteManager.GetItemSprite();


        }
        public bool Push(Oitem pushItem)
        {
            if (connectionOut == null || !(connectionOut.HasRoom(pushItem.item) == 1f)) return false;

            connectionOut.Receive(pushItem.item);
            items.Remove(pushItem);
            return true;

        }
        public float HasRoom(Item newItem)
        {
            if (!items.Any()) return 1f;
            if (items.Last().relativePosition >= itemDistance)
                return 1f;
            else
                return itemDistance - items.Last().relativePosition;
        }

        public void Draw(RenderWindow window, Vector2f pos)
        {
            foreach(Oitem item in items)
            {
                item.DrawItem(window, pos);
            }
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
        float HasRoom(Item item); //check if there is room
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

        public void DrawItem(RenderWindow window, Vector2f pos)
        {
            sprite.Position = pos + new Vector2f(Structure.structureSize *0.5f - relativePosition * Structure.structureSize, 0f);
            sprite.Draw(window, RenderStates.Default);
        }
    }
}
