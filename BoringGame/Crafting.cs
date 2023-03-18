using BoringGame;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BoringGame
{
    public static class Craft
    {
        public static int recipesInfoDuration = 1; public static int recipesInfoName = 2; public static int recipesInfoInput = 3; public static int recipesInfoOutput = 4;
        public static Dictionary<int, object[]> recipesInfo = new Dictionary<int, object[]>
        {
            //ID                     temp        duration   name               input                                      output
            { 1, new object[] {     "temp"        ,1f    ,"Iron"            ,new Item[] { new Item(1001, 5) }         ,new Item[] { new Item(1002, 1) }, } },
            { 100, new object[] {   "temp"        ,1f    ,"Burner Furnace"  ,new Item[] { new Item(1002, 5) }         ,new Item[] { new Item(100, 1) }, } },
            { 200, new object[] {   "temp"        ,1f    , "Cart"           ,new Item[] { new Item(1002, 30) }        ,new Item[] { new Item(200, 1) }, } },
            { 210, new object[] {   "temp"        ,1f    , "Platform"       ,new Item[] { new Item(1002, 10) }        ,new Item[] { new Item(210, 1) }, } },
            { 220, new object[] {   "temp"        ,1f    , "Ladder"         ,new Item[] { new Item(1002, 5) }         ,new Item[] { new Item(220, 1) }, } },
            { 500, new object[] {   "temp"        ,1f    , "Axle"           ,new Item[] { new Item(1002, 2) }         ,new Item[] { new Item(500, 5) }, } },
            { 510, new object[] {   "temp"        ,1f    , "Cogwheel"       ,new Item[] { new Item(1002, 3) }         ,new Item[] { new Item(510, 2) }, } },
            { 520, new object[] {   "temp"        ,1f    , "Motor"          ,new Item[] { new Item(1002, 50) }        ,new Item[] { new Item(520, 1) }, } },
            { 530, new object[] {   "temp"        ,1f    , "Drillhead"      ,new Item[] { new Item(1002, 40) }        ,new Item[] { new Item(530, 1) }, } },

        };

        public static Recipe GetRecipe(int recipeId)
        {
            return new Recipe((Item[])recipesInfo[recipeId][3], (Item[])recipesInfo[recipeId][4], (float)recipesInfo[recipeId][1]);
        }
    }

    public class Crafter : IStructureFunctionality, Itransport
    {
        public Recipe recipe;
        public Contents input;
        public Contents output;

        public bool isCrafting = false;
        public float craftingProgress = 0;

        public Text tooltip;
        //RectangleShape progressBar;
        //RectangleShape progressFill;
        VertexArray progressBar;

        public Crafter()
        {
            input = new Contents();
            output = new Contents();

            tooltip = TextManager.AddText(TooltipString(), new Vector2f(0,0));

            progressBar = TextManager.GetRectangle(new Vector2f(100, 20), new Vector2f(0, 0), Color.Cyan);

            ////TEMP progress bar:      
            //progressBar = new RectangleShape(new Vector2f(200, 20));
            //progressBar.FillColor = Color.Black;
            //progressBar.Position = new Vector2f(50, 50);

            //progressFill = new RectangleShape(new Vector2f(10, 20));
            //progressFill.FillColor = Color.Green;
            //progressFill.Position = progressBar.Position;

            //TextManager.AddProgressBar(progressBar);
            //TextManager.AddProgressBar(progressFill);
        }

        public void Update(float dt, Vector2f pos)
        {
            if (recipe == null) return;
            StartCraft(); //Maybe unwise to do this every loop, only when input/output/recipe change?
            if (isCrafting) Craft(dt);
            if (tooltip != null)
                tooltip.Position = pos;

            if (isCrafting)
            {
                float progress = craftingProgress / recipe.duration;
                progressBar = TextManager.UpdateRectangle(progressBar, new Vector2f(Structure.structureSize * progress, 3), pos - new Vector2f(0,-7f), Color.Cyan);
            }
            else
            {
                TextManager.RemoveRectangle(progressBar);
            }
            ////TEMP progress bar
            //if (isCrafting)
            //{
            //    float progress = craftingProgress / recipe.duration;
            //    progressBar.Position = pos;
            //    progressFill = new RectangleShape(new Vector2f(progressBar.Size.X * progress, progressBar.Size.Y));
            //    progressFill.Position = pos;
            //}
        }

        public void Craft(float dt)
        {
            craftingProgress += dt;
            float dp = craftingProgress - recipe.duration;
            if (dp >= 0)
            {
                craftingProgress = dp;
                FinishCraft();
            }
        }
        public void AddInput(Item[] items)
        {
            input.Add(items);
            UpdateTooltip();
        }
        public void AddInput(Item item)
        {
            input.Add(item);
            UpdateTooltip();
        }
        public List<Item> Loot()
        {
            List<Item> loot = output.items;
            output.items = new List<Item>();
            //TODO update output string
            return loot;
        }
        public void SetRecipe(Recipe rec)
        {
            recipe = rec;
        }

        public void StartCraft()
        {
            if (!isCrafting && input.Enough(recipe.input))
            {
                //Write();
                isCrafting = true;
                input.Remove(recipe.input);
                UpdateTooltip();
                TextManager.AddRectangle(progressBar);

            }
        }

        public void FinishCraft()
        {
            output.Add(recipe.output);
            UpdateTooltip();
            isCrafting = false;
        }

        public void Write()
        {
            Console.WriteLine("================================");
            Console.WriteLine(recipe.ToString());
            Console.WriteLine($"Input: {input.ToString()}");
            Console.WriteLine($"Output: {output.ToString()}");
            Console.WriteLine("================================");
        }

        public void UpdateTooltip()
        {
            tooltip.DisplayedString = TooltipString();
        }

        public void Destroy()
        {
            TextManager.RemoveText(tooltip);
            TextManager.RemoveRectangle(progressBar);
        }

        public string TooltipString()
        {
            return $"{input.ToString()}\n{output.ToString()}";
        }

        public Item Grab()
        {
            Item grabItem = output.Grab();
            UpdateTooltip();
            return grabItem;//throw new NotImplementedException();
        }

        public void Receive(Item item)
        {
            AddInput(item);
            //throw new NotImplementedException();
        }

        public bool HasRoom(Item item)
        {
            return true; //TODO: implement
                //throw new NotImplementedException();
        }

        public void Draw(RenderWindow window, Vector2f pos)
        {
            
        }
    }

    public interface IStructureFunctionality
    {
        void Destroy();
        void Update(float dt, Vector2f pos);
        void Draw(RenderWindow window, Vector2f pos);

    }


    public class Recipe
    {
        public Item[] input;
        public Item[] output;

        public float duration;

        public Recipe(Item[] inp, Item[] outp, float dur)
        {
            input = inp;
            output = outp;
            duration = dur;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Recipe: ");

            foreach (Item item in input)
                sb.Append(item.toString());
            sb.Append(" -> ");
            foreach (Item item in output)
                sb.Append(item.toString());
            return sb.ToString();
        }
    }

    public class Contents : Itransport
    {

        public List<Item> items;

        public Contents()
        {
            items = new List<Item>();
        }

        public void Add(Item addedItem)
        {
            if (addedItem == null) return;
            int remaining = addedItem.amount;
            foreach (Item currentItem in items)
            {
                if (currentItem.id == addedItem.id)
                {
                    currentItem.amount += remaining;
                    int stackSize = 2000;    //stack size hardcoded, use currentItem.stacksize 
                    if (currentItem.amount > stackSize)
                    {
                        remaining = currentItem.amount - stackSize;
                        currentItem.amount = stackSize;
                    }
                    else remaining = 0;
                }
            }
            if(remaining > 0)
                items.Add(new Item(addedItem.id, remaining));
        }

        public void Add(Item[] addedItems)
        {
            for (int i = 0; i < addedItems.Count(); i++)
                Add(addedItems[i]);
        }


        public void Remove(Item removedItem)
        {
            int remaining = removedItem.amount;
            List<Item> removeList = new List<Item>();
            foreach (Item currentItem in items)
            {
                if (currentItem.id == removedItem.id)
                {
                    currentItem.amount -= remaining;
                    if (currentItem.amount <= 0)
                    {
                        remaining = currentItem.amount * -1;
                        removeList.Add(currentItem);
                    }
                    else break;
                }
            }
            foreach (Item removeItem in removeList)
                items.Remove(removeItem);

        }

        public void Remove(Item[] removedItems)
        {
            for (int i = 0; i < removedItems.Count(); i++)
                Remove(removedItems[i]);
        }

        public void Remove(int index)
        {
            items.RemoveAt(index);
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Item item in items)
            {
                sb.Append($"{item.toString()} ");
            }
            return sb.ToString();
        }

        public Item Grab()
        {
            //throw new NotImplementedException();
            if(items.Count == 0) return null;

            Item firstItem = items.FirstOrDefault();
            Item grabItem = new Item(firstItem.id, 1);
            Remove(grabItem);
            return grabItem;
        }

        public void Receive(Item item)
        {
            Add(item);
            //throw new NotImplementedException();
        }

        public bool HasRoom(Item item)
        {
            return true; //TODO implement
            //throw new NotImplementedException();
        }
    }
}
