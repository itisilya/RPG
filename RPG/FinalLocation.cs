using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace RPG
{
    public class FinalLocation
    {
        private Texture2D backgroundTexture;

        private NPC daughter;
        private Rectangle daughterBounds;

        private NPC king;
        private Rectangle kingBounds;

        Texture2D[] dialogTextures;

        int currentDialogIndex = 0;
        bool isDialogActive = false;
        bool isDialogTold = false;
        bool isSpacePressed = false;
        double dialogTimer = 0;
        double dialogInterval = 500;

        private static Texture2D hintTexture;
        public void LoadContent(ContentManager content)
        {
            backgroundTexture = content.Load<Texture2D>("background5");
            
            daughter = new NPC(content.Load<Texture2D>("daughter"), new Vector2(410, 510), 32, 32);
            daughterBounds = new Rectangle((int)daughter.position.X, (int)daughter.position.Y, 32, 32);
            
            king = new NPC(content.Load<Texture2D>("king"), new Vector2(450, 510), 32, 32);
            kingBounds = new Rectangle((int)king.position.X, (int)king.position.Y, 32, 32);

            int dialogFrameCount = 12;
            dialogTextures = new Texture2D[dialogFrameCount];
            for (int i = 0; i < dialogFrameCount; i++)
                dialogTextures[i] = content.Load<Texture2D>($"3/dialoge{i + 1}");
            hintTexture = content.Load<Texture2D>("hint5");
        }

        public void Update(GameTime gameTime, ref Vector2 playerPosition)
        {
            Rectangle playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);
            kingBounds = new Rectangle((int)king.position.X, (int)king.position.Y, 32, 32);
            daughterBounds = new Rectangle((int)daughter.position.X, (int)daughter.position.X, 32, 32);
            
            Rectangle interactionKingZone = new Rectangle(
                kingBounds.X - 10, kingBounds.Y - 10,
                kingBounds.Width + 20, kingBounds.Height + 20);
            Rectangle interactionDaughterZone = new Rectangle(
                daughterBounds.X - 10, daughterBounds.Y - 10,
                daughterBounds.Width + 20, daughterBounds.Height + 20);

            var oldPosition = playerPosition;
            if (playerBounds.Intersects(kingBounds) || playerBounds.Intersects(daughterBounds)) 
                playerPosition = oldPosition;
            
            if (playerBounds.Intersects(interactionKingZone)||playerBounds.Intersects(interactionDaughterZone))
                isDialogActive = true;

            dialogTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            var keyboard = Keyboard.GetState();
            bool spaceKeyPressed = keyboard.IsKeyDown(Keys.Space);
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

        public void Draw(SpriteBatch spriteBatch, Vector2 playerPosition, Animation playerAnimation, Vector2 playerVelocity)
        {
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            playerAnimation.Draw(spriteBatch, playerPosition, playerVelocity);
            king.Draw(spriteBatch);
            daughter.Draw(spriteBatch);

            if (isDialogActive && !isDialogTold)
            {
                int dialogSpriteWidth = dialogTextures[currentDialogIndex].Width / 2;
                Vector2 dialogPosition = new Vector2((780 - dialogSpriteWidth) / 2, 5);
                spriteBatch.Draw(dialogTextures[currentDialogIndex], dialogPosition, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }

            if (isDialogTold)
                spriteBatch.Draw(hintTexture, new Vector2((780 - hintTexture.Width) / 2, 10), Color.White);
        }
    }
}
