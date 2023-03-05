using Microsoft.VisualBasic;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BoringGame
{
    public enum OrthSlot
    {
        Top,
        Bottom,
        Right,
        Left
    }
    public class Axle : Structure
    {
        public float torque;


        public Axle Top = null;    //0
        public Axle Bottom = null; //1
        public Axle Right = null;  //2
        public Axle Left = null;   //3

        public bool topOpen = false;
        public bool bottomOpen = false;
        public bool leftOpen = false;
        public bool rightOpen = false;

        public Axle(float x, float y) : base(x, y, 0) //TEMP id = 0
        {
            //Incase its not a default axle we do not want to set default axle behaviour
        }

        public Axle(float x, float y, Axle connectingAxle, int side) : base(x, y, 0) //TEMP id = 0
        {
            switch (side)
            {
                case 0:
                    Top = connectingAxle;
                    bottomOpen = true;
                    break;
                case 1:
                    Bottom = connectingAxle;
                    topOpen = true;
                    break;
                case 2:
                    Left = connectingAxle;
                    rightOpen = true;
                    break;
                case 3:
                    Right = connectingAxle;
                    leftOpen = true;
                    break;
            }

            torque = connectingAxle.torque;
        }

        public (OrthSlot, float) FindClosestOpenSlot(Vector2f mousePos)
        {
            OrthSlot closestSlot = OrthSlot.Top;
            float closestDist = float.MaxValue;

            if (topOpen) {
                float dist = (GetX() - mousePos.X) *(GetX() - mousePos.X) + (GetY() - mousePos.Y - Structure.structureSize) * (GetY() - mousePos.Y - Structure.structureSize);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestSlot = OrthSlot.Top;
                }
            }
            if (bottomOpen)
            {
                float dist = (GetX() - mousePos.X) * (GetX() - mousePos.X) + (GetY() - mousePos.Y + Structure.structureSize) * (GetY() - mousePos.Y + Structure.structureSize);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestSlot = OrthSlot.Bottom;
                }
            }
            if (rightOpen)
            {
                float dist = (GetX() - mousePos.X + Structure.structureSize) * (GetX() - mousePos.X + Structure.structureSize) + (GetY() - mousePos.Y) * (GetY() - mousePos.Y);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestSlot = OrthSlot.Right;
                }
            }
            if (leftOpen)
            {
                float dist = (GetX() - mousePos.X - Structure.structureSize) * (GetX() - mousePos.X - Structure.structureSize) + (GetY() - mousePos.Y) * (GetY() - mousePos.Y);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestSlot = OrthSlot.Left;
                }
            }

            return (closestSlot, closestDist);
        }

        public Axle GetNeighbour(OrthSlot direction)
        {
            switch (direction)
            {
                case OrthSlot.Left: return Left;
                case OrthSlot.Right: return Right;
                case OrthSlot.Top: return Top;
                case OrthSlot.Bottom: return Bottom;
                default:
                    break;
            }
            return null;
        }

        public Vector2f GetNeighbourCoordinates(OrthSlot direction)
        {
            switch (direction)
            {
                case OrthSlot.Left: return new Vector2f(GetX() - structureSize, GetY());
                case OrthSlot.Right: return new Vector2f(GetX() + structureSize, GetY());
                case OrthSlot.Top: return new Vector2f(GetX(), GetY() - structureSize); 
                case OrthSlot.Bottom: return new Vector2f(GetX(), GetY() + structureSize);
            }

            return new Vector2f(GetX(),GetY());
        }

        public static new Axle Place(Axle connectingAxle, Map map, OrthSlot side)
        {
            Build.building = false;
            Axle newAxle;
            if (Build.buildingMode == StructureType.Drillhead)
                newAxle = new Drillhead(connectingAxle.GetX() + Structure.structureSize, connectingAxle.GetY(), connectingAxle, 2);
            else if (Build.buildingMode == StructureType.Cog)
            {
                if (connectingAxle is Cog)
                    newAxle = new Cog(connectingAxle.GetX(), connectingAxle.GetY(), connectingAxle, side);   //TEMP TODO open side = TOP & RIGHT hardcoded
                else
                    newAxle = new Cog(connectingAxle.GetX(), connectingAxle.GetY(), connectingAxle, side);   //TEMP TODO open side = TOP & RIGHT hardcoded
            }

            else
                newAxle = new Axle(connectingAxle.GetX() + Structure.structureSize, connectingAxle.GetY(), connectingAxle, 2); //TEMP TODO side = 2 is only right side hard coded for now
            newAxle.torque = connectingAxle.torque;
            newAxle.SetSprite(Build.buildingSprite);
            Build.buildingSprite.Color = new Color(255, 255, 255, 255);
            Build.buildingSprite = null;

            map.axles.Add(newAxle);
            if (!(Build.buildingMode == StructureType.Cog && connectingAxle is Cog))
            {
                Console.WriteLine("Right closed");
                connectingAxle.Right = newAxle;
                connectingAxle.rightOpen = false;
            }

            return newAxle;
        }
    }

    public class Cog : Axle
    {
        public Cog(float x, float y, Axle connectingAxle, OrthSlot side) : base(x, y)
        {
            switch (side)
            {
                case OrthSlot.Top:
                    {
                        topOpen = true;
                        rightOpen = true;
                        leftOpen = true;
                        Bottom = connectingAxle;
                        connectingAxle.Top = this;
                        connectingAxle.topOpen = false;
                        this.SetY(GetY() - structureSize);
                        break;
                    }
                 case OrthSlot.Bottom:
                    {
                        bottomOpen = true;
                        rightOpen = true;
                        leftOpen = true;
                        Top = connectingAxle;
                        connectingAxle.Bottom = this;
                        connectingAxle.bottomOpen = false;
                        this.SetY(GetY() + structureSize);
                        break;
                    }
                    case OrthSlot.Right:
                    {
                        rightOpen = true;
                        topOpen = true;
                        bottomOpen = true;
                        Left = connectingAxle;
                        connectingAxle.Right = this;
                        connectingAxle.rightOpen = false;
                        this.SetX(GetX() + structureSize);
                        break;
                    }
                    case OrthSlot.Left:
                    {
                        leftOpen = true;
                        topOpen = true;
                        bottomOpen = true;
                        Right = connectingAxle;
                        connectingAxle.Left = this;
                        connectingAxle.leftOpen = false;
                        this.SetX(GetX() - structureSize);
                        break;
                    }
            }

            //if(connectingAxle is Cog) //if its a cog we are connecting to
            //{
            //    bottomOpen = true;
            //    rightOpen = true;
            //    Top = connectingAxle;
            //    connectingAxle.Bottom = this;
            //    connectingAxle.bottomOpen = false;
            //}
            //else //if its an axle we are connecting to
            //{
            //    bottomOpen = true;
            //    rightOpen = true;
            //    Left = connectingAxle;
            //    connectingAxle.Right = this;
            //    connectingAxle.rightOpen = false;
            //}
        }
    }


    public class Motor : Axle
    {
        public float torqueCreated;
        public Motor(float x, float y, Axle connectingAxle, int openSide) : base(x, y)
        {
            switch(openSide)
            {
                case 0: topOpen = true; break;
                case 1: bottomOpen = true; break;
                case 2: leftOpen = true; break; 
                case 3: rightOpen = true; break;   
            }
        }
    }

    public class AxledMachine : Axle
    {
        public float torqueConsumed;
        public AxledMachine(float x, float y, Axle connectingAxle, int connectingSide) : base(x, y)
        {
            switch (connectingSide)
            {
                case 0: Top = connectingAxle; break;
                case 1: Bottom = connectingAxle; break;
                case 2: Left = connectingAxle; break;
                case 3: Right = connectingAxle; break;
            }
        }
    }

    public class Drillhead : AxledMachine
    {
        public float hardness;
        public static float drillPower = 0.1f;
        public Drillhead(float x, float y, Axle connectingAxle, int connectingSide) : base(x, y, connectingAxle, connectingSide)
        {
            //drillPower = 0.1f;
        }        
        public override float CollisionCheckRightN(float dx, Map map)
        {

            Tile tile = map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X / 2, GetY());
            if (tile == null)
                return 0;

            if (tile.minable)
            {
                tile.Mine(drillPower, map); //TODO: drilling power needs to be a function of dt
            }

            if (tile.minable && !tile.passable)
            {
                return Math.Max(0f, tile.sprite.Position.X - GetX() - tile.RelativeHealth() * map.tileSize/2 - this.GetSprite().Texture.Size.X / 2);
            }

            if (!tile.passable)
                return Math.Max(0f, tile.sprite.Position.X - GetX() - map.tileSize / 2 - this.GetSprite().Texture.Size.X / 2);

            return dx;
        }
    }
}
