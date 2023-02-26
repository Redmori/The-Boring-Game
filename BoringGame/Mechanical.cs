using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
{
    public class Axle : Structure
    {
        public float torque;

        public Axle Top = null;    //1
        public Axle Bottom = null; //2
        public Axle Left = null;   //3
        public Axle Right = null;  //4

        public bool topOpen = false;
        public bool bottomOpen = false;
        public bool leftOpen = false;
        public bool rightOpen = false;


        public Axle(float x, float y, Axle connectingAxle, int side ) : base(x, y)
        {
            if (side != -1)
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
        }
    }

    public class Motor : Axle
    {
        public Motor(float x, float y, Axle connectingAxle, int openSide) : base(x, y, connectingAxle, -1)
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
        public AxledMachine(float x, float y, Axle connectingAxle, int connectingSide) : base(x, y, connectingAxle, -1)
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
}
