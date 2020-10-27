using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
{
    public class Ladder : Structure
    {
        public float previousDX;
        public Ladder(float x, float y) : base(x,y)
        {
        }


        //public new void MoveX(float dx)
        //{
        //    previousDX = dx;
        //    Console.WriteLine("moving");
        //    base.MoveX(dx);
        //}
    }
}
