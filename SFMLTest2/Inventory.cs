using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFMLTest2
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
            contents[0] = StructureType.Ladder;
        }

        public GameObject CheckBuilding(Vector2f mousePos, Map map)
        {
           
            if (!building && Keyboard.IsKeyPressed(Keyboard.Key.B)) //Start building mode
            {
                StartBuilding();
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
            return null;
        }

        public void StartBuilding()
        {
            buildingMode = contents[0];
            buildingSprite = SpriteManager.GetStructureSprite(buildingMode);
            buildingSprite.Color = new Color(0, 255, 0, 128);
            building = true;
        }

        public void CancelBuilding()
        {
            building = false;
            buildingSprite = null;
        }

        public GameObject PlaceBuilding(Platform platform, int slot) 
        {
            building = false;
            buildingSprite = null;

            if (platform.structures[slot] == null)
            {
                if (buildingMode == StructureType.Ladder)
                {
                    Ladder newLadder = new Ladder(platform.GetSlotPosition(slot).X, platform.GetSlotPosition(slot).Y);
                    platform.BuildStructure(newLadder, slot);

                    return newLadder;
                }
                else if(buildingMode == StructureType.Platform)
                {
                    //TODO create platform building
                    return null;
                }
                else if(buildingMode == StructureType.Cart)
                {
                    //TODO create cart building
                    return null;
                }
                else
                {
                    Structure newObject = new Structure(platform.GetSlotPosition(slot).X, platform.GetSlotPosition(slot).Y);
                    platform.BuildStructure(newObject, slot);

                    return newObject;
                }
            }
            else
            {
                //TODO place error sound to indicate the building slot is full
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
