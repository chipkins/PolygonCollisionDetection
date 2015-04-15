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
    /// Struct that holds all of the data pertaining to an objects mass
    /// </summary>
    public struct MassData
    {
        public float mass;
        public float invMass;

        /// <summary>
        /// Calculates the objects mass based on it's area and the provided density
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="density"></param>
        /// <param name="sides"></param>
        public MassData(Shape shape, float density, int sides = 0)
        {
            float area = 0.0f;

            PerfectPolygon poly = (shape as PerfectPolygon);
            float sideLength = (float)Math.Sqrt(poly.faces[0].X * poly.faces[0].X + poly.faces[0].Y * poly.faces[0].Y);
            area = (float)(0.25f * sides * ((sideLength * 0.1f) * (sideLength * 0.1f)) * (1 / Math.Tan(Math.PI / sides)));

            mass = density * area;
            invMass = mass > 0 ? 1 / mass : 0;
        }
    }
}
