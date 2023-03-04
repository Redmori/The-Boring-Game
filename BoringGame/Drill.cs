using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
{
    public class Drill : Structure
    {
        public float drillPower;

        public Drill(float x, float y, int id) : base(x, y, id)
        {
            drillPower = 0.1f;
        }

        public override float CollisionCheckRightN(float dx, Map map)
        {

            Tile tile = map.TileAtCoords(GetX() + dx + 3* this.GetSprite().Texture.Size.X / 4, GetY());
            if (tile == null)
                return 0;

            if (tile.minable)
            {
                tile.Mine(drillPower, map);
            }

            if(tile.minable && !tile.passable)
            {
                return Math.Max(0f, tile.sprite.Position.X - GetX() - tile.RelativeHealth() * map.tileSize /2  - 3 * this.GetSprite().Texture.Size.X / 4);
            }

            if (!tile.passable)
                return Math.Max(0f, tile.sprite.Position.X - GetX() - map.tileSize / 2 - 3*  this.GetSprite().Texture.Size.X / 4);

            return dx;
        }

    }
}
