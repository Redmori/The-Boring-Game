using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SFMLTest2
{
    public class Player : GameObject
    {
        public float speed;
        public float halfSizeX;
        public float halfSizeY;

        public float mineDistance;

        public Player(float x, float y, float spd, Texture player_tex) : base(x, y)
        {
            this.speed = spd;
            mineDistance = 100f;

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






            if (CollisionCheckDown(fallLength, map) && !(Keyboard.IsKeyPressed(Keyboard.Key.A) && !CollisionCheckLeft(-stepSize, map)) && !(Keyboard.IsKeyPressed(Keyboard.Key.D) && !CollisionCheckRight(stepSize, map)))
            {
                this.MoveY(fallLength);
            }
            else
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                {
                    if (CollisionCheckLeft(-stepSize, map)) //Collision check Left side Top and Bottom corner
                    {
                        this.MoveX(-stepSize);
                    }
                    else
                    {
                        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && CollisionCheckUp(-stepSize * climbSpeed, map))
                        {
                            this.MoveY(-stepSize * climbSpeed);
                            if (CollisionCheckRight(-stepSize, map))
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
                        this.MoveX(stepSize);
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
            return map.TileAtCoords(GetX() + halfSizeX, GetY() + dy + halfSizeY).passable && map.TileAtCoords(GetX() - halfSizeX, GetY() + dy + halfSizeY).passable;
        }

        public bool CollisionCheckUp(float dy, Map map)
        {
            return map.TileAtCoords(GetX() + halfSizeX, GetY() + dy - halfSizeY).passable && map.TileAtCoords(GetX() - halfSizeX, GetY() + dy - halfSizeY).passable;
        }

        public bool CollisionCheckLeft(float dx, Map map)
        {
            return map.TileAtCoords(GetX() + dx - halfSizeX, GetY() + halfSizeY).passable && map.TileAtCoords(GetX() + dx - halfSizeX, GetY() - halfSizeY).passable;
        }

        public bool CollisionCheckRight(float dx, Map map)
        {
            return map.TileAtCoords(GetX() + dx + halfSizeX, GetY() + halfSizeY).passable && map.TileAtCoords(GetX() + dx + halfSizeX, GetY() - halfSizeY).passable;
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
