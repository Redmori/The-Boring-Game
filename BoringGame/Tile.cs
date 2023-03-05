using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
{
    public enum TileType{
        Empty,
        Ground,
        Rock,
        Hard,

        Count
    }
    public class Tile
    {
        public TileType type;
        public Sprite sprite;
        public bool passable;
        public bool minable;

        public float health;
        public float healthFull;

        public Tile(Sprite spr)
        {
            sprite = spr;
            passable = true;
            minable = true;

            SetHealth(50);
        }

        public void SetType(TileType newType)
        {
            type = newType;
            sprite = SpriteManager.ChangeSprite(newType, sprite);
            switch (newType)
            {
                case TileType.Empty:
                {
                    passable = true;
                        minable = false;
                    break;
                }
                case TileType.Ground:
                {
                    passable = false;
                    minable = true;
                        break;
                }
                case TileType.Hard:
                {
                    passable = false;
                    minable = false;
                    break;
                }
                case TileType.Rock:
                    {
                        passable = false;
                        minable = true;
                        SetHealth(100);
                        break;
                    }
            }
        }
        public void SetHealth(int h)
        {
            health = h;
            healthFull = h;
        }
        public void Mine(float power , Map map)
        {
            health -= power;

            if(health <= 0)
            {
                int indexLoc = map.CoordsToIndex(this.sprite.Position.X, 0).X;
                this.SetType(TileType.Empty);
                if (indexLoc > map.width / 2)
                {
                    map.AddColumn();
                }
            }

        }

        public float RelativeHealth()
        {
            if (health <= 0)
                return 0f;
            return health / healthFull;
        }
    }
}
