using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Mazer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        Player player;
        Map map;
        Texture2D tex;
        SpriteFont font;
        Thread worker;
        bool info;
        bool pause;
        bool showMap;
        double last = 0;
        KeyboardState oks;
        bool debug = true;

        public Game1()
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

            base.Initialize();
            Data.SetNull();
            graphics.PreferredBackBufferWidth = Data.Windowsize;
            graphics.PreferredBackBufferHeight = Data.Windowsize;
            graphics.ApplyChanges();
            this.camera = new Camera(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            this.player = new Player();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Data.SetNull();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tex = Content.Load<Texture2D>("pix");
            font = Content.Load<SpriteFont>("sf");
            map = new Map(DateTime.Now.Second * DateTime.Now.Millisecond * DateTime.Now.Hour, Data.BlockPerLevel,0);
            oks = Keyboard.GetState();
            pause = false;
            showMap = false;
            worker = new Thread(MapBuilder.CreateNextMap);
            worker.Start();
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
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            #region unpausable Logic
            this.UpdatePause();
            KeyboadInput(gameTime);
            #endregion
            #region pausable Logic
            if (!pause)
            {
                player.Update(gameTime);
                camera.Update(player.Position, 32, 64, 64, 64);
                if (gameTime.TotalGameTime.TotalMilliseconds - last >= 150)
                {
                    last = gameTime.TotalGameTime.TotalMilliseconds;
                    if (!map.OnPath(player.Hitbox))
                    {
                        player = new Player();
                        map.RecalcBuffer();
                        camera = new Camera(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                        this.camera.Zoom = (float)(Data.Windowsize / 256f);
                        Data.Death++;
                    }
                }
                if (map.restart)
                {
                    NextLevel();
                }
            }
            #endregion
            base.Update(gameTime);
            oks = Keyboard.GetState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //        spriteBatch.Begin(SpriteSortMode.Deferred,SpriteBlendMode.AlphaBlend SaveStateMode.None, camera.GetMatrix());

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, this.camera.GetMatrix());
            this.map.Draw(spriteBatch, tex,showMap);
            this.player.Draw(spriteBatch, tex);
            #region Draw Helper
            if (map.helper)
            {
                DrawLine(spriteBatch, new Vector2(player.Hitbox.Center.X, player.Hitbox.Center.Y), new Vector2(map.Destination.Hitbox.Center.X, map.Destination.Hitbox.Center.Y));
            }
            #endregion
            #region Draw Info
            spriteBatch.DrawString(font, "Level: " + Data.Levels + "    Death: " + Data.Death, new Vector2(-30, -20), Color.White);
            if (Data.Levels > 2)
                spriteBatch.DrawString(font, "Helper : " + map.HelperCount, new Vector2(-30, -40), Color.Blue);
            if (Data.Levels == 0)
            {
                spriteBatch.DrawString(font, "Press [H]", new Vector2(-30, -40), Color.White);
                spriteBatch.DrawString(font, "Find the green", new Vector2(-30, -60), Color.LimeGreen);
            }
            else if (Data.Levels == 2)
            {
                spriteBatch.DrawString(font, "Blue will show you the way", new Vector2(-30, -40), Color.Blue);
            }
            #endregion
            #region Draw Help
            if (info)
            {
                spriteBatch.DrawString(font, "Controls", new Vector2(-camera.position.X, -camera.position.Y), Color.Red);
                spriteBatch.DrawString(font, "W A S D = Moving", new Vector2(-camera.position.X, -camera.position.Y + 20), Color.Red);
                spriteBatch.DrawString(font, "H = Help", new Vector2(-camera.position.X, -camera.position.Y + 40), Color.Red);
                spriteBatch.DrawString(font, "L = Load last interrupted Game", new Vector2(-camera.position.X, -camera.position.Y + 60), Color.Red);
                spriteBatch.DrawString(font, "Y & X = Resize Window", new Vector2(-camera.position.X, -camera.position.Y + 80), Color.Red);
                spriteBatch.DrawString(font, "Space = Toggle Pause", new Vector2(-camera.position.X, -camera.position.Y + 100), Color.Red);
                spriteBatch.DrawString(font, "ESC = Save and Quit", new Vector2(-camera.position.X, -camera.position.Y + 120), Color.Red);
                spriteBatch.DrawString(font, "Old saves will be overwritten", new Vector2(-camera.position.X, -camera.position.Y + 160), Color.Red);
                spriteBatch.DrawString(font, "if you end an other game.", new Vector2(-camera.position.X, -camera.position.Y + 180), Color.Red);

            }
            #endregion
            #region Draw DebugInfo
            if(debug)
            {
                spriteBatch.DrawString(font, "BL" + Data.NextMaps.Count, new Vector2(-camera.position.X + 100, -camera.position.Y), Color.Yellow);
            }
            #endregion
            spriteBatch.End();

            base.Draw(gameTime);
        }
        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(tex,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    5), //width of line, change this to make thicker line
                null,
                Color.LimeGreen,
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }

        private void Load()
        {
            Data.Load();;
            int blocks = (Data.BlockPerLevel + (Data.Levels / 10) * 25) * Data.Levels;
            map = new Map(Data.LastSeed, Data.BlockPerLevel + blocks,Data.Levels);
            Data.NextMaps.Clear();
            player = new Player();
            camera = new Camera(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Data.Levels++;
        }
        private void SaveNEnd(GameTime gameTime)
        {
            Data.Time += Convert.ToInt32(gameTime.TotalGameTime.TotalSeconds);
            Data.LastSeed = map.Seed;
            Data.Save();
            this.Exit();
        }
        private void NextLevel()
        {
            while (Data.NextMaps.Count == 0)
                Thread.Sleep(100);
            map = Data.NextMaps.Dequeue();
            player = new Player();
            camera = new Camera(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            this.camera.Zoom = (float)(Data.Windowsize / 256f); 
            Data.Levels++;
        }
        private void ResizeGame(int mod)
        {
            Data.Windowsize += mod;
            if (Data.Windowsize <= 160)
                Data.Windowsize = 176;
            graphics.PreferredBackBufferWidth = Data.Windowsize;
            graphics.PreferredBackBufferHeight = Data.Windowsize;
            graphics.ApplyChanges();
            this.camera = new Camera(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            this.camera.Zoom = (float)(Data.Windowsize / 256f);
        }
        private void KeyboadInput(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || ks.IsKeyDown(Keys.Escape))
                SaveNEnd(gameTime);

            if (!pause)
            {
                if (ks.IsKeyDown(Keys.L) && !oks.IsKeyDown(Keys.L))
                    Load();

                info = ks.IsKeyDown(Keys.H);
                if (debug && ks.IsKeyDown(Keys.F12) && !oks.IsKeyDown(Keys.F12))
                    NextLevel();
                if (ks.IsKeyDown(Keys.Y) && !oks.IsKeyDown(Keys.Y))
                    ResizeGame(16);
                if (ks.IsKeyDown(Keys.X) && !oks.IsKeyDown(Keys.X))
                    ResizeGame(-16);
                if (debug && ks.IsKeyDown(Keys.M) && !oks.IsKeyDown(Keys.M))
                    ToggleMap();
            }

        }
        private void UpdatePause()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && oks.IsKeyUp(Keys.Space))
            {
                pause = !pause;
            }
        }
        private void ToggleMap()
        {
            showMap = !showMap;
            if (showMap)
            {
                camera.Zoom = 0.1f;
                camera.position = new Vector2(Data.Windowsize * 5, Data.Windowsize * 5);
            }
            else
            {
                this.camera.Zoom = (float)(Data.Windowsize / 256f);
            }
        }
    }
}
