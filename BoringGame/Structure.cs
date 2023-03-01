using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BoringGame
{
    public enum StructureType
    {
        Cart,
        Platform,
        Ladder,
        Drill,
        Furnace,
        Axle,
        Motor,
        Drillhead,
        Cog,

        Count
    }

    public class Structure : GameObject
    {
        public static float structureSize = 50f;

        public float weight;

        public Structure(float x, float y) : base(x, y)
        {
            weight = 1000;
        }

        public void Build()
        {

        }

        public bool CollisionCheckRight(float dx, Map map)
        {
            if (!map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X/2, GetY() + this.GetSprite().Texture.Size.Y/2).passable || !map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X/2, GetY() - this.GetSprite().Texture.Size.Y/2).passable)
                return false;

            return true;
        }

        public virtual float CollisionCheckRightN(float dx, Map map)
        {
            //WARNING CHECK DRILL COLLISION WHEN CHANGING THIS WARNING

            //Tile tile = map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X / 2, GetY());
            //if (tile == null)
            //    return 0;

            //if (!tile.passable)
            //    return Math.Max(0f, tile.sprite.Position.X - GetX() - map.tileSize / 2 - this.GetSprite().Texture.Size.X / 2);

            //return dx;

            float rightX = GetSprite().GetGlobalBounds().Left + GetSprite().GetGlobalBounds().Width;

            Tile tile = map.TileAtCoords(rightX + dx , GetY());
            if (tile == null)
                return 0;

            if (!tile.passable)
                return Math.Max(0f, tile.sprite.Position.X - map.tileSize / 2 - rightX);

            return dx;
        }

    }
}
