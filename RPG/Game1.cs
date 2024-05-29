using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
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
        
        //игрок
        private Texture2D player;
        private Vector2 playerPosition;
        private Vector2 playerVelocity;
        private Rectangle playerBounds;
        public Animation playerAnimation;
        private float playerSpeed = 2.7f;

        //диалог
        Texture2D[] dialogTextures;
        int currentDialogIndex = 0;
        bool isDialogActive = false;
        public bool isDialogTold = false;
        bool isSpacePressed = false;
        double dialogTimer = 0;
        double dialogInterval = 500;

        //подсказки
        private static Texture2D hintTexture;
        private static Texture2D hintTexture0;

        //дочка короля
        private NPC daughter;
        private Rectangle daughterBounds;

        private enum Location
        {
            FirstLocation,
            SecondLocation,
            ThirdLocation,
            FourthLocation,
            FifthLocation
        }

        private Location currentLocation;
        private CastleLocation castleLocation;
        private FieldLocation fieldLocation;
        private MountainLocation mountainLocation;
        private FinalLocation finalLocation;

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

            fieldLocation = new FieldLocation();
            fieldLocation.LoadContent(Content);

            mountainLocation = new MountainLocation();
            mountainLocation.LoadContent(Content);

            finalLocation = new FinalLocation();
            finalLocation.LoadContent(Content);
        }

        public void PlayerMoves(KeyboardState k, GameTime gm)
        {
            if (k.IsKeyDown(Keys.A))
            {
                playerVelocity.X -= 1;
                playerAnimation.Update(gm);
            }
            if (k.IsKeyDown(Keys.D))
            {
                playerVelocity.X += 1;
                playerAnimation.Update(gm);
            }
            if (k.IsKeyDown(Keys.W))
            {
                playerVelocity.Y -= 1;
                playerAnimation.Update(gm);
            }
            if (k.IsKeyDown(Keys.S))
            {
                playerVelocity.Y += 1;
                playerAnimation.Update(gm);
            }
            playerPosition += playerVelocity * playerSpeed;
        }

        public void DialogLogic(bool k)
        {
            if (isDialogActive && k && !isSpacePressed && dialogTimer >= dialogInterval)
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
            isSpacePressed = k;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var keyboard = Keyboard.GetState();

            switch (currentLocation)
            {
                case Location.FirstLocation:
                    Vector2 oldPosition = playerPosition;
                    playerVelocity = Vector2.Zero;

                    PlayerMoves(keyboard, gameTime);

                    if (playerPosition.X > GraphicsDevice.Viewport.Width - 32 && isDialogTold)
                    {
                        currentLocation = Location.SecondLocation;
                        playerPosition = new Vector2(50, GraphicsDevice.Viewport.Height / 2); //позиция игрока при входе на вторую локацию
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
                    DialogLogic(spaceKeyPressed);
                    break;

                case Location.SecondLocation:
                    playerVelocity = Vector2.Zero;
                    PlayerMoves(keyboard, gameTime);
                    bool isThirdLocation = false;
                    castleLocation.Update(gameTime, ref playerPosition, ref isThirdLocation);
                    if (isThirdLocation)
                    {
                        currentLocation = Location.ThirdLocation;
                        playerPosition = new Vector2(390, 325);
                    }
                    break;
                case Location.ThirdLocation:
                    bool isFourthLocation = false;
                    playerVelocity = Vector2.Zero;
                    PlayerMoves(keyboard, gameTime);
                    fieldLocation.Update(gameTime, ref playerPosition, ref isFourthLocation);
                    if (isFourthLocation)
                    {
                        currentLocation = Location.FourthLocation;
                        playerPosition = new Vector2(20, 600);
                    }
                   break;
                case Location.FourthLocation:
                    bool isFifthLocation = false;
                    playerVelocity = Vector2.Zero;
                    PlayerMoves(keyboard, gameTime);
                    mountainLocation.Update(ref playerPosition, ref isFifthLocation);
                    if (isFifthLocation)
                    {
                        currentLocation = Location.FifthLocation;
                        playerPosition = new Vector2(20, 510);
                    }
                    break;
                case Location.FifthLocation:
                    playerVelocity = Vector2.Zero;
                    PlayerMoves(keyboard, gameTime);
                    finalLocation.Update(gameTime, ref playerPosition);
                    break;
            }
            base.Update(gameTime); 
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            
            switch (currentLocation)
            {
                case Location.FirstLocation:
                    _spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

                    playerAnimation.Draw(_spriteBatch, playerPosition, playerVelocity);

                    daughter.Draw(_spriteBatch);
                    if (!isDialogActive) _spriteBatch.Draw(hintTexture0, new Vector2((780 - hintTexture.Width) / 2, 10), Color.White);
                    if (isDialogActive && !isDialogTold)
                    {
                        int dialogSpriteWidth = dialogTextures[currentDialogIndex].Width / 2;
                        int dialogSpriteHeight = dialogTextures[currentDialogIndex].Height / 2;

                        Vector2 dialogPosition = new Vector2(
                            (780 - dialogSpriteWidth) / 2,  //центрирование по горизонтали
                            650 - dialogSpriteHeight - 7 // размещение внизу, отступ 7 пикселей от нижнего края
                        );
                        _spriteBatch.Draw(dialogTextures[currentDialogIndex], dialogPosition, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                    }

                    if (isDialogTold)
                    {
                        _spriteBatch.Draw(hintTexture, new Vector2((780 - hintTexture.Width) / 2, 10), Color.White);
                    }
                    break;

                case Location.SecondLocation:
                    castleLocation.Draw(_spriteBatch, playerPosition, playerAnimation, playerVelocity);
                    break;
                case Location.ThirdLocation:
                    fieldLocation.Draw(_spriteBatch, playerPosition, playerAnimation, playerVelocity);
                    break;

                case Location.FourthLocation:
                    mountainLocation.Draw(_spriteBatch, playerPosition, playerAnimation, playerVelocity);
                    break;

                case Location.FifthLocation:
                    finalLocation.Draw(_spriteBatch, playerPosition, playerAnimation, playerVelocity);
                    break;
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