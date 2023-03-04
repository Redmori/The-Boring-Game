using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
{
    public class Ladder : Structure
    {
        public float previousDX;
        public Ladder(float x, float y, int id) : base(x,y,id)
        {
        }


        public override void MoveX(float dx)
        {
            previousDX = dx;
            base.MoveX(dx);
        }
    }
}
