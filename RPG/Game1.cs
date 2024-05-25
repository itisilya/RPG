using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.ExceptionServices;
using System.Runtime.Intrinsics.X86;
using static System.Formats.Asn1.AsnWriter;

namespace RPG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D backgroundTexture;
        public Animation playerAnimation;

        Texture2D[] dialogTextures;
        int currentDialogIndex = 0;
        bool isDialogActive = false;
        public bool isDialogTold = false;
        bool isSpacePressed = false;

        double dialogTimer = 0;
        double dialogInterval = 500;

        private static Texture2D hintTexture;
        private static Texture2D hintTexture0;

        private Texture2D player;
        private Vector2 playerPosition;
        private Vector2 playerVelocity;
        private Rectangle playerBounds;
        private float playerSpeed = 2.7f;

        private NPC daughter;
        private Rectangle daughterBounds;

        private bool isSecondLocation = false;
        private CastleLocation castleLocation;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 780; 
            _graphics.PreferredBackBufferHeight = 650; 
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTexture = Content.Load<Texture2D>("background1");
            
            player = Content.Load<Texture2D>("male");
            playerAnimation = new Animation(player, 32, 32);
            playerPosition = new Vector2(50, 50);
            playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);

            daughter = new NPC(Content.Load<Texture2D>("daughter"), new Vector2(100, 100), 32, 32);
            daughterBounds = new Rectangle((int)daughter.position.X, (int)daughter.position.Y, 32, 32);

            int dialogFrameCount1 = 11;
            dialogTextures = new Texture2D[dialogFrameCount1];
            for (int i = 0; i < dialogFrameCount1; i++)
                dialogTextures[i] = Content.Load<Texture2D>($"1/dialoge{i + 1}");

            hintTexture = Content.Load<Texture2D>("hint");
            hintTexture0 = Content.Load<Texture2D>("hint0");
            castleLocation = new CastleLocation();
            castleLocation.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var keyboard = Keyboard.GetState();
            if (!isSecondLocation)
            {
                Vector2 oldPosition = playerPosition;
                playerVelocity = Vector2.Zero;
                
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
                    playerPosition.Y -= playerSpeed;
                    playerAnimation.Update(gameTime);
                }
                if (keyboard.IsKeyDown(Keys.S))
                {
                    playerPosition.Y += playerSpeed;
                    playerAnimation.Update(gameTime);
                }
                playerPosition += playerVelocity * playerSpeed;
                
                if (playerPosition.X > GraphicsDevice.Viewport.Width - 32 && isDialogTold)
                {
                    isSecondLocation = true;
                    playerPosition = new Vector2(50, GraphicsDevice.Viewport.Height / 2); // Позиция игрока при входе на вторую локацию
                }

                playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 20, 20);
                var housesBounds = new Rectangle(162, 22, 600, 308);
                if (Collide(playerBounds, daughterBounds)
                    || Collide(playerBounds, housesBounds)
                    || playerPosition.Y < 0
                    || playerPosition.X < 0
                    || playerPosition.Y > GraphicsDevice.Viewport.Height - 32)
                    playerPosition = oldPosition;

                Rectangle interactionDaughterZone = new Rectangle(
                daughterBounds.X - 10, daughterBounds.Y - 10,
                daughterBounds.Width + 20, daughterBounds.Height + 20);

                if (Collide(playerBounds, interactionDaughterZone) && !isDialogActive)
                    isDialogActive = true;
                dialogTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                KeyboardState keyboardState = Keyboard.GetState();
                bool spaceKeyPressed = keyboardState.IsKeyDown(Keys.Space);
                if (isDialogActive && spaceKeyPressed && !isSpacePressed && dialogTimer >= dialogInterval)
                {
                    currentDialogIndex++;
                    if (currentDialogIndex >= dialogTextures.Length)
                    {
                        isDialogActive = false;
                        isDialogTold = true;
                        currentDialogIndex = 0;
                    }
                    dialogTimer = 0;
                }
                isSpacePressed = spaceKeyPressed;
            }
            else
            {
                castleLocation.Update(gameTime, keyboard);
            }
            base.Update(gameTime); 
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            if (!isSecondLocation)
            {
                _spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

                playerAnimation.Draw(_spriteBatch, playerPosition, playerVelocity);

                daughter.Draw(_spriteBatch);

                int windowWidth = GraphicsDevice.Viewport.Width;
                int windowHeight = GraphicsDevice.Viewport.Height;
                if (!isDialogActive) _spriteBatch.Draw(hintTexture0, new Vector2((windowWidth - hintTexture.Width) / 2, 10), Color.White);
                if (isDialogActive && !isDialogTold)
                {
                    int dialogSpriteWidth = dialogTextures[currentDialogIndex].Width / 2;
                    int dialogSpriteHeight = dialogTextures[currentDialogIndex].Height / 2;
                    

                    Vector2 dialogPosition = new Vector2(
                        (windowWidth - dialogSpriteWidth) / 2,  // wентрирование по горизонтали
                        windowHeight - dialogSpriteHeight - 7 // Размещение внизу, отступ 7 пикселей от нижнего края
                    );
                    _spriteBatch.Draw(dialogTextures[currentDialogIndex], dialogPosition, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                }

                if (isDialogTold)
                {
                    
                    _spriteBatch.Draw(hintTexture, new Vector2((windowWidth - hintTexture.Width) / 2, 10), Color.White);
                }
                
            }
            else
            {
                // Отрисовка второй локации
                castleLocation.Draw(_spriteBatch, GraphicsDevice);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        protected bool Collide(Rectangle first, Rectangle second)
        {
            return first.Intersects(second);
        }
    }
}