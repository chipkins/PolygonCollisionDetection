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
    /// Generates Pairs by running checks to see if objects in the Scene have the possibility of colliding
    /// </summary>
    public class BroadPhase
    {
        List<Pair> pairs;

        public BroadPhase()
        {
            pairs = new List<Pair>();
        }

        public List<Pair> Pairs
        {
            get { return pairs; }
        }

        /// <summary>
        /// Checks the box formed around the object by its min and max bounds is colliding with the bounding box around another object
        /// If returns true then the objects could be colliding but aren't necissarily
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public bool AABBvsAABB(AABB A, AABB B)
        {
            Vector2 vAB = B.Center - A.Center;

            float aExtentX = (A.Max.X - A.Min.X) / 2;
            float bExtentX = (B.Max.X - B.Min.X) / 2;

            float xOverlap = aExtentX + bExtentX - Math.Abs(vAB.X);

            if (xOverlap > 0)
            {
                float aExtentY = (A.Max.Y - A.Min.Y) / 2;
                float bExtentY = (B.Max.Y - B.Min.Y) / 2;

                float yOverlap = aExtentY + bExtentY - Math.Abs(vAB.Y);

                if (yOverlap > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generates Pairs by checking the bounding boxes of 2 objects against one another
        /// </summary>
        /// <param name="bodies"></param>
        public void GeneratePairs(List<Body> bodies)
        {
            //Clears previous list of Pairs
            pairs.Clear();

            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = 0; j < bodies.Count; j++)
                {
                    Body A = bodies[i];
                    Body B = bodies[j];
                    //If A and B are the same object then skip this instence
                    if (A.Equals(B))
                        continue;

                    if (AABBvsAABB(A.AABB, B.AABB))
                    {
                        pairs.Add(new Pair(A, B));
                    }
                }
            }
        }
    }
}
