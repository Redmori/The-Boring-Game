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

        public bool MoveCart(float dt, Map map)
        {
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
    }
}
