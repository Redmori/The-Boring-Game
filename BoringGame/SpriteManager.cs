using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace BoringGame
{
    class SpriteManager
    {
        public static string path = "../../../Content/";
        public static Texture empty_tex = new Texture(path + "Empty.png");
        public static Texture ground_tex = new Texture(path + "Ground.png");
        public static Texture rock_tex = new Texture(path + "Rock.png");
        public static Texture hard_tex = new Texture(path + "Hard.png");
        public static Texture player_tex = new Texture(path + "Player.png");
        public static Texture mouse_tex = new Texture(path + "Mouse.png");


        public static Texture cart_tex = new Texture(path + "Cart.png");
        public static Texture ladder_tex = new Texture(path + "Ladder.png");
        public static Texture platform_tex = new Texture(path + "Platform.png");
        public static Texture drill_tex = new Texture(path + "Drill.png");
        public static Texture furnace_tex = new Texture(path + "Furnace.png");
        public static Texture axle_tex = new Texture(path + "Axle.png");
        public static Texture motor_tex = new Texture(path + "Motor.png");
        public static Texture drillhead_tex = new Texture(path + "Drillhead.png");

        public static Texture test_tex = new Texture(path + "testtileset.png");

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
                case StructureType.Drill:
                    {
                            structSprite = new Sprite(drill_tex);
                            structSprite.Origin = new Vector2f(structSprite.Texture.Size.X / 4f, structSprite.Texture.Size.Y / 2f);
                            break;
                    }
                case StructureType.Furnace:
                    {
                        structSprite = new Sprite(furnace_tex);
                        break;
                    }
                case StructureType.Axle:
                    {
                        structSprite = new Sprite(axle_tex);
                        break;
                    }
                case StructureType.Motor:
                    {
                        structSprite = new Sprite(motor_tex);
                        break;
                    }
                case StructureType.Drillhead:
                    {
                        structSprite = new Sprite(drillhead_tex);
                        break;
                    }
            }
            if(type != StructureType.Ladder && type != StructureType.Drill)
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
            newSprite.Origin = new Vector2f(25f, 25f);//new Vector2f(newSprite.Texture.Size.X / 2f, newSprite.Texture.Size.Y / 2f);
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
                        tileSprite = MakeTileSprite(test_tex);//new Sprite(ground_tex);
                        break;
                }
                case Type.Hard:
                {
                    tileSprite = new Sprite(hard_tex);
                    break;
                }
                case Type.Rock:
                    {
                        tileSprite = new Sprite(rock_tex);
                        break;
                    }

            }

            return tileSprite;
        }

        public static Sprite MakeTileSprite(Texture tex)
        {
            int nx = (int)tex.Size.X / 50;
            int ny = (int)tex.Size.Y / 50;
            Random r = new Random();
            int rx = r.Next(0, nx);
            int ry = r.Next(0, ny);
            return new Sprite(tex, new IntRect(rx * 50, ry* 50, 50, 50));

        }

    }
}
