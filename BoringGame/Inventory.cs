using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Text;

namespace BoringGame
{
    public class Inventory
    {
        public bool building = false;
        public StructureType buildingMode;
        public Sprite buildingSprite;

        public StructureType[] contents;

        public Inventory()
        {
            contents = new StructureType[20];
            contents[0] = StructureType.Cart;
            contents[1] = StructureType.Ladder;
            contents[2] = StructureType.Platform;
            contents[3] = StructureType.Drill;
            contents[4] = StructureType.Furnace;
        }

        public GameObject CheckBuilding(Vector2f mousePos, Map map)
        {
           
            if(!building && HotKeyPressed() != -1)
            {
                StartBuilding(HotKeyPressed());
            }

            if (!building && Keyboard.IsKeyPressed(Keyboard.Key.B)) 
            {
                //OpenInventory();      TODO
            }

            if (building && Keyboard.IsKeyPressed(Keyboard.Key.Escape)) //Cancel building mode
            {
                CancelBuilding();
            }

            if (building && buildingSprite != null) //place building indicator on the closest slot of a platform
            {
                return UpdateBuilding(mousePos, map);                                
            }
            return null;
        }

        public GameObject UpdateBuilding(Vector2f mousePos, Map map)
        {
            if (buildingMode == StructureType.Cart)
            {

                if (map.drivingCart != null)
                {
                    float totalWidth = 0;
                    foreach (Cart cart in map.carts)
                    {
                        totalWidth += cart.halfWidth * 2;
                    }
                    totalWidth = totalWidth - map.drivingCart.halfWidth + map.tileSize; //TODO map.tileSize = halfwidth of the placing cart
                    buildingSprite.Position = new Vector2f(map.drivingCart.GetX() - totalWidth, map.tiles[0][map.height - 2].sprite.Position.Y);

                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                        return PlaceCart(map.drivingCart.GetX() - totalWidth, map.tiles[0][map.height - 2].sprite.Position.Y, map);
                }
                else
                {
                    buildingSprite.Position = new Vector2f(mousePos.X, map.tiles[0][map.height - 2].sprite.Position.Y);

                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                        return PlaceCart(mousePos.X, map.tiles[0][map.height - 2].sprite.Position.Y, map);
                }


            }
            else if (buildingMode == StructureType.Platform)
            {
                //loop over all carts to see which one is closest x wise and place it on top of there
                Cart closestCart = null;
                float closestDist = float.MaxValue;
                foreach(Cart cart in map.carts)
                {
                    if(Math.Abs(cart.GetX() - mousePos.X) < closestDist)
                    {
                        closestDist = Math.Abs(cart.GetX() - mousePos.X);
                        closestCart = cart;
                    }
                }
                if(closestCart != null)
                {
                    float platformY = closestCart.GetY() - (closestCart.platforms.Count + 1) * (2f * map.tileSize);
                    buildingSprite.Position = new Vector2f(closestCart.GetX(), platformY);

                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                        return PlacePlatform(closestCart.GetX(), platformY, closestCart, map);
                }


            }
            else
            {
                Platform closestPlatform = null;
                int closestSlot = 0;
                float closestDist = float.MaxValue;
                foreach (Platform platform in map.platforms)
                {
                    for (int i = 0; i < platform.slots; i++)
                    {
                        Vector2f slotPos = platform.GetSlotPosition(i);
                        float dist = (slotPos.X - mousePos.X) * (slotPos.X - mousePos.X) + (slotPos.Y - mousePos.Y) * (slotPos.Y - mousePos.Y);
                        if (dist < closestDist)
                        {
                            closestPlatform = platform;
                            closestDist = dist;
                            closestSlot = i;
                        }
                    }
                }
                if (closestPlatform != null)
                {
                    buildingSprite.Position = closestPlatform.GetSlotPosition(closestSlot);

                    //if mouse is clicked, place the actual structure
                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                    {
                        return PlaceBuilding(closestPlatform, closestSlot);
                    }
                }
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
            buildingMode = contents[hotkey-1];
            buildingSprite = SpriteManager.GetStructureSprite(buildingMode);
            buildingSprite.Color = new Color(0, 255, 0, 128);
            building = true;
        }

        public void CancelBuilding()
        {
            building = false;
            buildingSprite = null;
        }

        public Platform PlacePlatform(float x, float y, Cart cart, Map map)
        {
            building = false;
            Platform newPlatform = new Platform(x, y, map.tileSize);

            newPlatform.SetSprite(buildingSprite);
            buildingSprite.Color = new Color(255, 255, 255, 255);
            buildingSprite = null;
            map.platforms.Add(newPlatform);
            cart.platforms.Add(newPlatform);

            return newPlatform;

        }
        public Cart PlaceCart(float x, float y, Map map)
        {
            building = false;
            Cart newCart = new Cart(x, y, map.tileSize);

            newCart.SetSprite(buildingSprite);
            buildingSprite.Color = new Color(255, 255, 255, 255);
            buildingSprite = null;

            map.carts.Add(newCart);
            map.platforms.Add(newCart);
            if(map.drivingCart == null)
                map.drivingCart = newCart;

            return newCart;
        }

        public GameObject PlaceBuilding(Platform platform, int slot) 
        {
            building = false;

            if (platform.structures[slot] == null)
            {
                Structure newObject;

                if (buildingMode == StructureType.Ladder)
                {
                    newObject = new Ladder(platform.GetSlotPosition(slot).X, platform.GetSlotPosition(slot).Y);
                }
                else if(buildingMode == StructureType.Drill)
                {
                    newObject = new Drill (platform.GetSlotPosition(slot).X, platform.GetSlotPosition(slot).Y);
                }
                else //buildingMode == regular structure
                {
                    newObject = new Structure(platform.GetSlotPosition(slot).X, platform.GetSlotPosition(slot).Y);
                }

                platform.BuildStructure(newObject, slot);
                newObject.SetSprite(buildingSprite);
                buildingSprite.Color = new Color(255, 255, 255, 255);
                buildingSprite = null;
                return newObject;
            }
            else
            {
                //TODO place error sound to indicate the building slot is full
                buildingSprite = null;
                SoundManager.PlayErrorSound();
                return null;
            }

            
        }

        public void DrawBuildingSprite(RenderWindow window)
        {
            if (buildingSprite != null)
            {
                buildingSprite.Draw(window, RenderStates.Default);
            }
        }
    }


    
}
