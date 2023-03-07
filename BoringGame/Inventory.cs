using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Security;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Text = SFML.Graphics.Text;

namespace BoringGame
{
    public class Inventory
    {
        public static Dictionary<int, object[]> mats = new Dictionary<int, object[]>
        {
            //ID                   name                  stacksize    weight      power     example bonus info if needed
            { 1001, new object[] { "Dirt"        ,1      ,100        ,0        ,1 } },
            { 1002, new object[] { "Resource"        ,1      ,100        ,0        ,1 } }

        };


        public StructureType[] contents;
        public Item[] items;

        Font arial = new Font("../../../Content/ArialCEMTBlack.ttf");

        public Inventory()
        {
            contents = new StructureType[20];
            contents[0] = StructureType.Cart;
            contents[1] = StructureType.Ladder;
            contents[2] = StructureType.Platform;
            contents[3] = StructureType.Drill;
            contents[4] = StructureType.Furnace;
            contents[5] = StructureType.Axle;
            contents[6] = StructureType.Motor;
            contents[7] = StructureType.Drillhead;
            contents[8] = StructureType.Cog;

            items = new Item[20];
            items[0] = new Item(200, 10);
            items[1] = new Item(210, 10);
            items[2] = new Item(220, 5);
            items[3] = new Item(100, 5);
            items[4] = new Item(520, 2);
            items[5] = new Item(500, 20);
            items[6] = new Item(510, 30);
            items[7] = new Item(530, 20);
            items[8] = new Item(1001, 100);



        }

        public GameObject CheckBuilding(Vector2f mousePos, Map map, Bore bore)
        {
           
            if(!Build.building && HotKeyPressed() != -1)
            {
                StartBuilding(HotKeyPressed());
            }

            if (!Build.building && Keyboard.IsKeyPressed(Keyboard.Key.B)) 
            {
                //OpenInventory();      TODO
            }

            if (Build.building && Keyboard.IsKeyPressed(Keyboard.Key.Escape)) //Cancel building mode
            {
                CancelBuilding();
            }

            if (Build.building && SpriteManager.BuildActive()) //place building indicator on the closest slot of a platform
            {
                return Build.UpdateBuilding(mousePos, map, bore);                                
            }
            return null;
        }



        public int HotKeyPressed()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num1))
                return 1;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num2))
                return 2;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num3))
                return 3;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num4))
                return 4;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num5))
                return 5;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num6))
                return 6;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num7))
                return 7;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num8))
                return 8;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num9))
                return 9;


            return -1;
        }

        public void StartBuilding(int hotkey)
        {
            if (items[hotkey-1] == null) return;
            Build.buildingId = items[hotkey - 1].id;
            if (!Build.InfoContains(Build.buildingId)) return;
            Build.buildingMode = Build.InfoStructureType(items[hotkey-1].id);
            SpriteManager.StartBuilding(Build.buildingMode);
            Build.building = true;
        }

        public void CancelBuilding()
        {
            Build.building = false;
            SpriteManager.ResetBuilding();
        }

        public void ReceiveItem(Item item)
        {
            if(item == null) return;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) continue;
                if (items[i].id == item.id)
                {
                    items[i].amount += item.amount;
                    return;
                }
            }
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = item;
                    break;
                }
            }
        }
        public Item ConsumeItem(Item item)
        {
            int returnAmount = 0;
            for(int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) continue;
                if (items[i].id == item.id)
                {
                    returnAmount += items[i].amount;
                    items[i].amount -= item.amount;
                    if (items[i].amount <= 0) items[i] = null;
                }
            }
            if (returnAmount > item.amount) returnAmount = item.amount;

            return new Item(item.id, returnAmount);
        }

        public void DrawInventory(RenderWindow window, View view) //TODO this can probably be improved to not make a new text every frame
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                    continue;
                Text newtext = new Text($"{i+1} - {items[i].toString()}", arial,19);
                newtext.Color = new Color(0, 102, 0);
                UIText text = new UIText(newtext, new Vector2f(-Program.windowWidth/2, i * 20f)) ;

                text.UpdatePosition(view.Center);

                text.text.Draw(window, RenderStates.Default);
            }
        }
        public static string MatsName(int id)
        {
            if(mats.ContainsKey(id))
                return (string)mats[id][0];
            return null;
        }
    }

    public class Item
    {
        public int amount;
        public int id;

        public Item(int id, int amount)
        {
            this.amount = amount;
            this.id = id;
        }

        public string toString()
        {
            return $"{amount}x {Inventory.MatsName(id)}{Build.InfoName(id)}";
        }
    }
    
}
