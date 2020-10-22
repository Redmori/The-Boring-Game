﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BoringGame
{
    public enum StructureType
    {
        Cart,
        Platform,
        Ladder,
        Furnace,

        Count
    }

    public class Structure : GameObject
    {
        public static float structureSize = 50f;

        public float weight;

        public Structure(float x, float y) : base(x, y)
        {
            weight = 1000;
        }

        public void Build()
        {

        }

        public bool CollisionCheckRight(float dx, Map map)
        {
            if (!map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X/2, GetY() + this.GetSprite().Texture.Size.Y/2).passable || !map.TileAtCoords(GetX() + dx + this.GetSprite().Texture.Size.X/2, GetY() - this.GetSprite().Texture.Size.Y/2).passable)
                return false;

            return true;
        }

    }
}