using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
{
    public enum Type{
        Empty,
        Ground,
        Hard,

        Count
    }
    public class Tile
    {
        public Type type;
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

            health = 100;
            healthFull = 100;
        }

        public void SetType(Type newType)
        {
            sprite = SpriteManager.ChangeSprite(newType, sprite);
            switch (newType)
            {
                case Type.Empty:
                {
                    passable = true;
                        minable = false;
                    break;
                }
                case Type.Ground:
                {
                    passable = false;
                    minable = true;
                    break;
                }
                case Type.Hard:
                {
                    passable = false;
                    minable = false;
                    break;
                }
            }
        }

        public void Mine(float power , Map map)
        {
            health -= power;

            if(health <= 0)
            {
                int indexLoc = map.CoordsToIndex(this.sprite.Position.X, 0).X;
                this.SetType(Type.Empty);
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
