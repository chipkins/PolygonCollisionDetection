using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace FunWithPolygons
{
    public class Shape
    {
        private Vector2 center;
        private Transform previous;
        private Transform current;

        public float Cross(Vector2 a, Vector2 b)
        {
            return (a.X * b.Y) - (a.Y * b.X);
        }

        public Shape(float x, float y, Texture2D texture)
        {
            current = new Transform(new Vector2(x, y));
            center = new Vector2(x + (texture.Width / 2), y + (texture.Height / 2));
        }

        public Transform Previous
        {
            get { return previous; }
            set { previous = value; }
        }

        public Transform Current
        {
            get { return current; }
            set { current = value; }
        }

        public Vector2 Position
        {
            get { return current.Position; }
            set { current.Position = value; }
        }

        public Vector2 Center
        {
            get { return center; }
            set { center = value; }
        }
    }
}
