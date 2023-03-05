using SFML.Graphics;
using SFML.System;
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

        public static new Cart UpdateBuilding(Vector2f mousePos, Map map, Bore bore, int id)
        {
            if (bore != null)
            {
                //float totalWidth = 0;
                //foreach (Cart cart in map.carts)
                //{
                //    totalWidth += cart.halfWidth * 2;
                //}
                //totalWidth = totalWidth - map.drivingCart.halfWidth + map.tileSize; //TODO map.tileSize = halfwidth of the placing cart
                //buildingSprite.Position = new Vector2f(map.drivingCart.GetX() - totalWidth, map.tiles[0][map.height - 2].sprite.Position.Y);

                Vector2f backpos = bore.IndextoCoords(new Vector2i(bore.GetSize().X, 0));

                SpriteManager.UpdateBuildingPos(backpos);
                //buildingSprite.Position = backpos;

                if (Program.mousePressed)
                    return Cart.Place(backpos.X, backpos.Y, map);
                //    return PlaceCart(map.drivingCart.GetX() - totalWidth, map.tiles[0][map.height - 2].sprite.Position.Y, map);
            }
            else
            {
                SpriteManager.UpdateBuildingPos(new Vector2f(mousePos.X, map.tiles[0][map.height - 2].sprite.Position.Y));

                if (Program.mousePressed)
                    return Cart.Place(mousePos.X, map.tiles[0][map.height - 2].sprite.Position.Y, map);
            }
            return null;
        }

        public static new Cart Place(float x, float y, Map map)
        {
            Build.building = false;
            Cart newCart = new Cart(x, y, map.tileSize);


            SpriteManager.Build(newCart);

            map.carts.Add(newCart);
            map.platforms.Add(newCart);
            if (map.drivingCart == null)
                map.drivingCart = newCart;

            return newCart;
        }
    }
}
