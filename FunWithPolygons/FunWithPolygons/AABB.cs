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
    /// Class is a Bounding Box that contains an object's minimum and maximum points
    /// This allows a simple check for the object collision for the BroadPhase to determine if 2 objects could be colliding
    /// </summary>
    public class AABB
    {
        private Vector2 min;
        private Vector2 max;
        private Vector2 center;
        private Vector2 extent;

        public AABB(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
            extent = min - max;
        }

        public Vector2 Center
        {
            get { return center; }
        }

        public Vector2 Min
        {
            get { return min; }
        }

        public Vector2 Max
        {
            get { return max; }
        }

        /// <summary>
        /// Recalculates the bounding box for a polygon, which is neccissary because the min and max change during rotation
        /// This process is not neccissary for circles as they are not affected by rotation
        /// </summary>
        /// <param name="pP"></param>
        public void CalculateAABB(PerfectPolygon pP)
        {
            Vector2 minimum = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 maximum = new Vector2(float.MinValue, float.MinValue);
            //Loops through all points and finds the min X and Y as well as the max for all the points
            for (int i = 0; i < pP.points.Length; i++)
            {
                minimum = new Vector2(Math.Min(minimum.X, pP.points[i].X), Math.Min(minimum.Y, pP.points[i].Y));
                maximum = new Vector2(Math.Max(maximum.X, pP.points[i].X), Math.Max(maximum.Y, pP.points[i].Y));
            }

            //set min and max equal to the minimum point and the maximum point
            min = minimum;
            max = maximum;
            extent = max - min;
            center = min + (extent / 2);
        }
    }
}
