using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public class CastleLocation
    {
        private Texture2D backgroundTexture;
        
        private NPC king;
        private Rectangle kingBounds;

        private Animation player;
        private Vector2 playerPosition;
        private float playerSpeed = 2.7f;
        private Vector2 playerVelocity;
        private Rectangle playerBounds;

        Texture2D[] dialogTextures;
        int currentDialogIndex = 0;
        bool isDialogActive = false;
        public bool isDialogTold = false;
        bool isSpacePressed = false;
        private static Texture2D hintTexture;

        double dialogTimer = 0;
        double dialogInterval = 500;
        public void LoadContent(ContentManager content)
        {
          
            backgroundTexture = content.Load<Texture2D>("background2");
            king = new NPC(content.Load<Texture2D>("king"), new Vector2(300, 300), 32, 32);
            player = new Animation(content.Load<Texture2D>("male"), 32, 32);
            playerPosition = new Vector2(50, 300);
            int dialogFrameCount1 = 17;
            dialogTextures = new Texture2D[dialogFrameCount1];
            for (int i = 0; i < dialogFrameCount1; i++)
                dialogTextures[i] = content.Load<Texture2D>($"2/dialoge{i + 1}");

            hintTexture = content.Load<Texture2D>("hint2");
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 oldPosition = playerPosition;
            playerVelocity = Vector2.Zero;
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.A))
            {
                playerVelocity.X -= 1;
                player.Update(gameTime);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                playerVelocity.X += 1;
                player.Update(gameTime);
            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                playerPosition.Y -= playerSpeed;
                player.Update(gameTime);
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                playerPosition.Y += playerSpeed;
                player.Update(gameTime);
            }
            playerPosition += playerVelocity * playerSpeed;

            playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);
            kingBounds = new Rectangle((int)king.position.X, (int)king.position.Y, 32, 32);

            if (playerBounds.Intersects(kingBounds)) isDialogActive = true;
            dialogTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            keyboardState = Keyboard.GetState();
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
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            player.Draw(spriteBatch, playerPosition, playerVelocity);
            king.Draw(spriteBatch);
            if (isDialogActive && !isDialogTold)
            {
                int dialogSpriteWidth = dialogTextures[currentDialogIndex].Width / 2;
                int dialogSpriteHeight = dialogTextures[currentDialogIndex].Height / 2;
                int windowWidth = graphicsDevice.Viewport.Width;
                int windowHeight = graphicsDevice.Viewport.Height;

                Vector2 dialogPosition = new Vector2(
                    (windowWidth - dialogSpriteWidth) / 2,  // wентрирование по горизонтали
                    windowHeight - dialogSpriteHeight - 7 // Размещение внизу, отступ 7 пикселей от нижнего края
                );
                spriteBatch.Draw(dialogTextures[currentDialogIndex], dialogPosition, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }

            if (isDialogTold)
            {
                int windowWidth = graphicsDevice.Viewport.Width;
              
                spriteBatch.Draw(hintTexture, new Vector2((windowWidth - hintTexture.Width) / 2, 10), Color.White);
            }
        }
    }
}
