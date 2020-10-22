using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BoringGame
{
    public class Player : GameObject
    {
        public float speed;
        public float halfSizeX;
        public float halfSizeY;

        public float mineDistance;

        public bool onLadder;
        public Inventory inventory;

        public Player(float x, float y, float spd, Texture player_tex) : base(x, y)
        {
            this.speed = spd;
            mineDistance = 100f;

            inventory = new Inventory();
            InitPlayer(player_tex);
        }

        public void MovePlayer(float dt, Map map)
        {
            //TODO: add constant downward movement unless collision
            // when that collision is there, you can move left and right with a and d
            // when you collide left/right against something, you can climb up and down with w and s

            //make function that moves certain direction for dt*speed, if no collision it just returns dt*speed, if collision it returns the clamp to that collision. This should work in all directions

            





            Tile currentTile = map.TileAtCoords(this.GetX(), this.GetY());


            float fallLength = dt * speed * 2f;
            float stepSize = dt * speed;
            float climbSpeed = 0.8f;


            if(!Keyboard.IsKeyPressed(Keyboard.Key.W) && !Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                onLadder = false;
            }



            if (!onLadder && CollisionCheckDown(fallLength, map) && !(Keyboard.IsKeyPressed(Keyboard.Key.A) && !CollisionCheckLeft(-stepSize, map)) && !(Keyboard.IsKeyPressed(Keyboard.Key.D) && !CollisionCheckRight(stepSize, map)))
            {
                this.MoveY(fallLength);
            }
            else
            {
                if (onLadder && (Keyboard.IsKeyPressed(Keyboard.Key.W) || Keyboard.IsKeyPressed(Keyboard.Key.S)))
                {
                    Ladder ladder = FindLadder(map);
                    if(ladder != null && CollisionCheckUp(-stepSize * climbSpeed, map) && (Keyboard.IsKeyPressed(Keyboard.Key.W)))
                    {
                        this.MoveY(-stepSize * climbSpeed);
                    }
                    if (ladder != null && CollisionCheckUp(stepSize * climbSpeed, map) && (Keyboard.IsKeyPressed(Keyboard.Key.S)))
                    {
                        this.MoveY(stepSize * climbSpeed);
                    }

                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                {
                    if (CollisionCheckLeft(-stepSize, map)) //Collision check Left side Top and Bottom corner
                    {
                        Ladder ladder = FindLadder(map);
                        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && ladder != null && CollisionCheckUp(-stepSize * climbSpeed, map))
                        {
                            if (onLadder)
                            {
                                this.MoveY(-stepSize * climbSpeed);
                            }
                            else
                            {
                                this.SetX(ladder.GetX());
                                onLadder = true;
                            }
                        }
                        else
                        {
                            this.MoveX(-stepSize);
                        }
                    }
                    else
                    {
                        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && CollisionCheckUp(-stepSize * climbSpeed, map))
                        {
                            this.MoveY(-stepSize * climbSpeed);
                            if (CollisionCheckLeft(-stepSize, map))
                                this.MoveX(-stepSize);
                        }
                        if (Keyboard.IsKeyPressed(Keyboard.Key.S) && CollisionCheckDown(stepSize * climbSpeed, map))
                            this.MoveY(stepSize * climbSpeed);
                    }
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                {
                    if (CollisionCheckRight(stepSize, map)) //Collision check Right side Top and Bottom corner
                    {
                        Ladder ladder = FindLadder(map);
                        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && ladder != null && CollisionCheckUp(-stepSize * climbSpeed, map))
                        {
                            if (onLadder)
                            {
                                this.MoveY(-stepSize * climbSpeed);
                            }
                            else
                            {
                                this.SetX(ladder.GetX());
                                onLadder = true;
                            }
                        }
                        else
                        {
                            this.MoveX(stepSize);
                        }
                    }
                    else
                    {
                        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && CollisionCheckUp(-stepSize * climbSpeed, map))
                        {
                            this.MoveY(-stepSize * climbSpeed);
                            if(CollisionCheckRight(stepSize, map))
                                this.MoveX(stepSize);
                        }
                        if (Keyboard.IsKeyPressed(Keyboard.Key.S) && CollisionCheckDown(stepSize * climbSpeed, map))
                            this.MoveY(stepSize * climbSpeed);
                    }
                }
            }



            //float stepSize = dt * speed;
            //if (Keyboard.IsKeyPressed(Keyboard.Key.S) && CollisionCheckDown(stepSize, map))
            //    this.MoveY(stepSize);  //Collision check Bottom side right and left corner
            //if (Keyboard.IsKeyPressed(Keyboard.Key.W) && CollisionCheckUp(-stepSize, map))
            //    this.MoveY(-stepSize); //Collision check Top side right and left corner
            //if (Keyboard.IsKeyPressed(Keyboard.Key.A) && CollisionCheckLeft(-stepSize,map))
            //    this.MoveX(-stepSize); //Collision check Left side Top and Bottom corner
            //if (Keyboard.IsKeyPressed(Keyboard.Key.D) && CollisionCheckRight(stepSize, map))
            //    this.MoveX(stepSize); //Collision check Right side Top and Bottom corner

        }

        public bool CollisionCheckDown(float dy, Map map)
        {
            if (!map.TileAtCoords(GetX() + halfSizeX, GetY() + dy + halfSizeY).passable || !map.TileAtCoords(GetX() - halfSizeX, GetY() + dy + halfSizeY).passable)
                return false;

            foreach (Platform platform in map.platforms)
            {
                if (GetY() + halfSizeY < platform.GetPlatformY() && GetY() + halfSizeY + dy > platform.GetPlatformY() && GetX() + halfSizeX > platform.GetX() - platform.halfWidth && GetX() - halfSizeX < platform.GetX() + platform.halfWidth)
                {
                    this.SetX(GetX() + platform.previousDX); //TODO collision with player and wall
                    return false;
                }
            }

            return true;

            //return map.TileAtCoords(GetX() + halfSizeX, GetY() + dy + halfSizeY).passable && map.TileAtCoords(GetX() - halfSizeX, GetY() + dy + halfSizeY).passable
        }

        public bool CollisionCheckUp(float dy, Map map)
        {
            return map.TileAtCoords(GetX() + halfSizeX, GetY() + dy - halfSizeY).passable && map.TileAtCoords(GetX() - halfSizeX, GetY() + dy - halfSizeY).passable;
        }

        public bool CollisionCheckLeft(float dx, Map map)
        {
            if (!map.TileAtCoords(GetX() + dx - halfSizeX, GetY() + halfSizeY).passable || !map.TileAtCoords(GetX() + dx - halfSizeX, GetY() - halfSizeY).passable)
                return false;

            foreach(Cart cart in map.carts)
            {
                if (GetY() + halfSizeY > cart.GetPlatformY() && GetY() - halfSizeY < cart.GetPlatformY() && GetX() - halfSizeX > cart.GetX() + cart.halfWidth && GetX() - halfSizeX + dx < cart.GetX() + cart.halfWidth)
                {
                    this.SetX(GetX() + cart.previousDX); //TODO: allow climbing with cart while its moving
                    return false;
                }
            }

            return true;

            //return map.TileAtCoords(GetX() + dx - halfSizeX, GetY() + halfSizeY).passable && map.TileAtCoords(GetX() + dx - halfSizeX, GetY() - halfSizeY).passable;
        }

        public bool CollisionCheckRight(float dx, Map map)
        {
            if (!map.TileAtCoords(GetX() + dx + halfSizeX, GetY() + halfSizeY).passable || !map.TileAtCoords(GetX() + dx + halfSizeX, GetY() - halfSizeY).passable)
                return false;

            foreach(Cart cart in map.carts)
            {
                if (GetY() + halfSizeY > cart.GetPlatformY() && GetY() - halfSizeY < cart.GetPlatformY() && GetX() + halfSizeX < cart.GetX() - cart.halfWidth && GetX() + halfSizeX + dx > cart.GetX() - cart.halfWidth)
                {
                    this.SetX(GetX() + cart.previousDX); //TODO: allow climbing with cart while its moving
                    return false;
                }
            }


            return true;

            //return map.TileAtCoords(GetX() + dx + halfSizeX, GetY() + halfSizeY).passable && map.TileAtCoords(GetX() + dx + halfSizeX, GetY() - halfSizeY).passable;
        }

        public Ladder FindLadder(Map map)
        {
            foreach(Ladder ladder in map.ladders)
            {
                if(Math.Abs(ladder.GetX() - GetX()) < 20f && Math.Abs(ladder.GetY() - GetY()) < 50f)
                return ladder;
            }

            return null;
        }

        public void InitPlayer(Texture player_tex)
        {
            Sprite newSpritePlayer = new Sprite(player_tex);
            newSpritePlayer.Origin = new Vector2f(newSpritePlayer.Texture.Size.X / 2f, newSpritePlayer.Texture.Size.Y / 2f);
            SetSprite(newSpritePlayer);
            halfSizeX = (float)newSpritePlayer.Texture.Size.X / 2f;
            halfSizeY = (float)newSpritePlayer.Texture.Size.Y / 2f;
        }
    }
}
