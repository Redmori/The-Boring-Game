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

        public Tile(Sprite spr)
        {
            sprite = spr;
            passable = true;
            minable = true;
        }

        public void SetType(Type newType)
        {
            sprite = SpriteManager.ChangeSprite(newType, sprite);
            switch (newType)
            {
                case Type.Empty:
                {
                    passable = true;
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
    }
}
