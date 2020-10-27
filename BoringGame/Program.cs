using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
//test
namespace BoringGame
{
    class Program
    {
        public static bool debugFPS = false;

        public static ContextSettings context = new ContextSettings();
        public static RenderWindow window;
        public static View view;

        //TODO remove mouse and player loading and put it in SpriteManager
        public static Texture mouse_tex = new Texture("../../Content/Mouse.png");
        public static Sprite mouse_sprite;

        public static Texture player_tex = new Texture("../../Content/Player.png");

        public static uint windowWidth = 1024;
        public static uint windowHeight = 760;
        public static int windowX;
        public static int windowY;


        public static Player player;
        public static List<GameObject> gameObjects;
        public static List<Text> texts;

        public static Map map;
        


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
            texts = new List<Text>();

            Font arial = new Font("../../Content/ArialCEMTBlack.ttf");
            Text exampleText = new Text("test", arial);
            exampleText.Position = new Vector2f(windowWidth / 2, windowHeight / 2);
            texts.Add(exampleText);


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
                DateTime newTime = DateTime.Now;
                float deltaTime = Math.Min(0.2f,(newTime.Ticks - currentTime.Ticks) / 10000000f);
                currentTime = newTime;
                if(debugFPS)
                    Console.WriteLine(1f / deltaTime);

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
                    tile.SetType(Type.Empty);
                    if(indexLoc.X > map.width/2)
                    {
                        map.AddColumn();
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
            if (map.drivingCart != null)
            {
                float dx = map.drivingCart.cartSpeed * dt;
                foreach(Structure structure in map.structures)
                {
                    dx = structure.CollisionCheckRightN(dx, map);
                }

                foreach(Cart cart in map.carts)
                {
                    cart.MoveCartN(dx, map);
                }

                //bool moved = map.drivingCart.MoveCart(dt, map);
                //foreach(Cart cart in map.carts)
                //{
                //    if(cart != map.drivingCart) //move all other carts if the driving cart moved
                //        cart.MoveCart(moved ? dt : 0, map);
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
            GameObject newGameObject = player.inventory.CheckBuilding(window.MapPixelToCoords(Mouse.GetPosition(window)), map);
            if(newGameObject != null)
            {
                gameObjects.Add(newGameObject);
                if(newGameObject is Structure)
                    map.structures.Add((Structure)newGameObject);
                if(newGameObject is Ladder)
                    map.ladders.Add((Ladder)newGameObject);
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


            //Draw the texts
            foreach (Text text in texts)
            {
                text.Draw(window, RenderStates.Default);
            }

            //Draw building indicator
            player.inventory.DrawBuildingSprite(window);


            //Draw the cursor
            mouse_sprite.Position = window.MapPixelToCoords(Mouse.GetPosition(window));
            mouse_sprite.Draw(window, RenderStates.Default);


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

        }

        static void window_KeyReleased(object sender, KeyEventArgs e)
        {

        }
        static void window_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                if(!player.inventory.building)
                        window.Close();
            }
            if (e.Code == Keyboard.Key.Space)
            {

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
