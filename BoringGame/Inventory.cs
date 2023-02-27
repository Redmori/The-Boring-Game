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
            contents[5] = StructureType.Axle;
            contents[6] = StructureType.Motor;
            contents[7] = StructureType.Drillhead;
            contents[8] = StructureType.Cog;
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
            //OLD
            //else if (buildingMode == StructureType.Axle || buildingMode == StructureType.Drillhead || buildingMode == StructureType.Cog)
            //{
            //    Axle closestAxle = null;
            //    float closestDist = float.MaxValue;
            //    foreach(Axle nearAxle in map.axles)
            //    {
            //        float dist = (nearAxle.GetX() - mousePos.X + Structure.structureSize) * (nearAxle.GetX() - mousePos.X + Structure.structureSize) + (nearAxle.GetY() - mousePos.Y) * (nearAxle.GetY() - mousePos.Y);
            //        if(dist < closestDist && nearAxle.rightOpen) //temp only check right side, TODO: check all axle sides and find locally closest from that
            //        {
            //            closestAxle = nearAxle;
            //            closestDist = dist;
            //        }
            //        //TEMP look for the top of a cog TODO fix bottom aswel

            //        if(buildingMode == StructureType.Cog && nearAxle is Cog)
            //        {
            //            float distCog = (nearAxle.GetX() - mousePos.X) * (nearAxle.GetX() - mousePos.X) + (nearAxle.GetY() + Structure.structureSize - mousePos.Y) * (nearAxle.GetY() - Structure.structureSize - mousePos.Y);
            //            if(nearAxle.bottomOpen && distCog < closestDist)
            //            {
            //                closestAxle = nearAxle;
            //                closestDist = distCog;
            //            }
            //        }

            //    }
            //    if(closestAxle != null)
            //    {
            //        buildingSprite.Position = new Vector2f(closestAxle.GetX() + Structure.structureSize, closestAxle.GetY());
            //        if(buildingMode == StructureType.Cog && closestAxle is Cog)
            //            buildingSprite.Position = new Vector2f(closestAxle.GetX(), closestAxle.GetY() + Structure.structureSize);

            //        //if mouse is clicked, place the actual structure
            //        if (Mouse.IsButtonPressed (Mouse.Button.Left))
            //        {
            //            return PlaceAxle(closestAxle, map);
            //        }

            //    }
            //}

            else if (buildingMode == StructureType.Axle || buildingMode == StructureType.Drillhead || buildingMode == StructureType.Cog)
            {
                Axle closestAxle = null;
                float closestDist = float.MaxValue;
                OrthSlot closestSide = OrthSlot.Top;
                foreach (Axle nearAxle in map.axles)
                {
                    (OrthSlot slot, float dist) = nearAxle.FindClosestOpenSlot(mousePos);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestAxle = nearAxle;
                        closestSide = slot;
                    }
                }
                if (closestAxle != null && closestDist != float.MaxValue)
                {
                    buildingSprite.Position = closestAxle.GetNeighbourCoordinates(closestSide);
                   // if (buildingMode == StructureType.Cog && closestAxle is Cog)
                   //     buildingSprite.Position = new Vector2f(closestAxle.GetX(), closestAxle.GetY() + Structure.structureSize);

                    //if mouse is clicked, place the actual structure
                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                    {
                        //TODO generalise this to work for all axle types properly, copy what was done for Cogs
                        return PlaceAxle(closestAxle, map, closestSide);
                    }

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
                        GameObject newObject = PlaceBuilding(closestPlatform, closestSlot);
                        if (buildingMode == StructureType.Motor) map.axles.Add((Axle)newObject);
                        return newObject;
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

        public Axle PlaceAxle(Axle connectingAxle, Map map, OrthSlot side)
        {
            building = false;
            Axle newAxle;
            if (buildingMode == StructureType.Drillhead)
                newAxle = new Drillhead(connectingAxle.GetX() + Structure.structureSize, connectingAxle.GetY(), connectingAxle, 2);
            else if (buildingMode == StructureType.Cog)
            {
                if(connectingAxle is Cog)
                    newAxle = new Cog(connectingAxle.GetX(), connectingAxle.GetY(), connectingAxle, side);   //TEMP TODO open side = TOP & RIGHT hardcoded
                else
                    newAxle = new Cog(connectingAxle.GetX(), connectingAxle.GetY(), connectingAxle, side);   //TEMP TODO open side = TOP & RIGHT hardcoded
            }
                
            else
                newAxle = new Axle(connectingAxle.GetX() + Structure.structureSize, connectingAxle.GetY(), connectingAxle, 2); //TEMP TODO side = 2 is only right side hard coded for now
            newAxle.torque = connectingAxle.torque;
            newAxle.SetSprite(buildingSprite);
            buildingSprite.Color = new Color(255, 255, 255, 255);
            buildingSprite = null;

            map.axles.Add(newAxle);
            if (!(buildingMode == StructureType.Cog && connectingAxle is Cog))
            {
                Console.WriteLine("Right closed");
                connectingAxle.Right = newAxle;
                connectingAxle.rightOpen = false;
            }

            return newAxle;
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
                else if(buildingMode == StructureType.Motor)
                {
                    newObject = new Motor(platform.GetSlotPosition(slot).X, platform.GetSlotPosition(slot).Y, null, 3);
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
