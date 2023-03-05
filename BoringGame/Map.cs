using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
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
        public List<Structure> structures;
        public List<Axle> axles;

        public Map()
        {
            this.tiles = new List<Tile[]>();
            this.platforms = new List<Platform>();
            this.carts = new List<Cart>();
            this.ladders = new List<Ladder>();
            this.structures = new List<Structure>();
            this.axles = new List<Axle>();
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

        public void CreateCluster(int x, int y, TileType type)
        {
            if (y > 1 && y < height - 1 && x > 1 && x < width && tiles[x][y] != null && tiles[x][y].type == TileType.Ground)
            {
                tiles[x][y].SetType(type);
                Random r = new Random();
                if (r.Next(0, 100) > 70)
                    CreateCluster(x + 1, y, type);
                if (r.Next(0, 100) > 70)
                    CreateCluster(x - 1, y, type);
                if (r.Next(0, 100) > 70)
                    CreateCluster(x, y + 1, type);
                if (r.Next(0, 100) > 70)
                    CreateCluster(x, y - 1, type);
            }
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
                    tileSprite = SpriteManager.GetSprite(TileType.Hard, progress * tileSize + discoverDistance * tileSize, j * tileSize);
                    minable = false;
                }
                else
                {
                    tileSprite = SpriteManager.GetSprite(TileType.Ground, progress * tileSize + discoverDistance * tileSize, j * tileSize);
                }
                Tile newTile = new Tile(tileSprite);
                newTile.passable = false;
                newTile.minable = minable;
                newTile.type = TileType.Ground;
                newColumn[j] = newTile;
            }
            tiles.Add(newColumn);
            tiles.RemoveAt(0);
            ChangeColumn(TileType.Hard, 0);
            progress++;
            Console.WriteLine("adding line: " + progress);

            //Add resources
            Random r = new Random();
            for (int j = 0; j < height; j++)
            {
                if (r.Next(0, 100) > 97)
                {
                    CreateCluster((int)width - 1, j, TileType.Rock);
                    //TODO: clusters made on the map cant propogate to the right, make it so that these propogations are saved and created when another column is added
                }
            }
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
                    TileType type = TileType.Empty;

                    if (i == 0 || j == 0 || j == height - 1)
                    {
                        type = TileType.Hard;
                        passable = false;
                        minable = false;
                    }
                    else if (i <= progress && j > 20  && i > 25)
                    {
                        type = TileType.Empty;
                    }
                    else
                    {
                        type = TileType.Ground;
                        passable = false;
                    }
                    Tile newTile = new Tile(SpriteManager.GetSprite(type, i * tileSize, j * tileSize));
                    newTile.type = type;
                    newTile.passable = passable;
                    newTile.minable = minable;
                    newColumn[j] = newTile;

                }
                tiles.Add(newColumn);
            }

            Random r = new Random();
            for (int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if(r.Next(0,100) > 97)
                    {
                        CreateCluster(i,j, TileType.Rock);
                    }
                }
            }
        }

        public void ChangeColumn(TileType newType, int column)
        {
            for(int i = 0; i < height; i++)
            {
                tiles[column][i].SetType(newType);

            }
        }
    }
}
