using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Reflection.Emit;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BoringGame
{
    class Program
    {
        public static bool debugFPS = true; //TEST fps counter
        public static Text fpsText;
        public static float totalTime = 0f;
        public static int numFrames = 0;
        public static float fps = 0f;

        public static ContextSettings context = new ContextSettings();
        public static RenderWindow window;
        public static View view;

        //TODO remove mouse and player loading and put it in SpriteManager
        public static Texture mouse_tex = new Texture("../../../Content/Mouse.png");
        public static Sprite mouse_sprite;

        public static Texture player_tex = new Texture("../../../Content/Player.png");

        public static uint windowWidth = 1024;
        public static uint windowHeight = 760;
        public static int windowX;
        public static int windowY;


        public static Player player;
        public static List<GameObject> gameObjects;
        public static List<UIText> uitexts;

        public static Map map;
        public static Bore bore;
        public static bool firstCart = true;

        public static bool mousePressed = false;

        public static InputState inputState = InputState.Game;
        public enum InputState
        {
            Game,
            Quitting,
            Building,
        }
        public static ScreenText stateText;


        static void Main(string[] args)
        {
            window = new RenderWindow(new VideoMode(windowWidth, windowHeight), "The Boring Game", Styles.Default, context);
            window.Closed += window_Closed;
            window.GainedFocus += window_GainedFocus;
            window.LostFocus += window_LostFocus;
            window.Resized += window_Resized;
            window.KeyPressed += window_KeyPressed;
            window.KeyReleased += window_KeyReleased;
            window.MouseButtonPressed += window_MouseButtonPressed;
            window.MouseButtonReleased += window_MouseButtonReleased;
            window.MouseMoved += window_MouseMoved;
            window.MouseEntered += window_MouseEntered;
            window.MouseLeft += window_MouseLeft;
            window.SetActive(true);
            mouse_sprite = new Sprite(mouse_tex);
            //window.SetFramerateLimit(60);

            windowX = window.Position.X;
            windowY = window.Position.Y;
            
            view = new View(new Vector2f(windowWidth/2f,windowHeight/2f), new Vector2f(windowWidth, windowHeight));
            window.SetView(view);
         
            Console.WriteLine("Window opened!");

            //Load sounds
            SoundManager.LoadSounds();

            //initialize the map
            map = new Map();
                        
            //initialize player
            player = new Player(map.progress* map.tileSize - 5 * map.tileSize, map.tileSize * (map.height - 4), 200f, player_tex) ;
            

            //initialize texts
            uitexts = new List<UIText>();

            stateText = new ScreenText("", new Vector2f(0, 0));
            stateText.text.Color = Color.Cyan;
            stateText.text.CharacterSize = 50;

            Font arial = new Font("../../../Content/ArialCEMTBlack.ttf");
            //Text exampleText = new Text("| 1 Cart | 2 Ladder | 3 Platform | 4 Drill | 5 Furnace | 6 Axle | 7 Motor | 8 Drillhead | 9 Cog |", arial);
            //exampleText.Color = Color.Red;
            //exampleText.CharacterSize = 21;
            //exampleText.Origin = new Vector2f(0, exampleText.GetLocalBounds().Height);
            //UIText exampleUIText = new UIText(exampleText, new Vector2f(-windowWidth/2 + 10f, windowHeight/2 - 20f));
            //uitexts.Add(exampleUIText);

            //FPS text
            fpsText = new Text(fps.ToString(), arial);
            fpsText.Color = Color.Green;
            fpsText.CharacterSize = 21;
            fpsText.Origin = new Vector2f(0, fpsText.GetLocalBounds().Height);
            UIText fpsUIText = new UIText(fpsText, new Vector2f(-windowWidth / 2 + 10f, -windowHeight / 2 + 20f));
            uitexts.Add(fpsUIText);


            //initialize gameobjects
            gameObjects = new List<GameObject>();

            //example gameobject
            GameObject newGameObject = new GameObject(windowWidth / 2, windowHeight / 2);
            Sprite newSprite = new Sprite(mouse_tex);
            newGameObject.SetSprite(newSprite);
            gameObjects.Add(newGameObject);



            DateTime currentTime = DateTime.Now;

            while (window.IsOpen)
            {
                mousePressed = false;
                DateTime newTime = DateTime.Now;
                float deltaTime = Math.Min(0.2f,(newTime.Ticks - currentTime.Ticks) / 10000000f);
                currentTime = newTime;
                calcFPS(deltaTime);
                //if (debugFPS)
                //{
                //    fps = (int)Math.Round(1f / deltaTime);
                //    fpsText.DisplayedString = fps.ToString();
                //    Console.WriteLine(1f / deltaTime);
                //}

                window.DispatchEvents();
                window.Clear(new SFML.Graphics.Color(0, 128, 160));


                SimulateGame(deltaTime);

                DrawGame();

                window.Display();

            }


        }

        public static void SimulateGame(float dt)
        {
            //Move the player and center the camera on him
            player.MovePlayer(dt, map);

            //Clamp the camera to the edges of the map
            float xpos = player.GetX();
            if (player.GetX() < view.Size.X / 2f + map.progress * map.tileSize - map.width / 2f * map.tileSize - map.tileSize/2f)   //Left side clamp
                xpos = view.Size.X / 2f + map.progress * map.tileSize - map.width / 2f * map.tileSize - map.tileSize / 2f;
            if (player.GetY() < view.Size.Y/2f - map.tileSize / 2f) //Top side clamp
                view.Center = new Vector2f(xpos, view.Size.Y/2f - map.tileSize / 2f);  
            else if(player.GetY() > map.height * map.tileSize - view.Size.Y / 2f - map.tileSize / 2f) //Bottom side clamp
                view.Center = new Vector2f(xpos, map.height * map.tileSize - view.Size.Y / 2f - map.tileSize / 2f);
            else
                view.Center = new Vector2f(xpos, player.GetY()); //No clamping
            window.SetView(view);

            //Mine blocks
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                Vector2i indexLoc = map.CoordsToIndex(window.MapPixelToCoords(Mouse.GetPosition(window)).X, window.MapPixelToCoords(Mouse.GetPosition(window)).Y);
                Tile tile = map.TileAtIndex(indexLoc);
                if (tile != null && tile.minable && !tile.passable && Math.Abs(player.GetX() - tile.sprite.Position.X) < player.mineDistance && Math.Abs(player.GetY() - tile.sprite.Position.Y) < player.mineDistance)
                {
                    player.inventory.ReceiveItem(tile.Mine(0.5f, map));
                    //{
                    //    tile.SetType(Type.Empty);
                    //    if (indexLoc.X > map.width / 2)
                    //    {
                    //        map.AddColumn();
                    //    }
                    //}
                }

                //click on a crafter structure
                if (bore?.StructureAtIndex((Vector2i)(bore?.CoordsToIndex(window.MapPixelToCoords(Mouse.GetPosition(window)))))?.crafter is Crafter clickedCrafter)
                {
                    Console.WriteLine("crafter " + (Vector2i)(bore?.CoordsToIndex(window.MapPixelToCoords(Mouse.GetPosition(window)))));
                    List<Item> loot = clickedCrafter.Loot();
                    clickedCrafter.AddInput(player.inventory.ConsumeItem(new Item(1001, 1))); //TEMP hardcoded to remove 1 dirt from inventory and put in crafter
                    if (loot != null)
                        foreach (Item item in loot)
                        {
                            player.inventory.ReceiveItem(item);
                        }
                }


            }
            if (Mouse.IsButtonPressed(Mouse.Button.Right)) //Destroying structures
            {
                Vector2i indexLoc = map.CoordsToIndex(window.MapPixelToCoords(Mouse.GetPosition(window)).X, window.MapPixelToCoords(Mouse.GetPosition(window)).Y);
                if (bore != null) { 
                    Vector2i index = (Vector2i)(bore?.CoordsToIndex(window.MapPixelToCoords(Mouse.GetPosition(window))));
                    if (bore?.StructureAtIndex(index) is Structure structDestroy)
                    {
                        //TODO, put this all in a Destroy() method of the structure?
                        if (bore.RemoveStructure(index))
                        {
                            player.inventory.ReceiveItem(new Item(structDestroy.id, 1));
                            gameObjects.Remove(structDestroy);
                        }

                    }
                }
            }

            ////Place Cart TEMP
            //if (Keyboard.IsKeyPressed(Keyboard.Key.Space) && map.drivingCart == null)
            //{
            //    Cart newCart = new Cart(window.MapPixelToCoords(Mouse.GetPosition(window)).X, map.tiles[0][map.height-2].sprite.Position.Y, map.tileSize);
            //    newCart.SetSprite(SpriteManager.GetStructureSprite(StructureType.Cart, newCart.GetX(), newCart.GetY()));

            //    gameObjects.Add(newCart);
            //    map.carts.Add(newCart);
            //    map.platforms.Add(newCart);
            //    map.drivingCart = newCart;
            //}


            //Move Carts
            if (bore != null)
            {
                bore.Update(dt);


                //TEST speedup:
                //map.drivingCart.cartSpeed = map.drivingCart.cartSpeed * 1.0001f;

                //float dx = map.drivingCart.cartSpeed * dt;
                //foreach(Structure structure in map.structures)
                //{
                //    dx = Math.Min(dx, structure.CollisionCheckRightN(dx, map));
                //}

                //foreach(Cart cart in map.carts)
                //{
                //    cart.MoveCartN(dx, map);
                //}
                ////TEMP moving axles, probably better way to do this
                //foreach (Axle axle in map.axles)
                //{
                //    if (!(axle is Motor))
                //    {
                //        axle.MoveX(dx);
                //    }
                //}


            }


            ////Place ladder TEMP
            //if (Keyboard.IsKeyPressed(Keyboard.Key.Num1))
            //{
            //    Ladder newLadder = new Ladder(window.MapPixelToCoords(Mouse.GetPosition(window)).X, window.MapPixelToCoords(Mouse.GetPosition(window)).Y);

            //    gameObjects.Add(newLadder);
            //    map.ladders.Add(newLadder);
            //}

            //Check the inventory for building
            GameObject newGameObject = player.inventory.CheckBuilding(window.MapPixelToCoords(Mouse.GetPosition(window)), map, bore);
            if(newGameObject != null)
            {
                gameObjects.Add(newGameObject);
                if(newGameObject is Structure)
                    //map.structures.Add((Structure)newGameObject);
                if(newGameObject is Ladder)
                    map.ladders.Add((Ladder)newGameObject);
                if(newGameObject is Cart && firstCart)
                {
                    bore = new Bore((Cart)newGameObject);
                    firstCart = false;
                }
                else if(newGameObject is Cart)
                {
                    bore.AddCart((Cart)newGameObject,2);
                }
            }



        }

        public static void DrawGame()
        {

            //Draw the map
            int ipos = map.CoordsToIndex(player.GetX(), player.GetY()).X;
            int renderDistance = map.height;
            int pos = 1;
            foreach(Tile[] column in map.tiles)
            {
                if (Math.Abs(pos - ipos) < renderDistance)
                {
                    for (int i = 0; i < map.height; i++)
                    {
                        column[i].sprite.Draw(window, RenderStates.Default);
                    }
                }
                pos++;
            }


            //Draw the GameObjects
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.GetSprite().Draw(window, RenderStates.Default);
            }

            //Draw the player
            player.GetSprite().Draw(window, RenderStates.Default);

            //Draw texts
            TextManager.Draw(window);

            //Draw the UI texts
            foreach (UIText text in uitexts)
            {
                text.UpdatePosition(view.Center);
                text.text.Draw(window, RenderStates.Default);
                //window.Draw(text.text, RenderStates.Default);
                //window.Draw(text.text);
            }
            player.inventory.DrawInventory(window, view);

            //TODO: implement world space texts, and timer texts etc
            UIManager.Update(window);

            //Draw building indicator
            Build.DrawBuildingSprite(window);

            //Draw state text
            stateText.UpdatePosition(view, new Vector2f(0, 0));    
            stateText.DrawElement(window);

            //Draw the cursor
            mouse_sprite.Position = window.MapPixelToCoords(Mouse.GetPosition(window));
            mouse_sprite.Draw(window, RenderStates.Default);

            //RectangleShape shape = new RectangleShape(new Vector2f(100, 100));
            //shape.Position = mouse_sprite.Position;
            //shape.Draw(window, RenderStates.Default);
        }

        public static void calcFPS(float dt)
        {
            if (!debugFPS)
                return;
            // Increment elapsed time and frame counter
            totalTime += dt;
            numFrames++;

            // If delta time is very high, give a warning:
            if (dt > 0.1f)
                Console.WriteLine("Lagspike detection: Long frame detected, took " + Math.Round(dt*1000) + " miliseconds to render.");

            // Check if one second has elapsed
            if (totalTime >= 1f)
            {
                // Calculate FPS
                fps = numFrames / totalTime;

                // Reset frame counter and elapsed time
                numFrames = 0;
                totalTime = 0f;

                // Print FPS to console (or display on screen)
                //Console.WriteLine("FPS: " + fps);
                fpsText.DisplayedString = Math.Round(fps).ToString();
            }
        }

        public static Vector2f GetMouse()
        {
            return Program.window.MapPixelToCoords(Mouse.GetPosition(Program.window));
        }
        public static float GetMouseX()
        {
            return Mouse.GetPosition(window).X;
        }

        public static float GetMouseY()
        {
            return Mouse.GetPosition(window).Y;
        }

        #region window interactions
        private static void window_MouseLeft(object sender, EventArgs e)
        {
            window.SetMouseCursorVisible(true);
        }

        private static void window_MouseEntered(object sender, EventArgs e)
        {
            window.SetMouseCursorVisible(false);
        }

        private static void window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            //mouse_sprite.Position = new Vector2f(e.X, e.Y);
        }

        private static void window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {

        }

        private static void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            mousePressed = true;

            bool clicked = false;
            if(!clicked)
            _ = player.inventory.CheckClick(new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y));

            inputState = InputState.Game;
        }

        static void window_KeyReleased(object sender, KeyEventArgs e)
        {

        }
        static void window_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                if (inputState == InputState.Quitting)
                    window.Close();
                if (!Build.building)
                {
                    stateText.text.DisplayedString = "Quit?";
                    stateText.text.Origin = new Vector2f(stateText.text.GetLocalBounds().Width * 0.5f, 0);
                    inputState = InputState.Quitting;
                }
            }
            else
            {
                inputState = InputState.Game;
                stateText.text.DisplayedString = "";
            }
            if (e.Code == Keyboard.Key.Space)
            {
                bore.SetSpeed(bore.GetSpeed() * 2); // TEST parameter increase speeeeed
            }
            if (e.Code == Keyboard.Key.P)
            {
                Drillhead.drillPower *= 2; // TEST parameter increase drillpower
            }
            if(e.Code == Keyboard.Key.Tab)
            {
                UIManager.TogglePopup(player.inventory.popup);
            }
        }
        private static void window_Resized(object sender, SizeEventArgs e)
        {

        }

        private static void window_LostFocus(object sender, EventArgs e)
        {

        }

        private static void window_GainedFocus(object sender, EventArgs e)
        {

        }

        static void window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }

        #endregion
    }
}
