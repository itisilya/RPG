using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace RPG
{
    public class CastleLocation
    {
        private Texture2D backgroundTexture;
        private NPC king;
        private Rectangle kingBounds;
        private Texture2D[] kingDialogTextures;
        private static Texture2D hintTexture;
        private int currentDialogIndex = 0;
        private bool isDialogActive = false;
        private bool isDialogTold = false;
        private double dialogTimer = 0;
        private double dialogInterval = 500;
        private bool isSpacePressed = false;

        public void LoadContent(ContentManager content)
        {
            backgroundTexture = content.Load<Texture2D>("background2");
            king = new NPC(content.Load<Texture2D>("king"), new Vector2(300, 300), 32, 32);
            kingBounds = new Rectangle((int)king.position.X, (int)king.position.Y, 32, 32);

            int dialogFrameCount = 17;
            kingDialogTextures = new Texture2D[dialogFrameCount];
            for (int i = 0; i < dialogFrameCount; i++)
                kingDialogTextures[i] = content.Load<Texture2D>($"2/dialoge{i + 1}");
            
            hintTexture = content.Load<Texture2D>("hint2");
        }

        public void Update(GameTime gameTime, ref Vector2 playerPosition, ref bool isThirdLocation)
        {
            var keyboard = Keyboard.GetState();
            if (playerPosition.Y > 650 - 32 && isDialogTold)
            {
                isThirdLocation = true;
                playerPosition = new Vector2(500, 500);
            }

            Rectangle playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);
            kingBounds = new Rectangle((int)king.position.X, (int)king.position.Y, 32, 32);
            
            Rectangle interactionKingZone = new Rectangle(
                kingBounds.X - 10, kingBounds.Y - 10,
                kingBounds.Width + 20, kingBounds.Height + 20);

            if (playerBounds.Intersects(interactionKingZone) && !isDialogActive)
                isDialogActive = true;

            dialogTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            bool spaceKeyPressed = keyboard.IsKeyDown(Keys.Space);
            if (isDialogActive && spaceKeyPressed && !isSpacePressed && dialogTimer >= dialogInterval)
            {
                currentDialogIndex++;
                if (currentDialogIndex >= kingDialogTextures.Length)
                {
                    isDialogActive = false;
                    isDialogTold = true;
                    currentDialogIndex = 0;
                }
                dialogTimer = 0;
            }
            isSpacePressed = spaceKeyPressed;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 playerPosition, Animation playerAnimation, Vector2 playerVelocity)
        {
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            playerAnimation.Draw(spriteBatch, playerPosition, playerVelocity);
            king.Draw(spriteBatch);

            if (isDialogActive && !isDialogTold)
            {
                int dialogSpriteWidth = kingDialogTextures[currentDialogIndex].Width / 2;
                int dialogSpriteHeight = kingDialogTextures[currentDialogIndex].Height / 2;

                Vector2 dialogPosition = new Vector2 ((780 - dialogSpriteWidth) / 2, 650 - dialogSpriteHeight - 7);
                spriteBatch.Draw(kingDialogTextures[currentDialogIndex], dialogPosition, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
            if (isDialogTold) 
                spriteBatch.Draw(hintTexture, new Vector2((780 - hintTexture.Width) / 2, 10), Color.White);
        }
    }
}