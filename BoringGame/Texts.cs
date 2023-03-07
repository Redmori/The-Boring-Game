using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BoringGame
{
    public static class TextManager
    {
        public static List<Text> texts = new List<Text>();
        public static Font arial = new Font("../../../Content/ArialCEMTBlack.ttf");

        //public static List<RectangleShape> shapes = new List<RectangleShape>(); //TEMP for progress bar

        public static void Draw(Window window)
        {
            foreach (Text text in texts)
            {
                text.Draw((RenderTarget)window, RenderStates.Default);
            }
        }

        public static Text AddText(string strng, Vector2f pos)
        {
            Text text = new Text(strng, arial, 12);
            text.Origin = new Vector2f(text.GetLocalBounds().Width * 0.5f, text.GetLocalBounds().Height * 0.5f);
            text.Position = pos;
            texts.Add(text);
            return text;
        }

        //public static void AddProgressBar(RectangleShape shape)
        //{
        //    shapes.Add(shape);
        //}
    }



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
