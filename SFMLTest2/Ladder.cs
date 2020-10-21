using System;
using System.Collections.Generic;
using System.Text;

namespace SFMLTest2
{
    public class Ladder : Structure
    {
        public Ladder(float x, float y) : base(x,y)
        {
            SetSprite(SpriteManager.GetStructureSprite(StructureType.Ladder, x, y));
            UpdateSprite();
        }

    }
}
