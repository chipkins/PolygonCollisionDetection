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
    /// This is the generic structure for any object that can collide with another object
    /// </summary>
    public class Body
    {
        private Shape shape;
        private Material material;
        private MassData massData;
        private Vector2 velocity;
        private AABB aabb;
        private Vector2 forceVector;
        private Vector2 totalForce;
        private Texture2D texture;
        //Coefficients of friction
        private float kineticFriction;
        private float staticFriction;

        public Body(Texture2D texture, float x, float y, float density, float restitution, int sides = 0)
        {
            shape = new PerfectPolygon(sides, x, y, texture);
            shape.Center = new Vector2(x + (texture.Width / 2), y + (texture.Height / 2));
            this.texture = texture;
            massData = new MassData(shape, density, sides);
            material = new Material(density, restitution);
            velocity = Vector2.Zero;
            aabb = new AABB(shape.Position, new Vector2(shape.Position.X + texture.Width, shape.Position.X + texture.Height));
            totalForce = Vector2.Zero;
            forceVector = new Vector2(0, 1);
            kineticFriction = 0.3f;
            staticFriction = 0.5f;
        }

        public Vector2 Position
        {
            get { return shape.Position; }
            set
            {
                (shape as PerfectPolygon).Position = value;
            }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector2 Center
        {
            get { return shape.Center; }
            set { shape.Center = value; }
        }

        public Vector2 TotalForce
        {
            get { return totalForce; }
            set { totalForce = value; }
        }

        public Vector2 ForceVector
        {
            get { return forceVector; }
            set { forceVector = value; }
        }

        public float Restitution
        {
            get { return material.Restitution; }
            set { material.Restitution = value; }
        }

        public float Density
        {
            get { return material.Density; }
            set { material.Density = value; }
        }

        public float InvMass
        {
            get { return massData.invMass; }
        }

        public float Mass
        {
            get { return massData.mass; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Shape Shape
        {
            get { return shape; }
        }

        public AABB AABB
        {
            get { return aabb; }
        }

        public float DynamicFriction
        {
            get { return kineticFriction; }
        }

        public float StaticFriction
        {
            get { return staticFriction; }
        }

        /// <summary>
        /// Applies a calculated amount of impulse to the Body
        /// </summary>
        /// <param name="impulse"></param>
        /// <param name="contactVector"></param>
        public void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
        {
            velocity += InvMass * impulse;
        }

        /// <summary>
        /// Applies a calculated amount of force to the Body
        /// </summary>
        /// <param name="f"></param>
        /// <param name="v"></param>
        public void ApplyForce(float f, Vector2 v)
        {
            totalForce += v * f;
        }

        /// <summary>
        /// Draws the Body
        /// As well as markers to show where the points of the polygon are located (debug)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="markers"></param>
        public void Draw(SpriteBatch spriteBatch, Texture2D markers, Vector2 interpolate = default(Vector2))
        {
            if(interpolate != Vector2.Zero)
                spriteBatch.Draw(texture, interpolate, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1.0f, SpriteEffects.None, 1);
            else
                spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1.0f, SpriteEffects.None, 1);
            
            (shape as PerfectPolygon).Draw(spriteBatch, markers);
        }
    }
}
