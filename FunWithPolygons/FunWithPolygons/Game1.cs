#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace FunWithPolygons
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BroadPhase bPhase;
        NarrowPhase nPhase;
        Scene scene;

        List<Pair> pairs;

        const float fps = 100;
        const float dt = 1 / fps;
        float accumulator = 0;

        float inputForce;
        static Random rand;

        Body triangle;
        Texture2D triangleTexture;
        Body hexagon;
        Texture2D hexagonTexture;
        Body square;
        Texture2D squareTexture;
        Body pentagon;
        Texture2D pentagonTexture;
        Body octagon;
        Texture2D octagonTexture;
        Body ball;
        Texture2D ballTexture;
        Texture2D bullet;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            bPhase = new BroadPhase();
            nPhase = new NarrowPhase(this);
            scene = new Scene(new Vector2(0, 9.8f), dt);
            inputForce = 10f;
            rand = new Random();
            pairs = new List<Pair>();

            //GraphicsDevice.Viewport = new Viewport(100, 100, 1600, 800);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            triangleTexture = Content.Load<Texture2D>("triangle.png");
            triangle = new Body(triangleTexture, 400.0f, 240.0f, 5.0f, 0.5f, 3);
            hexagonTexture = Content.Load<Texture2D>("hexagon.png");
            hexagon = new Body(hexagonTexture, (float)rand.Next(0, GraphicsDevice.Viewport.Width - hexagonTexture.Width), (float)rand.Next(0, GraphicsDevice.Viewport.Height - hexagonTexture.Height), 10.0f, 0.5f, 6);
            squareTexture = Content.Load<Texture2D>("square.png");
            square = new Body(squareTexture, (float)rand.Next(0, GraphicsDevice.Viewport.Width - squareTexture.Width), (float)rand.Next(0, GraphicsDevice.Viewport.Height - squareTexture.Height), 5.0f, 0.5f, 4);
            pentagonTexture = Content.Load<Texture2D>("pentagon.png");
            pentagon = new Body(pentagonTexture, (float)rand.Next(0, GraphicsDevice.Viewport.Width - pentagonTexture.Width), (float)rand.Next(0, GraphicsDevice.Viewport.Height - pentagonTexture.Height), 7.5f, 0.5f, 5);
            octagonTexture = Content.Load<Texture2D>("octagon.png");
            octagon = new Body(octagonTexture, (float)rand.Next(0, GraphicsDevice.Viewport.Width - octagonTexture.Width), (float)rand.Next(0, GraphicsDevice.Viewport.Height - octagonTexture.Height), 2.5f, 0.5f, 8);

            scene.InsertBody(triangle);
            scene.InsertBody(hexagon);
            scene.InsertBody(square);
            scene.InsertBody(pentagon);
            scene.InsertBody(octagon);

            bullet = Content.Load<Texture2D>("bulletDev.png");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Uses linear interpolation to be able to calculate and render the objects position inbetween time steps
        /// </summary>
        /// <param name="alpha"></param>
        public void RenderGame(float alpha)
        {
            foreach (Body body in scene.Bodies)
            {
                Transform i = body.Shape.Previous * alpha + body.Shape.Current * (1.0f - alpha);
                body.Shape.Previous = body.Shape.Current;
                spriteBatch.Begin();

                body.Draw(spriteBatch, bullet, i.Position);

                spriteBatch.End();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float frameStart = gameTime.ElapsedGameTime.Milliseconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.Up))
            {
                triangle.ForceVector = new Vector2(0, -1);
                triangle.ApplyForce(inputForce, triangle.ForceVector);
            }

            if (kb.IsKeyDown(Keys.Down))
            {
                triangle.ForceVector = new Vector2(0, 1);
                triangle.ApplyForce(inputForce, triangle.ForceVector);
            }

            if (kb.IsKeyDown(Keys.Left))
            {
                triangle.ForceVector = new Vector2(-1, 0);
                triangle.ApplyForce(inputForce, triangle.ForceVector);
            }

            if (kb.IsKeyDown(Keys.Right))
            {
                triangle.ForceVector = new Vector2(1, 0);
                triangle.ApplyForce(inputForce, triangle.ForceVector);
            }

            float currentTime = gameTime.TotalGameTime.Milliseconds;

            accumulator += currentTime - frameStart;

            frameStart = currentTime;

            if (accumulator > 0.2f)
                accumulator = 0.2f;

            while (accumulator > dt)
            {
                bPhase.GeneratePairs(scene.Bodies);
                scene.UpdatePhysics(bPhase.Pairs, nPhase);
                scene.Step(GraphicsDevice, dt);
                accumulator -= dt;
            }

            float alpha = (accumulator / dt);

            RenderGame(alpha);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            for (int i = 0; i < scene.Bodies.Count; i++)
            {
                scene.Bodies[i].Draw(spriteBatch, bullet);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
