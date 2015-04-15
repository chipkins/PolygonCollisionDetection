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
    public class PerfectPolygon : Shape
    {
        //Array of all the poins on the polygon
        public Vector2[] points;
        //Array of the vector from the center to a given point
        public Vector2[] vectorToPoints;
        //Array of vectors that represent the faces of the polygon
        public Vector2[] faces;
        //Array of vectors that represent the direction of the normal of the face
        public Vector2[] faceNormals;
        public RotationMatrix rotMat;

        /// <summary>
        /// Accessor that recalculates the center of the polygon as well as all of the points
        /// </summary>
        new public Vector2 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = base.Position - vectorToPoints[i];
                }
            }
        }

        public PerfectPolygon(int sides, float x, float y, Texture2D texture) :
            base(x, y, texture)
        {
            points = new Vector2[sides];
            vectorToPoints = new Vector2[sides];
            faces = new Vector2[sides];
            faceNormals = new Vector2[sides];

            //radius of the polygon, from the center to point 0 for an odd sided polygon, or from center to face 0 for even sided one
            float radius = texture.Height / 2;
            float pi = (float)Math.PI;
            //Inner angle of the polygon
            float theta = (2 * pi) / sides;
            //Creates a rotation matrix that rotates a vector by theta
            rotMat = new RotationMatrix(-theta);

            //Calculates the points of a perfect polygon with an odd number of sides
            if ((sides % 2) == 1)
            {
                vectorToPoints[0] = new Vector2(0, radius);
                points[0] = base.Position - vectorToPoints[0];
                for (int i = 1; i < sides; i++)
                {
                    //Rotates the vecor of the previous point by theta to get the new point
                    vectorToPoints[i] = rotMat * vectorToPoints[i - 1];
                    points[i] = base.Position - vectorToPoints[i];
                }

                //Calculates the face and face normal of all sides
                for (int i = 0; i < sides; i++)
                {
                    //face normal is the vector between face[i] and face[i - 1] or face[0] and face[#sides - 1]
                    faces[i] = points[i + 1 < sides ? i + 1 : 0] - points[i];
                    //Calculates the the normal of the face by finding the orthogonal vector
                    faceNormals[i] = new Vector2(-faces[i].Y, faces[i].X);
                    //Normalizes so that it is a directional vector
                    faceNormals[i].Normalize();
                }
            }

            //Calculates the points of a perfect polygon with an even number of sides
            else
            {
                //float side = 2 * (radius * (float)Math.Tan(theta / 2));

                //Finds vector to the first point using tan(theta) = opp/adj -> adj*tan(theta) = opp for the X component
                vectorToPoints[0] = new Vector2(radius * (float)Math.Tan(theta / 2), radius);
                points[0] = base.Position - vectorToPoints[0];
                for (int i = 1; i < sides; i++)
                {
                    vectorToPoints[i] = rotMat * vectorToPoints[i - 1];
                    points[i] = base.Position - vectorToPoints[i];
                }

                for (int i = 0; i < sides; i++)
                {
                    faces[i] = points[i + 1 < sides ? i + 1 : 0] - points[i];
                    faceNormals[i] = new Vector2(-faces[i].Y, faces[i].X);
                    faceNormals[i].Normalize();
                }
            }
        }

        /// <summary>
        /// Finds the point farthest along a given direction
        /// </summary>
        /// <param name="dir">normalized vector representing a given direction</param>
        /// <returns></returns>
        public Vector2 GetSupport(Vector2 dir)
        {
            float bestProjection = -float.MaxValue;
            Vector2 bestVertex = Vector2.Zero;

            //for each point find how far along the direction it is using the dot product of the point and the directional vector
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 v = points[i];
                float projection = Vector2.Dot(v, dir);

                if (projection > bestProjection)
                {
                    bestVertex = v;
                    bestProjection = projection;
                }
            }

            return bestVertex;
        }

        public void Update()
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = base.Position - vectorToPoints[i];
            }
        }

        //Draws the points of the polygon (debug)
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            for (int i = 0; i < points.Length; i++)
            {
                spriteBatch.Draw(texture, points[i], Color.White);
            }
        }
    }
}
