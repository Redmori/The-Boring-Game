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


            //if(!Keyboard.IsKeyPressed(Keyboard.Key.W) && !Keyboard.IsKeyPressed(Keyboard.Key.S))
            //{
            //    onLadder = false;
            //}


            //if (!onLadder && CollisionCheckDown(fallLength, map) && !(Keyboard.IsKeyPressed(Keyboard.Key.A) && !CollisionCheckLeft(-stepSize, map)) && !(Keyboard.IsKeyPressed(Keyboard.Key.D) && !CollisionCheckRight(stepSize, map)))
            //{
            //    this.MoveY(fallLength);
            //}
            //else
            //{
            //    if (onLadder && (Keyboard.IsKeyPressed(Keyboard.Key.W) || Keyboard.IsKeyPressed(Keyboard.Key.S)))
            //    {
            //        Ladder ladder = FindLadder(map);
            //        if(ladder != null && CollisionCheckUp(-stepSize * climbSpeed, map) && (Keyboard.IsKeyPressed(Keyboard.Key.W)))
            //        {
            //            this.MoveY(-stepSize * climbSpeed);
            //        }
            //        if (ladder != null && CollisionCheckUp(stepSize * climbSpeed, map) && (Keyboard.IsKeyPressed(Keyboard.Key.S)))
            //        {
            //            this.MoveY(stepSize * climbSpeed);
            //        }

            //    }
            //    if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            //    {
            //        if (CollisionCheckLeft(-stepSize, map)) //Collision check Left side Top and Bottom corner
            //        {
            //            Ladder ladder = FindLadder(map);
            //            if (Keyboard.IsKeyPressed(Keyboard.Key.W) && ladder != null && CollisionCheckUp(-stepSize * climbSpeed, map))
            //            {
            //                if (onLadder)
            //                {
            //                    this.MoveY(-stepSize * climbSpeed);
            //                }
            //                else
            //                {
            //                    this.SetX(ladder.GetX());
            //                    onLadder = true;
            //                }
            //            }
            //            else
            //            {
            //                this.MoveX(-stepSize);
            //            }
            //        }
            //        else
            //        {
            //            if (Keyboard.IsKeyPressed(Keyboard.Key.W) && CollisionCheckUp(-stepSize * climbSpeed, map))
            //            {
            //                this.MoveY(-stepSize * climbSpeed);
            //                if (CollisionCheckLeft(-stepSize, map))
            //                    this.MoveX(-stepSize);
            //            }
            //            if (Keyboard.IsKeyPressed(Keyboard.Key.S) && CollisionCheckDown(stepSize * climbSpeed, map))
            //                this.MoveY(stepSize * climbSpeed);
            //        }
            //    }
            //    if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            //    {
            //        if (CollisionCheckRight(stepSize, map)) //Collision check Right side Top and Bottom corner
            //        {
            //            Ladder ladder = FindLadder(map);
            //            if (Keyboard.IsKeyPressed(Keyboard.Key.W) && ladder != null && CollisionCheckUp(-stepSize * climbSpeed, map))
            //            {
            //                if (onLadder)
            //                {
            //                    this.MoveY(-stepSize * climbSpeed);
            //                }
            //                else
            //                {
            //                    this.SetX(ladder.GetX());
            //                    onLadder = true;
            //                }
            //            }
            //            else
            //            {
            //                this.MoveX(stepSize);
            //            }
            //        }
            //        else
            //        {
            //            if (Keyboard.IsKeyPressed(Keyboard.Key.W) && CollisionCheckUp(-stepSize * climbSpeed, map))
            //            {
            //                this.MoveY(-stepSize * climbSpeed);
            //                if(CollisionCheckRight(stepSize, map))
            //                    this.MoveX(stepSize);
            //            }
            //            if (Keyboard.IsKeyPressed(Keyboard.Key.S) && CollisionCheckDown(stepSize * climbSpeed, map))
            //                this.MoveY(stepSize * climbSpeed);
            //        }
            //    }
            //}



            //float stepSize = dt * speed;
            //if (Keyboard.IsKeyPressed(Keyboard.Key.S) && CollisionCheckDown(stepSize, map))
            //    this.MoveY(stepSize);  //Collision check Bottom side right and left corner
            //if (Keyboard.IsKeyPressed(Keyboard.Key.W) && CollisionCheckUp(-stepSize, map))
            //    this.MoveY(-stepSize); //Collision check Top side right and left corner
            //if (Keyboard.IsKeyPressed(Keyboard.Key.A) && CollisionCheckLeft(-stepSize,map))
            //    this.MoveX(-stepSize); //Collision check Left side Top and Bottom corner
            //if (Keyboard.IsKeyPressed(Keyboard.Key.D) && CollisionCheckRight(stepSize, map))
            //    this.MoveX(stepSize); //Collision check Right side Top and Bottom corner



            if (Keyboard.IsKeyPressed(Keyboard.Key.W) && OnLadder(map)) //TODO check if on ladder or climbing something i guess

                this.MoveY(CollisionCheckUp(-climbSpeed * stepSize, map));
            else
                this.MoveY(CollisionCheckDown(fallLength, map));

            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                this.MoveX(CollisionCheckRight(stepSize, map));

            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                this.MoveX(CollisionCheckLeft(-stepSize, map));

        }

        //TEMP doesnt work
        public Vector2f CollsionCheck(Vector2f delta, Map map)
        {
            //Check for tile at the position where we move to
            Tile targetTile = map.TileAtCoords(GetX() + delta.X, GetY() + delta.Y); //TODO: account for halfsizes, maybe try aabb collision check?

            if (targetTile != null) //if the tile exists
            {
                if (targetTile.passable)
                { //if the tile is passable, just move there
                    return delta;
                }
                else //if the tile is not passable, find the edge so we can move there
                {
                    if(delta.X > 0) //going right
                    {

                    }
                    else if(delta.X < 0) //going left
                    {

                    }

                    if(delta.Y > 0) //falling or going down
                    {

                    }
                    else if (delta.Y < 0) //going up
                    {

                    }

                    return new Vector2f(delta.X / 2, delta.Y / 2);
                }
            }
            else //tile does not exist, dont move
            {
                return new Vector2f(0f, 0f);
            }
        }

        public float CollisionCheckDown(float dy, Map map)
        {
            Tile SE = map.TileAtCoords(GetX() + halfSizeX, GetY() + dy + halfSizeY);
            Tile SW = map.TileAtCoords(GetX() - halfSizeX, GetY() + dy + halfSizeY);

            if (SE == null || SW == null)
                return 0f;

            
            if (!SE.passable)
            {
                dy = Math.Min(dy, SE.sprite.Position.Y  - map.tileSize /2 - GetY() - halfSizeY - 0.001f);
            }
            if (!SW.passable)
            {
                dy = Math.Min(dy, SW.sprite.Position.Y - map.tileSize / 2 - GetY() - halfSizeY - 0.001f);
            }

            if (!Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                foreach (Platform platform in map.platforms)
                {
                    if (GetY() + halfSizeY < platform.GetPlatformY() && GetY() + halfSizeY + dy > platform.GetPlatformY() && GetX() + halfSizeX > platform.GetX() - platform.halfWidth*1.5f && GetX() - halfSizeX < platform.GetX() + platform.halfWidth/2)
                    {
                        dy = Math.Min(dy, platform.GetPlatformY() - GetY() - halfSizeY - 0.001f);
                        //this.SetX(GetX() + platform.previousDX); //TODO collision with player and wall
                        if(Program.bore.isMoving) this.MoveX(CollisionCheckRight(platform.previousDX, map));
                    }
                }
            }


            return dy;
        }

        public float CollisionCheckUp(float dy, Map map)
        {
            Tile NE = map.TileAtCoords(GetX() + halfSizeX, GetY() + dy - halfSizeY);
            Tile NW = map.TileAtCoords(GetX() - halfSizeX, GetY() + dy - halfSizeY);

            if (NE == null || NW == null)
                return 0f;


            if (!NE.passable)
            {
                dy = Math.Max(dy, NE.sprite.Position.Y + map.tileSize / 2 - GetY() + halfSizeY + 0.001f);
            }
            if (!NW.passable)
            {
                dy = Math.Max(dy, NW.sprite.Position.Y + map.tileSize / 2 - GetY() + halfSizeY + 0.001f);
            }

            return dy;
        }

        public float CollisionCheckRight(float dx, Map map)
        {
            Tile NE = map.TileAtCoords(GetX() + dx + halfSizeX, GetY() + halfSizeY);
            Tile SE = map.TileAtCoords(GetX() + dx + halfSizeX, GetY() - halfSizeY);

            if (NE == null || SE == null)
                return 0f;

            if (!NE.passable)
            {
                dx = Math.Min(dx, NE.sprite.Position.X - map.tileSize / 2 - GetX() - halfSizeX - 0.001f);
            }
            if (!SE.passable)
            {
                dx = Math.Min(dx, SE.sprite.Position.X - map.tileSize / 2 - GetX() - halfSizeX - 0.001f);
            }

            return dx;
        }


        public float CollisionCheckLeft(float dx, Map map)
        {
            Tile NW = map.TileAtCoords(GetX() + dx - halfSizeX, GetY() + halfSizeY);
            Tile SW = map.TileAtCoords(GetX() + dx - halfSizeX, GetY() - halfSizeY);

            if (NW == null || SW == null)
                return 0f;


            if (!NW.passable)
            {
                dx = Math.Max(dx, NW.sprite.Position.X + map.tileSize / 2 - GetX() + halfSizeX + 0.001f);
            }
            if (!SW.passable)
            {
                dx = Math.Max(dx, SW.sprite.Position.X + map.tileSize / 2 - GetX() + halfSizeX + 0.001f);
            }

            return dx;
        }


        //public bool CollisionCheckDown(float dy, Map map)
        //{
        //    if (!map.TileAtCoords(GetX() + halfSizeX, GetY() + dy + halfSizeY).passable || !map.TileAtCoords(GetX() - halfSizeX, GetY() + dy + halfSizeY).passable)
        //        return false;

        //    foreach (Platform platform in map.platforms)
        //    {
        //        if (GetY() + halfSizeY < platform.GetPlatformY() && GetY() + halfSizeY + dy > platform.GetPlatformY() && GetX() + halfSizeX > platform.GetX() - platform.halfWidth && GetX() - halfSizeX < platform.GetX() + platform.halfWidth)
        //        {
        //            this.SetX(GetX() + platform.previousDX); //TODO collision with player and wall
        //            return false;
        //        }
        //    }

        //    return true;

        //    //return map.TileAtCoords(GetX() + halfSizeX, GetY() + dy + halfSizeY).passable && map.TileAtCoords(GetX() - halfSizeX, GetY() + dy + halfSizeY).passable
        //}

        //public bool CollisionCheckUp(float dy, Map map)
        //{
        //    return map.TileAtCoords(GetX() + halfSizeX, GetY() + dy - halfSizeY).passable && map.TileAtCoords(GetX() - halfSizeX, GetY() + dy - halfSizeY).passable;
        //}

        //public bool CollisionCheckLeft(float dx, Map map)
        //{
        //    if (!map.TileAtCoords(GetX() + dx - halfSizeX, GetY() + halfSizeY).passable || !map.TileAtCoords(GetX() + dx - halfSizeX, GetY() - halfSizeY).passable)
        //        return false;

        //    foreach(Cart cart in map.carts)
        //    {
        //        if (GetY() + halfSizeY > cart.GetPlatformY() && GetY() - halfSizeY < cart.GetPlatformY() && GetX() - halfSizeX > cart.GetX() + cart.halfWidth && GetX() - halfSizeX + dx < cart.GetX() + cart.halfWidth)
        //        {
        //            this.SetX(GetX() + cart.previousDX); //TODO: allow climbing with cart while its moving
        //            return false;
        //        }
        //    }

        //    return true;

        //    //return map.TileAtCoords(GetX() + dx - halfSizeX, GetY() + halfSizeY).passable && map.TileAtCoords(GetX() + dx - halfSizeX, GetY() - halfSizeY).passable;
        //}

        //public bool CollisionCheckRight(float dx, Map map)
        //{
        //    if (!map.TileAtCoords(GetX() + dx + halfSizeX, GetY() + halfSizeY).passable || !map.TileAtCoords(GetX() + dx + halfSizeX, GetY() - halfSizeY).passable)
        //        return false;

        //    foreach(Cart cart in map.carts)
        //    {
        //        if (GetY() + halfSizeY > cart.GetPlatformY() && GetY() - halfSizeY < cart.GetPlatformY() && GetX() + halfSizeX < cart.GetX() - cart.halfWidth && GetX() + halfSizeX + dx > cart.GetX() - cart.halfWidth)
        //        {
        //            this.SetX(GetX() + cart.previousDX); //TODO: allow climbing with cart while its moving
        //            return false;
        //        }
        //    }


        //    return true;

        //    //return map.TileAtCoords(GetX() + dx + halfSizeX, GetY() + halfSizeY).passable && map.TileAtCoords(GetX() + dx + halfSizeX, GetY() - halfSizeY).passable;
        //}

        public bool OnLadder(Map map)
        {
            foreach(Ladder ladder in map.ladders){
                //float xDist = Math.Abs(ladder.GetX() - GetX());
                //float yDist = Math.Abs(ladder.GetY() - map.tileSize / 2 - GetY());
                if (Math.Abs(ladder.GetX() - GetX()) < halfSizeX && Math.Abs(ladder.GetY() - map.tileSize / 2 - GetY()) < halfSizeY + map.tileSize)
                {
                    float snapSpeed = 0.25f;                                //TODO improve this to be dependend on dt and make it actually snap to the center
                    this.MoveX(CollisionCheckRight(ladder.previousDX, map));
                    if(ladder.GetX() - GetX() > -snapSpeed)
                        this.MoveX(CollisionCheckRight(snapSpeed, map));
                    else if (ladder.GetX() - GetX() < snapSpeed)
                        this.MoveX(-snapSpeed);
                    return true;
                }
            }

            return false;
        }

        //public Ladder FindLadder(Map map) //TODO old
        //{
        //    foreach(Ladder ladder in map.ladders)
        //    {
        //        if(Math.Abs(ladder.GetX() - GetX()) < 20f && Math.Abs(ladder.GetY() - GetY()) < 50f)
        //        return ladder;
        //    }

        //    return null;
        //}

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
