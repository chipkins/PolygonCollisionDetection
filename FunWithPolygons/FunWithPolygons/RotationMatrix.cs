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
    /// Matrix that rotates a vector by a certain number of radians
    /// </summary>
    public struct RotationMatrix
    {
        private float m00;
        private float m01;
        private float m10;
        private float m11;

        public RotationMatrix(Vector2 x, Vector2 y)
        {
            m00 = x.X;
            m01 = x.Y;
            m10 = y.X;
            m11 = y.Y;
        }

        public RotationMatrix(float radians)
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            m00 = cos;
            m01 = -sin;
            m10 = sin;
            m11 = cos;
        }

        public static Vector2 operator *(RotationMatrix m, Vector2 v)
        {
            return new Vector2(m.m00 * v.X + m.m01 * v.Y, m.m10 * v.X + m.m11 * v.Y);
        }

        public void SetAngle(float radians)
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            m00 = cos;
            m01 = -sin;
            m10 = sin;
            m11 = cos;
        }
    }
}
