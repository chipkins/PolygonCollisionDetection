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
    /// <summary>
    /// Struct that holds positional data for an objcect
    /// </summary>
    public struct Transform
    {
        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Transform(Vector2 p)
        {
            position = p;
        }

        public static Transform operator *(Transform t, float r)
        {
            return new Transform(t.position * r);
        }

        public static Transform operator +(Transform t1, Transform t2)
        {
            return new Transform(t1.position + t2.position);
        }
    }
}
