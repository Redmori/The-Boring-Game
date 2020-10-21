using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFMLTest2
{


    public class Map
    {
        public  uint width;
        public  int height;
        public  uint progress;
        public int discoverDistance;
        public float tileSize;
        public  List<Tile[]> tiles;

        public List<Platform> platforms;
        public List<Cart> carts;
        public Cart drivingCart;
        public List<Ladder> ladders;

        public Map()
        {
            this.tiles = new List<Tile[]>();
            this.platforms = new List<Platform>();
            this.carts = new List<Cart>();
            this.ladders = new List<Ladder>();
            width = 100;
            height = 30;
            progress = width / 2;
            discoverDistance = 50;
            tileSize = 50f;

            InitMap();
        }


        public Tile TileAtCoords(float x, float y)
        {
            return TileAtIndex(CoordsToIndex(x, y));
        }

        public Vector2i CoordsToIndex(float x, float y)
        {
            //WHEN WE ARE MOVING THE REFERENCE FRAME MAP USE CODE LIKE THIS
            int ipos = (int)(Math.Round((x - progress * tileSize) / tileSize)) + (int)width / 2;
            //int ipos = (int)(Math.Round((x) / tileSize));
            int jpos = (int)(Math.Round((y) / tileSize));

            return new Vector2i(ipos, jpos);
        }

        public Tile TileAtIndex(Vector2i loc)
        {
            if (loc.X < 0 || loc.Y < 0 || loc.X >= width  || loc.Y >= height)
                return null;
            return tiles[loc.X][loc.Y];
        }

        public void AddColumn()
        {
            Tile[] newColumn = new Tile[height];
            for (int j = 0; j < height; j++)
            {
                bool minable = true;
                Sprite tileSprite;
                if (j == 0 | j == height - 1)
                {
                    tileSprite = SpriteManager.GetSprite(Type.Hard, progress * tileSize + discoverDistance * tileSize, j * tileSize);
                    minable = false;
                }
                else
                {
                    tileSprite = SpriteManager.GetSprite(Type.Ground, progress * tileSize + discoverDistance * tileSize, j * tileSize);
                }
                Tile newTile = new Tile(tileSprite);
                newTile.passable = false;
                newTile.minable = minable;
                newColumn[j] = newTile;
            }
            tiles.Add(newColumn);
            tiles.RemoveAt(0);
            ChangeColumn(Type.Hard, 0);
            progress++;
            Console.WriteLine("adding line: " + progress);
        }

        public void InitMap()
        {
            for (int i = 0; i < width; i++)
            {
                Tile[] newColumn = new Tile[height];
                for (int j = 0; j < height; j++)
                {
                    bool passable = true;
                    bool minable = true;
                    Type type = Type.Empty;

                    if (i == 0 || j == 0 || j == height - 1)
                    {
                        type = Type.Hard;
                        passable = false;
                        minable = false;
                    }
                    else if (i <= progress && j > 5 && j < 15 && i > 25)
                    {
                        type = Type.Empty;
                    }
                    else
                    {
                        type = Type.Ground;
                        passable = false;
                    }
                    Tile newTile = new Tile(SpriteManager.GetSprite(type, i * tileSize, j * tileSize));
                    newTile.passable = passable;
                    newTile.minable = minable;
                    newColumn[j] = newTile;

                }
                tiles.Add(newColumn);
            }
        }

        public void ChangeColumn(Type newType, int column)
        {
            for(int i = 0; i < height; i++)
            {
                tiles[column][i].SetType(newType);

            }
        }
    }
}
