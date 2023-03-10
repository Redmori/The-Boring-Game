using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BoringGame
{
    public static class TextManager
    {
        public static List<Text> texts = new List<Text>();
        public static List<VertexArray> shapes = new List<VertexArray>();
        public static Font arial = new Font("../../../Content/ArialCEMTBlack.ttf");

        //public static List<RectangleShape> shapes = new List<RectangleShape>(); //TEMP for progress bar

        public static void Draw(Window window)
        {
            foreach (Text text in texts)
            {
                text.Draw((RenderTarget)window, RenderStates.Default);
            }
            foreach (VertexArray shape in shapes)
                shape.Draw((RenderTarget)window, RenderStates.Default);
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

        public static VertexArray GetCenteredRectangle(Vector2f halfSize, Vector2f pos, Color colour)
        {

            VertexArray rect = new VertexArray(PrimitiveType.Quads, 4);
            shapes.Add(rect);
            return rect;
        }
        public static RectangleShape GetCenteredRectangle2(Vector2f size, Vector2f pos, Color colour) //TEMP is the same as previous method, just doesnt add to the shapes list
        {
            RectangleShape rect = new RectangleShape(size);
            rect.Origin = size * 0.5f;
            rect.FillColor = colour;
            return rect;
        }

        public static RectangleShape UpdateCenteredRectangle(RectangleShape rect, Vector2f size, Vector2f pos, Color colour)
        {
            if (rect == null) return null;
            rect.Position = pos;
            rect.Size = size;
            rect.FillColor= colour;
            return rect;
        }

        public static VertexArray GetRectangle(Vector2f size, Vector2f pos, Color colour)
        {

            VertexArray rect = new VertexArray(PrimitiveType.Quads, 4);
            shapes.Add(rect);
            return rect;
        }

        public static VertexArray UpdateRectangle(VertexArray rect, Vector2f size, Vector2f pos, Color colour)
        {
            if (rect == null) return null;
            rect[0] = new Vertex(new Vector2f(pos.X, pos.Y), colour);
            rect[1] = new Vertex(new Vector2f(pos.X + size.X, pos.Y), colour);
            rect[2] = new Vertex(new Vector2f(pos.X + size.X, pos.Y + size.Y), colour);
            rect[3] = new Vertex(new Vector2f(pos.X, pos.Y + size.Y), colour);
            return rect;
        }
        public static void AddRectangle(VertexArray rect)
        {
            shapes.Add(rect);
        }
        public static void RemoveRectangle(VertexArray rect)
        {
            shapes.Remove(rect);
        }
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
