using System;
using System.Collections.Generic;
using System.Text;

namespace SFMLTest2
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

        public void MoveCart(float dt, Map map)
        {
            if (this.CollisionCheckRight(cartSpeed * dt, map))
            {
                SetX(GetX() + cartSpeed * dt);
                MovePlatform(cartSpeed * dt);
            }
            else
            {
                MovePlatform(0);
            }


            //TODO move all platforms ontop of this
        }
    }
}
