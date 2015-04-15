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
    /// Contains all of the collidable objects and "world" variables for collision
    /// </summary>
    struct Scene
    {
        //Time since collision was last checked
        private float dt;
        private float invDT;
        private List<Body> bodyList;
        private Vector2 gravity;

        public Vector2 Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }

        public float DT
        {
            get { return dt; }
            set { dt = value; }
        }

        public Scene(Vector2 gravity, float dt)
        {
            this.gravity = gravity;
            this.dt = dt;
            invDT = dt > 0 ? 1 / dt : 0;
            bodyList = new List<Body>();
        }

        /// <summary>
        /// Adds a body to the scene
        /// </summary>
        /// <param name="body"></param>
        public void InsertBody(Body body)
        {
            bodyList.Add(body);
        }

        /// <summary>
        /// Removes a body from the scene
        /// </summary>
        /// <param name="body"></param>
        public void RemoveBody(Body body)
        {
            bodyList.Remove(body);
        }

        public List<Body> Bodies
        {
            get { return bodyList; }
        }

        /// <summary>
        /// Increments all of the bodies by one step in time
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="dt"></param>
        public void Step(GraphicsDevice graphicsDevice, float dt)
        {
            for (int i = 0; i < bodyList.Count; i++)
            {
                bodyList[i].Velocity += bodyList[i].TotalForce * dt;
                bodyList[i].Position += bodyList[i].Velocity * dt;
                bodyList[i].TotalForce = Vector2.Zero;
                StayOnScreen(bodyList[i], graphicsDevice);
                StayOnScreen(bodyList[i], graphicsDevice);
            }
        }

        /// <summary>
        /// Checks and resolves all Pairs
        /// </summary>
        /// <param name="pairs"></param>
        /// <param name="nPhase"></param>
        public void UpdatePhysics(List<Pair> pairs, NarrowPhase nPhase)
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                if (nPhase.CheckCollision(pairs[i]))
                {
                    nPhase.ResolveCollision(pairs[i]);                    
                    nPhase.PositionalCorrection(pairs[i], 0.4f, 0.1f);
                }
            }
        }

        /// <summary>
        /// Keeps the objects on the screen at all times
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="graphicsDevice"></param>
        public void StayOnScreen(Body obj, GraphicsDevice graphicsDevice)
        {
            obj.AABB.CalculateAABB(obj.Shape as PerfectPolygon);

            if (obj.AABB.Min.Y < 0)
            {
                Vector2 v = obj.Velocity;
                v.Y = -v.Y;
                obj.Velocity = v;

                v = Vector2.Zero;
                v.Y = -obj.AABB.Min.Y + 1;
                obj.Position += v;
            }
            else if (obj.AABB.Max.Y > graphicsDevice.Viewport.Height)
            {
                Vector2 v = obj.Velocity;
                v.Y = -v.Y;
                obj.Velocity = v;

                v = Vector2.Zero;
                v.Y = (graphicsDevice.Viewport.Height - obj.AABB.Max.Y) - 1;
                obj.Position += v;
            }

            if (obj.AABB.Min.X < 0)
            {
                Vector2 v = obj.Velocity;
                v.X = -v.X;
                obj.Velocity = v;

                v = Vector2.Zero;
                v.X = -obj.AABB.Min.X + 1;
                obj.Position += v;
            }
            else if (obj.AABB.Max.X > graphicsDevice.Viewport.Width)
            {
                Vector2 v = obj.Velocity;
                v.X = -v.X;
                obj.Velocity = v;

                v = Vector2.Zero;
                v.X = (graphicsDevice.Viewport.Width - obj.AABB.Max.X) - 1;
                obj.Position += v;
            }
        }
    }
}
