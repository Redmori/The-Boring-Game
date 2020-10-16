using System;
using System.Collections.Generic;
using System.Text;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SFMLTest2
{
    public class GameObject
    {
        private float x;
        private float y;
        private Sprite sprite;
        public GameObject(float xPos, float yPos)
        {
            x = xPos;
            y = yPos;
            UpdateSprite();
        }

        public float GetX()
        {
            return x;
        }

        public float GetY()
        {
            return y;
        }

        public void SetX(float newX)
        {
            x = newX;
            UpdateSprite();
        }

        public void SetY(float newY)
        {
            y = newY;
            UpdateSprite();
        }

        public void SetPosition(float newX, float newY)
        {
            x = newX;
            y = newY;
            UpdateSprite();
        }

        public void MoveX(float dX)
        {
            x += dX;
            UpdateSprite();
        }

        public void MoveY(float dY)
        {
            y += dY;
            UpdateSprite();
        }

        public void Move(float dX, float dY)
        {
            x += dX;
            y += dY;
            UpdateSprite();
        }

        public Sprite GetSprite()
        {
            return sprite;
        }
        public void SetSprite(Sprite newSprite)
        {
            sprite = newSprite;
            UpdateSprite();
        }
        public void UpdateSprite()
        {
            if (sprite != null)
            {
                sprite.Position = new Vector2f(x, y);
            }
        }
    }
}
