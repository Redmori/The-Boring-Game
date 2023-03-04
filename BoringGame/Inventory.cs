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



        }

        public GameObject CheckBuilding(Vector2f mousePos, Map map)
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

            if (Build.building && Build.buildingSprite != null) //place building indicator on the closest slot of a platform
            {
                return Build.UpdateBuilding(mousePos, map);                                
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
            Build.buildingMode = Build.InfoStructureType(items[hotkey-1].id);
            Build.buildingId = items[hotkey-1].id;
            //buildingMode = contents[hotkey-1];    //OLD
            Build.buildingSprite = SpriteManager.GetStructureSprite(Build.buildingMode); //TODO change this to buildingID to have a sprite for each building
            Build.buildingSprite.Color = new Color(0, 255, 0, 128);
            Build.building = true;
        }

        public void CancelBuilding()
        {
            Build.building = false;
            Build.buildingSprite = null;
        }




        public void DrawInventory(RenderWindow window, View view) //TODO this can probably be improved to not make a new text every frame
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                    break;
                Text newtext = new Text($"{i+1} - {items[i].ToString()}", arial,19);
                newtext.Color = Color.Red;
                UIText text = new UIText(newtext, new Vector2f(-Program.windowWidth/2, i * 20f)) ;

                text.UpdatePosition(view.Center);

                text.text.Draw(window, RenderStates.Default);
            }
        }
    }

    public class Item
    {
        public int id;
        public int amount;

        public Item(int id, int amount)
        {
            this.id = id;
            this.amount = amount;
        }
        public string ToString()
        {
            return $"{amount}x {Build.InfoName(id)} ";
        }
    }    
}
