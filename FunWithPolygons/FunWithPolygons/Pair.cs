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
    /// Holds 2 objects so that they can be tested by the collision handler
    /// Also holds variables from the calculation of the collision so they may be used to resolve it
    /// </summary>
    public struct Pair
    {
        private Body a;
        private Body b;
        //Amount object A penetrates object B
        private static float penetration;
        //Vector normal of the collision
        private static Vector2 normal;
        //Array of the points of contact between object A and object B
        private static Vector2[] contacts;
        private static int contactCount;
        

        public Body A
        {
            get { return a; }
        }

        public Body B
        {
            get { return b; }
        }

        public static float Penetration
        {
            get { return penetration; }
            set { penetration = value; }
        }

        public static Vector2 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public static int ContactCount
        {
            get { return contactCount; }
            set { contactCount = value; }
        }

        public static Vector2[] Contacts
        {
            get { return contacts; }
            set { contacts = value; }
        }

        public Pair(Body a, Body b)
        {
            this.a = a;
            this.b = b;
            penetration = 0.0f;
            normal = Vector2.Zero;
            contactCount = 0;
            contacts = new Vector2[2];
        }
    }
}
