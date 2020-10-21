using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFMLTest2
{
    class SpriteManager
    {

        public static Texture empty_tex = new Texture("../../Content/Empty.png");
        public static Texture ground_tex = new Texture("../../Content/Ground.png");
        public static Texture hard_tex = new Texture("../../Content/Hard.png");
        public static Texture player_tex = new Texture("../../Content/Player.png");
        public static Texture mouse_tex = new Texture("../../Content/Mouse.png");


        public static Texture cart_tex = new Texture("../../Content/Cart.png");
        public static Texture ladder_tex = new Texture("../../Content/Ladder.png");
        public static Texture platform_tex = new Texture("../../Content/Platform.png");

        public static Sprite GetStructureSprite(StructureType type, float x, float y )
        {
            Sprite newSprite = GetStructureSprite(type);
            newSprite.Position = new Vector2f(x, y);
            return newSprite;
        }

        public static Sprite GetStructureSprite(StructureType type)
        {
            Sprite structSprite = null;
            switch (type)
            {
                case StructureType.Cart:
                {
                        structSprite = new Sprite(cart_tex);
                         break;
                }
                case StructureType.Platform:
                    {
                        structSprite = new Sprite(platform_tex);
                        break;
                }
                case StructureType.Ladder:
                    {
                        structSprite = new Sprite(ladder_tex);
                        structSprite.Origin = new Vector2f(structSprite.Texture.Size.X / 2f, 3* structSprite.Texture.Size.Y / 4f);
                        break;
                }
            }
            if(type != StructureType.Ladder)
                structSprite.Origin = new Vector2f(structSprite.Texture.Size.X / 2f, structSprite.Texture.Size.Y / 2f);
            return structSprite;
        }

        public static Sprite ChangeSprite(Type type, Sprite oldSprite)
        {
            return GetSprite(type, oldSprite.Position.X, oldSprite.Position.Y);
        }
        public static Sprite GetSprite(Type type, float x, float y)
        {
            Sprite newSprite = GetSprite(type);
            newSprite.Origin = new Vector2f(newSprite.Texture.Size.X / 2f, newSprite.Texture.Size.Y / 2f);
            newSprite.Position = new Vector2f(x,y);
            return newSprite;
        }
        public static Sprite GetSprite(Type type)
        {
            Sprite tileSprite = null;
            switch (type)
            {
                case Type.Empty:
                {
                        tileSprite = new Sprite(empty_tex);
                        break;
                }
                case Type.Ground:
                {
                        tileSprite = new Sprite(ground_tex);
                        break;
                }
                case Type.Hard:
                {
                    tileSprite = new Sprite(hard_tex);
                    break;
                }

            }

            return tileSprite;
        }

    }
}
