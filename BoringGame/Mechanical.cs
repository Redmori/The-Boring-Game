using Microsoft.VisualBasic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Axle(float x, float y, Axle connectingAxle, OrthSlot side) : base(x, y, 0) //TEMP id = 0
        {
            switch (side)
            {
                case OrthSlot.Right:
                    Left = connectingAxle;
                    rightOpen = true;
                    connectingAxle.Right = this;
                    connectingAxle.rightOpen = false;
                    this.SetX(GetX() + Structure.structureSize);
                    break;
                case OrthSlot.Left:
                    Right = connectingAxle;
                    leftOpen = true;
                    connectingAxle.Left = this;
                    connectingAxle.leftOpen = false;
                    this.SetX(GetX() - Structure.structureSize);
                    break;
            }

            torque = connectingAxle.torque;
        }

        public (OrthSlot, float) FindClosestOpenSlot(Vector2f mousePos, bool[] angles)
        {
            OrthSlot closestSlot = OrthSlot.Top;
            float closestDist = float.MaxValue;

            if (topOpen && angles[0]) {
                float dist = (GetX() - mousePos.X) *(GetX() - mousePos.X) + (GetY() - mousePos.Y - Structure.structureSize) * (GetY() - mousePos.Y - Structure.structureSize);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestSlot = OrthSlot.Top;
                }
            }
            if (bottomOpen && angles[1])
            {
                float dist = (GetX() - mousePos.X) * (GetX() - mousePos.X) + (GetY() - mousePos.Y + Structure.structureSize) * (GetY() - mousePos.Y + Structure.structureSize);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestSlot = OrthSlot.Bottom;
                }
            }
            if (rightOpen && angles[2])
            {
                float dist = (GetX() - mousePos.X + Structure.structureSize) * (GetX() - mousePos.X + Structure.structureSize) + (GetY() - mousePos.Y) * (GetY() - mousePos.Y);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestSlot = OrthSlot.Right;
                }
            }
            if (leftOpen && angles[3])
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

        public static bool[] GetConnection()
        {
            return new bool[] { false, false, true, true };
        }

        public static new GameObject UpdateBuilding(Vector2f mousePos, Map map, Bore bore, int id)
        {
            bool[] angles = (bool[])Build.GetMethodInfo(id, "GetConnection").Invoke(null, new object[] { });

            Axle closestAxle = null;
            float closestD = float.MaxValue;
            OrthSlot closestSide = OrthSlot.Top;
            foreach (Axle nearAxle in map.axles)
            {
                (OrthSlot slot, float dist) = nearAxle.FindClosestOpenSlot(mousePos,angles);
                if (dist < closestD)
                {
                    closestD = dist;
                    closestAxle = nearAxle;
                    closestSide = slot;
                }
            }
            if (closestAxle != null && closestD != float.MaxValue)
            {
                SpriteManager.UpdateBuildingPos(closestAxle.GetNeighbourCoordinates(closestSide));
                // if (buildingMode == StructureType.Cog && closestAxle is Cog)
                //     buildingSprite.Position = new Vector2f(closestAxle.GetX(), closestAxle.GetY() + Structure.structureSize);

                //if mouse is clicked, place the actual structure
                if (Program.mousePressed)
                {
                    //TODO generalise this to work for all axle types properly, copy what was done for Cogs
                    var args = new Object[] { closestAxle, map, closestSide };
                    Axle newAxle = (Axle)Build.GetMethodInfo(id,"Place").Invoke(null,args);
                    bore.structures.Add(newAxle);
                    return newAxle;
                }

            }

            return null;
        }


        public static new Axle Place(Axle connectingAxle, Map map, OrthSlot side)
        {
            Build.building = false;
            Axle newAxle = new Axle(connectingAxle.GetX(), connectingAxle.GetY(), connectingAxle, side);

            newAxle.torque = connectingAxle.torque;

            SpriteManager.Build(newAxle);

            map.axles.Add(newAxle);

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
                        this.SetY(GetY() - Structure.structureSize);
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
                        this.SetY(GetY() + Structure.structureSize);
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
                        this.SetX(GetX() + Structure.structureSize);
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
                        this.SetX(GetX() - Structure.structureSize);
                        break;
                    }
            }
        }

        public static new Axle Place(Axle connectingAxle, Map map, OrthSlot side)
        {
            Build.building = false;
            Axle newAxle;
            
            newAxle = new Cog(connectingAxle.GetX(), connectingAxle.GetY(), connectingAxle, side);

            newAxle.torque = connectingAxle.torque;

            SpriteManager.Build(newAxle);

            map.axles.Add(newAxle);

            //if (!(connectingAxle is Cog))
            //{
            //    connectingAxle.Right = newAxle;
            //    connectingAxle.rightOpen = false;
            //}

            return newAxle;
        }

        public static new bool[] GetConnection()
        {
            return new bool[] { true, true, true, true };
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

        public static new GameObject UpdateBuilding(Vector2f mousePos, Map map, Bore bore, int id)
        {
            GameObject newMotor = Structure.UpdateBuilding(mousePos, map, bore, id);
            if(newMotor != null)
                map.axles.Add((Axle)newMotor);
            return newMotor;
        }

        public static new GameObject Place(Vector2i indexLoc, Bore br, int id)
        {
            return Structure.Place(indexLoc, br, id);
        }
}

    public class AxledMachine : Axle
    {
        public float torqueConsumed;

        public AxledMachine(float x, float y) : base(x, y)
        {
        }

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

        public Contents content; //TEMP contents
        public Text tooltip;
        public Drillhead(float x, float y, Axle connectingAxle, OrthSlot side) : base(x, y)
        {
            //drillPower = 0.1f;

            Left = connectingAxle;
            connectingAxle.Right = this;
            connectingAxle.rightOpen = false;
            this.SetX(GetX() + Structure.structureSize);

            content = new Contents(); //TEMP contents
            tooltip = TextManager.AddText("", new Vector2f(0, 0),Color.Blue);
            Program.window.MouseButtonPressed += window_MouseButtonPressed;

        }

        public override void Update(float dt)
        {
            tooltip.DisplayedString = content.ToString();
            tooltip.Position = new Vector2f(this.GetX()- 0.3f*  Structure.structureSize, this.GetY() - 10f);
            base.Update(dt);
        }
        public override float CollisionCheckRightN(float dx, Map map)
        {

            Tile tile = map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X / 2, GetY());
            if (tile == null)
                return 0;

            if (tile.minable)
            {
                content.Add(tile.Mine(drillPower, map)); //TODO: drilling power needs to be a function of dt
            }

            if (tile.minable && !tile.passable)
            {
                return Math.Max(0f, tile.sprite.Position.X - GetX() - tile.RelativeHealth() * map.tileSize/2 - this.GetSprite().Texture.Size.X / 2);
            }

            if (!tile.passable)
                return Math.Max(0f, tile.sprite.Position.X - GetX() - map.tileSize / 2 - this.GetSprite().Texture.Size.X / 2);

            return dx;
        }

        private void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            Vector2f mousePos = Program.GetMouse();
            if (GetSprite().GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
            {
                Program.player.inventory.ReceiveItem(content.items);
                content.items.Clear();
            }


        }
        public static new Axle Place(Axle connectingAxle, Map map, OrthSlot side)
        {
            Build.building = false;
            Axle newAxle;
            newAxle = new Drillhead(connectingAxle.GetX(), connectingAxle.GetY(), connectingAxle, OrthSlot.Right);

            newAxle.torque = connectingAxle.torque;

            SpriteManager.Build(newAxle);

            map.axles.Add(newAxle);

            return newAxle;
        }

        public static bool[] GetConnection()
        {
            return new bool[] { false, false, true, false };
        }
    }
}
