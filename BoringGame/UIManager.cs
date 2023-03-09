using BoringGame;
using Microsoft.VisualBasic;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static SFML.Window.Mouse;
using System.Xml.Linq;
using SFML.Window;

namespace BoringGame
{
    public static class UIManager
    {

        public static List<Popup> popups = new List<Popup>();


        //public static void Click(Vector2f mousePos)
        //{
        //    foreach (Popup popup in popups)
        //        popup.Click(mousePos);
        //}

        public static void Update(RenderWindow window)
        {
            foreach (Popup popup in popups)
                popup.Update(window);
        }

        public static void ActivatePopup(Popup pop)
        {
            popups.Add(pop);
            pop.Activate();
        }

        public static void DeactivatePopup(Popup pop)
        {   
            popups.Remove(pop);
            pop.Dectivate();
        }

        public static void TogglePopup(Popup pop)
        {
            if (popups.Contains(pop))
                DeactivatePopup(pop);
            else
                ActivatePopup(pop);
        }
    }


    public class Popup
    {
        public List<IScreenElement> elements;
        public Vector2f offset;

        public Popup()
        {
            elements = new List<IScreenElement>();
            offset = new Vector2f(0, 0);
        }

        public Popup(Vector2f off)
        {
            elements = new List<IScreenElement>();
            offset = off;
        }

        public bool CheckClick(Vector2f mousePos)
        {
            foreach (IScreenElement element in elements)
                if (element is ScreenButton && ((ScreenButton)element).active)
                    return ((ScreenButton)element).Contains(mousePos);
            return false;
        }

        public void Activate()
        {
            foreach (IScreenElement element in elements)
            {
                if(element is ScreenButton)
                {
                    ((ScreenButton)element).active = true;
                }
            }
        }
        public void Dectivate()
        {
            foreach (IScreenElement element in elements)
            {
                if (element is ScreenButton)
                {
                    ((ScreenButton)element).active = false;
                }
            }
        }
        public void Update(RenderWindow window)
        {
            foreach (IScreenElement element in elements)
            {
                element.UpdatePosition(window.GetView(), offset);
                element.DrawElement(window);
            }
        }

        //public void Click(Vector2f mousePos)
        //{
        //    foreach (IScreenElement element in elements)
        //        if (element is ScreenButton) 
        //            (ScreenButton)element.Click(mousePos);
        //}
    }

    public class ScreenButton : ScreenSquare
    {
        public bool isPressed = false;
        public bool active = false;

        public ScreenButton(Vector2f sz, Vector2f off) : base(sz, off)
        {
            //Program.window.MouseButtonPressed += window_MouseButtonPressed;
        }

        public bool IsPressed()
        {
            return isPressed;
        }
        //private void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        //{
        //    if (active && e.Button == Mouse.Button.Left)
        //    {
        //        Vector2f mousePos = new Vector2f(Mouse.GetPosition(Program.window).X, Mouse.GetPosition(Program.window).Y);

        //        if (Contains(mousePos))
        //        {
        //            isPressed = true;
        //            Console.WriteLine("pressing button");
        //        }
        //    }
        //}
        //OLD
        //public void HandleEvents(RenderWindow window, Event e)
        //{
        //    Console.WriteLine("handling");
        //    if (e.MouseButton.Button == Mouse.Button.Left)
        //    {
        //        Vector2f mousePos = new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y);

        //        Console.WriteLine("checking");
        //        if (Contains(mousePos))
        //        {
        //            isPressed = true;
        //            Console.WriteLine("pressing button");
        //        }
        //    }
        //}
    }


    public class ScreenSquare : IScreenElement
    {
        public Vector2f size;
        public Vector2f offset;
        public VertexArray rectangle;

        public ScreenSquare(Vector2f sz, Vector2f off)
        {
            offset = off;

            size = sz;

            rectangle = TextManager.GetCenteredRectangle2(size, off, SFML.Graphics.Color.Cyan);
        }

        public void UpdatePosition(View view, Vector2f off)
        {
            TextManager.UpdateCenteredRectangle(rectangle, size, view.Center + offset + off, SFML.Graphics.Color.Cyan);
        }

        public void DrawElement(RenderWindow window)
        {
            window.Draw(rectangle);
        }

        public bool Contains(Vector2f point)
        {
            Vector2f differenceBR = point - (Vector2f)Program.window.Size *0.5f - this.size - this.offset;
            Vector2f differenceTL = point - (Vector2f)Program.window.Size * 0.5f + this.size - this.offset;
            if (differenceBR.X < 0 && differenceBR.Y < 0 && differenceTL.X > 0 && differenceTL.Y > 0)
                return true;
            return false;
        }
    }

    public class ScreenText : IScreenElement
    {
        public Vector2f offset;
        public Text text;

        public ScreenText(string strng, Vector2f off)
        {
            text = new Text(strng, TextManager.arial, 12);
            text.Origin = new Vector2f(text.GetLocalBounds().Width * 0.5f, text.GetLocalBounds().Height * 0.5f);

            offset = off;

        }

        public ScreenText(Text txt, Vector2f off)
        {
            text = txt;
            offset = off;
        }

        public void UpdatePosition(View view, Vector2f off)
        {
            text.Position = view.Center + offset + off;
        }

        public void DrawElement(RenderWindow window)
        {
            window.Draw(text);
        }
    }

    public interface IScreenElement
    {
        void UpdatePosition(View view, Vector2f off);
        void DrawElement(RenderWindow window);
    }
}

