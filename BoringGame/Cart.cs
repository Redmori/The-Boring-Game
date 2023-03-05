using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
{
    public class Cart : Platform
    {
        public float cartSpeed;
        public List<Platform> platforms;

        public Cart(float x, float y, float width) : base(x, y, width)
        {
            cartSpeed = 50;
            platforms = new List<Platform>();
        }

        public override void MoveX(float dx)
        {

            foreach (Platform platform in platforms)
            {
                platform.MoveX(dx);
            }

            base.MoveX(dx);
        }

        public bool MoveCart(float dt, Map map)
        {
            Console.WriteLine("is this running?");
            if (this.CollisionCheckRight(cartSpeed * dt, map) && dt != 0)
            {
                MovePlatform(cartSpeed * dt);

                foreach(Platform platform in platforms)
                {
                    platform.MovePlatform(cartSpeed * dt);
                }
                return true;
            }
            else
            {
                MovePlatform(0);

                foreach (Platform platform in platforms)
                {
                    platform.MovePlatform(0);
                }
                return false;
            }


            //TODO move all platforms ontop of this
        }

        public void MoveCartN(float dx, Map map)
        {
            MovePlatform(dx);

            foreach (Platform platform in platforms)
            {
                platform.MovePlatform(dx);
            }



            //TODO move all platforms ontop of this
        }

        public static new Cart Place(float x, float y, Map map)
        {
            Build.building = false;
            Cart newCart = new Cart(x, y, map.tileSize);

            newCart.SetSprite(Build.buildingSprite);
            Build.buildingSprite.Color = new Color(255, 255, 255, 255);
            Build.buildingSprite = null;

            map.carts.Add(newCart);
            map.platforms.Add(newCart);
            if (map.drivingCart == null)
                map.drivingCart = newCart;

            return newCart;
        }
    }
}
