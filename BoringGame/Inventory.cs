using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            { 1002, new object[] { "Iron"        ,1      ,100        ,0        ,1 } }

        };


        //public StructureType[] contents;
        //public Item[] items;
        public List<Item> items = new List<Item>(20);

        Font arial = new Font("../../../Content/ArialCEMTBlack.ttf");

        public Popup popup;
        public List<Recipe> recipeActions;
        public List<ScreenButton> recipesButtons;


        public Inventory()
        {
            //contents = new StructureType[20];
            //contents[0] = StructureType.Cart;
            //contents[1] = StructureType.Ladder;
            //contents[2] = StructureType.Platform;
            //contents[3] = StructureType.Drill;
            //contents[4] = StructureType.Furnace;
            //contents[5] = StructureType.Axle;
            //contents[6] = StructureType.Motor;
            //contents[7] = StructureType.Drillhead;
            //contents[8] = StructureType.Cog;

            //items = new Item[20];
            //items[0] = new Item(200, 10);
            //items[1] = new Item(210, 10);
            //items[2] = new Item(220, 5);
            //items[3] = new Item(100, 5);
            //items[4] = new Item(520, 2);
            //items[5] = new Item(500, 20);
            //items[6] = new Item(510, 30);
            //items[7] = new Item(530, 20);
            //items[8] = new Item(1001, 100);

            items.Add(new Item(200, 10));
            items.Add(new Item(210, 10));
            items.Add(new Item(220, 5));
            items.Add(new Item(100, 5));
            items.Add(new Item(520, 2));
            items.Add(new Item(500, 20));
            items.Add(new Item(510, 30));
            items.Add(new Item(530, 20));
            items.Add(new Item(600, 5));
            items.Add(new Item(1001, 100));


            popup = new Popup();
            recipeActions = new List<Recipe>();
            recipesButtons = new List<ScreenButton>();

            ScreenSquare box = new ScreenSquare(new Vector2f(400,500), new Vector2f(0, 0), new Color(120,120,130));
            popup.elements.Add(box);
            // loop over these to get all recipes. (or only obtained recipes?)
            int i = 0;
            foreach (KeyValuePair<int, object[]> kvp in Craft.recipesInfo)
            {
                Vector2f offset = new Vector2f(0, -4 * 50f + i * 50f);
                Recipe newRecipe = Craft.GetRecipe(kvp.Key);
                ScreenButton rect = new ScreenButton(new Vector2f(300, 30), offset, new Color(50,50,80));
                popup.elements.Add(rect);
                ScreenText txt = new ScreenText((string)Craft.recipesInfo[kvp.Key][2], offset);
                popup.elements.Add(txt);
                recipeActions.Add(newRecipe);
                recipesButtons.Add(rect);
                i++;
            }

        }

        public bool CheckClick(Vector2f mousePos)
        {
            if (popup.CheckClick(mousePos) is ScreenButton button && button != null)
            {
                //perform action based on click here
                Recipe action = recipeActions[recipesButtons.IndexOf(button)];
                Console.WriteLine(action.ToString());

                if (Enough(action.input))
                {
                    ConsumeItem(action.input);
                    ReceiveItem(action.output);
                }
                else
                    SoundManager.PlayErrorSound();

                return true;
            }
         
            return false;
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
            if(hotkey > items.Count) return;
            //if (items[hotkey-1] == null) return;
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
        public void ReceiveItem(Item[] items)
        {
            foreach (Item item in items)
                ReceiveItem(item);
        }

        public void ReceiveItem(List<Item> items)
        {
            foreach (Item item in items)
                ReceiveItem(item);
        }
        public void ReceiveItem(Item item)
        {
            if(item == null) return;
            foreach(Item checkItem in items)
            {
                if (checkItem.id == item.id)
                {
                    checkItem.amount += item.amount;
                    return;
                }
            }
            items.Add(new Item( item.id, item.amount));
        }
        public void ConsumeItem(Item[] items)
        {
            foreach (Item item in items)
                ConsumeItem(item);
        }
        public Item ConsumeItem(Item item)
        {
            int returnAmount = 0;
            List<Item> removeItems = new List<Item>();
            foreach (Item checkItem in items)
            {
                if (checkItem.id == item.id)
                {
                    returnAmount += checkItem.amount;
                    checkItem.amount -= item.amount;
                    if (checkItem.amount <= 0) removeItems.Add(checkItem); //removing in loop?
                }
            }
            foreach(Item checkItem in removeItems)
                items.Remove(checkItem);

            if (returnAmount > item.amount) returnAmount = item.amount;

            return new Item(item.id, returnAmount);
        }

        public bool Enough(Item[] checkItems)
        {
            for (int i = 0; i < checkItems.Count(); i++)
            {
                Item checkItem = checkItems[i];
                int amount = checkItem.amount;
                foreach (Item item in items)

                    if (item.id == checkItem.id)
                        amount -= item.amount;
                if (amount > 0) return false;

            }
            return true;
        }

        public void DrawInventory(RenderWindow window, View view) //TODO this can probably be improved to not make a new text every frame
        {
            ScreenSquare box = new ScreenSquare(new Vector2f(380, 400), new Vector2f(-Program.windowWidth / 2 + 50, 200), new Color(100,100,100));
            box.UpdatePosition(window.GetView(), new Vector2f(0,0));
            box.DrawElement(window);
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                    continue;
                Text newtext = new Text($"{i+1} - {items[i].toString()}", arial,19);
                newtext.Color = Color.White;
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
