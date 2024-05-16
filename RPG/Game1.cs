using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static System.Formats.Asn1.AsnWriter;

namespace RPG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D backgroundTexture;
        private Animation playerAnimation;

        private Texture2D dialogeFrame;
        
        private Texture2D player;
        private Vector2 playerPosition;
        private Vector2 playerVelocity;
        private float speed = 5f;

        private NPC daughter;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 800; 
            _graphics.PreferredBackBufferHeight = 600; 
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTexture = Content.Load<Texture2D>("background");
            
            player = Content.Load<Texture2D>("male");
            playerAnimation = new Animation(player, 32, 32);
            playerPosition = new Vector2(50, 50);

            daughter = new NPC(Content.Load<Texture2D>("daughter"), new Vector2(100, 100), 32, 32);
            dialogeFrame = Content.Load<Texture2D>("dialoge");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            playerVelocity = Vector2.Zero;
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.A))
            {
                playerVelocity.X -= 1;
                playerAnimation.Update(gameTime);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                playerVelocity.X += 1;
                playerAnimation.Update(gameTime);
            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                playerPosition.Y -= 1;
                playerAnimation.Update(gameTime);
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                playerPosition.Y += 1;
                playerAnimation.Update(gameTime);
            }
            // TODO: Add your update logic here
            playerPosition += playerVelocity;

           

            //daughter.Update(gameTime);
            
            base.Update(gameTime); 
        }

        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            playerAnimation.Draw(_spriteBatch, playerPosition, playerVelocity);
            daughter.Draw(_spriteBatch);
            if (Collide()) _spriteBatch.Draw(dialogeFrame, new Vector2(200, 300), null, Color.White, 0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
            _spriteBatch.End();
            // TODO: Add your drawing code here
            
            base.Draw(gameTime);
        }

        protected bool Collide()
        {
            Rectangle playerRec = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);
            Rectangle daughterRec = new Rectangle((int)daughter.position.X, (int)daughter.position.Y, 32, 32);
            return playerRec.Intersects(daughterRec);
        }
    }
}