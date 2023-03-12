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

        int id;

        public Tile(Sprite spr)
        {
            sprite = spr;
            passable = true;
            minable = true;

            id = 2001; //TEMP id

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
                        id = 2001; //TEMP id
                        
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
                        id = 2002; //TEMP id
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
        public Item Mine(float power , Map map)
        {
            health -= power;

            if(type!= TileType.Empty && health <= 0)
            {
                int indexLoc = map.CoordsToIndex(this.sprite.Position.X, 0).X;
                this.SetType(TileType.Empty);
                if (indexLoc > map.width / 2)
                {
                    map.AddColumn();
                }
                if (id == 2001)
                    return new Item(1001, 1); //TEMP default dirt when mining
                else if (id == 2002)
                    return new Item(1002, 3); //TEMP default resource when mining
            }
            return null;

        }

        public float RelativeHealth()
        {
            if (health <= 0)
                return 0f;
            return health / healthFull;
        }
    }
}
