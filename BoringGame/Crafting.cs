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
            //ID                  temp        duratio      name               input                                      output
            { 1, new object[] { "temp"        ,1f    ,"Burner Furnace"        ,new Item[] { new Item(1001, 5) }       ,new Item[] { new Item(1002, 1) }, } },

        };

        public static Recipe GetRecipe(int recipeId)
        {
            return new Recipe((Item[])recipesInfo[recipeId][3], (Item[])recipesInfo[recipeId][4], (float)recipesInfo[recipeId][1]);
        }
    }

    public class Crafter : IStructureFunctionality
    {
        public Recipe recipe;
        public Contents input;
        public Contents output;

        public bool isCrafting = false;
        public float craftingProgress = 0;

        public Text tooltip;
        //RectangleShape progressBar;
        //RectangleShape progressFill;

        public Crafter()
        {
            input = new Contents();
            output = new Contents();

            tooltip = TextManager.AddText(TooltipString(), new Vector2f(0,0));


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
                Write();
                isCrafting = true;
                input.Remove(recipe.input);
                UpdateTooltip();

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
        public string TooltipString()
        {
            return $"{input.ToString()}\n{output.ToString()}";
        }
    }

    public interface IStructureFunctionality
    {
        void Update(float dt, Vector2f pos);

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

    public class Contents
    {

        public List<Item> items;

        public Contents()
        {
            items = new List<Item>();
        }

        public void Add(Item addedItem)
        {
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
    }
}
