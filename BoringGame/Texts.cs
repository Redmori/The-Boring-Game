using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BoringGame
{
    public class UIText
    {
        public Text text;
        public Vector2f position;

        public UIText(Text text, Vector2f position)
        {
            this.text = text;
            this.position = position;
        }

        public void UpdatePosition(Vector2f viewPos)
        {
            text.Position = viewPos + position;
        }
    }
}
