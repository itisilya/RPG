using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
namespace RPG
{
    public class FieldLocation
    {
        private int playerHealth = 30;
        private Texture2D backgroundTexture;
        private Texture2D ruleTexture;
        private Texture2D skeletonTexture;
        private Texture2D hintTexture;
        private Texture2D deathScreen;
        private List<Skeleton> skeletons;
        private Random random;

        private bool showRules = true;
        private bool isSpacePressed = false;
        private bool showDeathScreen = false;
        private bool allDead = false;
        double lastSkeletonHitTime = 0;
        double skeletonHitInterval = 250;
        public FieldLocation()
        {
            skeletons = new List<Skeleton>();
            random = new Random();
        }
        public void LoadContent(ContentManager content)
        {
            backgroundTexture = content.Load<Texture2D>("background3");
            ruleTexture = content.Load<Texture2D>("rule");
            skeletonTexture = content.Load<Texture2D>("skeleton");
            deathScreen = content.Load<Texture2D>("death");
            hintTexture = content.Load<Texture2D>("hint3");
        }

        public void Update(GameTime gameTime, ref Vector2 playerPosition, ref bool isFourthLocation)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            if (showRules)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !isSpacePressed)
                {
                    showRules = false;
                    SpawnSkeletons();
                }
                isSpacePressed = keyboardState.IsKeyDown(Keys.Space);
                return;
            }
            var playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);

            List<Skeleton> skeletonsToRemove = new List<Skeleton>();
            double elapsedMilliseconds = gameTime.ElapsedGameTime.TotalMilliseconds;
            lastSkeletonHitTime += elapsedMilliseconds;

            foreach (var skeleton in skeletons)
            {
                skeleton.Update(gameTime, playerPosition, skeletons);
                if (playerBounds.Intersects(skeleton.Bounds))
                {
                    if (lastSkeletonHitTime >= skeletonHitInterval)
                    {
                        playerHealth -= 2;
                        if (playerHealth <= 0) showDeathScreen = true;
                        lastSkeletonHitTime = 0;
                    }
                }

                if (mouseState.LeftButton == ButtonState.Pressed && skeleton.Bounds.Contains(mouseState.Position))
                {
                    skeleton.Health -= 2;
                    if (skeleton.Health <= 0)
                        skeletonsToRemove.Add(skeleton);
                }
            }

            foreach (var skeletonToRemove in skeletonsToRemove)
                skeletons.Remove(skeletonToRemove);

            if (playerPosition.X > 780 - 32 && allDead) 
            {
                isFourthLocation = true;
                playerPosition = new Vector2(25, 600); 
            }
        }

        private void SpawnSkeletons()
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 position = new Vector2(random.Next(0, 780), random.Next(0, 650));
                skeletons.Add(new Skeleton(skeletonTexture, position));
            }
        }

        private void Respawn()
        {
            skeletons.Clear();
            playerHealth = 30;
            showDeathScreen = false;
            SpawnSkeletons();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 playerPosition, Animation playerAnimation, Vector2 playerVelocity)
        {
            allDead = skeletons.Count == 0;
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

            if (showDeathScreen)
            {
                spriteBatch.Draw(deathScreen, new Vector2(390 - ruleTexture.Width / 2, 325 - ruleTexture.Height / 2), Color.White);
                var keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Keys.Space)) Respawn();
            }
            else
            {
                if (showRules) spriteBatch.Draw(ruleTexture, new Vector2(390 - ruleTexture.Width / 2, 325 - ruleTexture.Height / 2), Color.White);
                else
                {
                    playerAnimation.Draw(spriteBatch, playerPosition, playerVelocity);
                    foreach (var skeleton in skeletons)
                        skeleton.Draw(spriteBatch);
                }
                if (allDead && !showRules)
                    spriteBatch.Draw(hintTexture, new Vector2((780 - hintTexture.Width) / 2, 10), Color.White);
            }
        }
    }

    public class Skeleton
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private float speed = 1f;
        public Rectangle Bounds;
        public int Health = 25;
        public Skeleton(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            Bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public void Update(GameTime gameTime, Vector2 playerPosition, List<Skeleton> skeletons)
        {
            Vector2 direction = playerPosition - position;
            if (direction.Length() > 0)
                direction.Normalize();
            
            velocity = direction * speed;
            Vector2 newPosition = position + velocity;

            foreach (var skeleton in skeletons)
                if (skeleton != this && (CollidesWith(skeleton, newPosition) || CollidesWith(skeleton, playerPosition)))
                    newPosition = position;

            position = newPosition;
            Bounds.Location = new Point((int)position.X, (int)position.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        private bool CollidesWith(Skeleton other, Vector2 newPosition)
        {
            Rectangle newBounds = new Rectangle((int)newPosition.X, (int)newPosition.Y, texture.Width, texture.Height);
            return newBounds.Intersects(other.Bounds);
        }
    }
}
