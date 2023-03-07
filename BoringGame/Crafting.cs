using BoringGame;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoringGame
{
    public class Crafter
    {
        public Recipe recipe;
        public Contents input;
        public Contents output;

        public bool isCrafting = false;
        public float craftingProgress = 0;

        public Text tooltip;

        public Crafter()
        {
            input = new Contents();
            output = new Contents();

            tooltip = TextManager.AddText(TooltipString(), new Vector2f(0,0));
        }

        public void Update(float dt)
        {
            if (recipe == null) return;
            StartCraft(); //Maybe unwise to do this every loop, only when input/output/recipe change?
            if (isCrafting) Craft(dt);
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
