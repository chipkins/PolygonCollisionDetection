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
    /// Holds the data pertaining to the objects specific material
    /// </summary>
    public struct Material
    {
        private float density;
        private float restitution; // "Bounciness" of the material

        public float Density
        {
            get { return density; }
            set { density = value; }
        }

        public float Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }

        public Material(float density, float restitution)
        {
            this.density = density;
            this.restitution = restitution;
        }
    }
}
