using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{

    public class MountainLocation
    {
        private Texture2D backgroundTexture;
        private Texture2D goldenTreeTexture;
        private Texture2D hintTexture;
        private bool isTreeVisable = true;
        
        public void LoadContent(ContentManager content)
        {
            backgroundTexture = content.Load<Texture2D>("background4");
            goldenTreeTexture = content.Load<Texture2D>("goldentree");
            hintTexture = content.Load<Texture2D>("hint4");
        }
        public void Update(ref Vector2 playerPosition, ref bool isFifthLocation)
        {
            //var keyboard = Keyboard.GetState();
            if (playerPosition.X > 780 - 32)  // Assuming the bottom boundary
            {
                isFifthLocation = true;
                playerPosition = new Vector2(20, 510);  // Position when entering the fourth location
            }
            var playerBounds = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32);
            var treeBounds = new Rectangle(550, 210, 350, 420);
            if (playerBounds.Intersects(treeBounds)) isTreeVisable = false;
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 playerPosition, Animation playerAnimation, Vector2 playerVelocity)
        {
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            playerAnimation.Draw(spriteBatch, playerPosition, playerVelocity);
            if (isTreeVisable) spriteBatch.Draw(goldenTreeTexture, new Vector2(400, 210), Color.White);
            spriteBatch.Draw(hintTexture, new Vector2((780 - hintTexture.Width) / 2, 10), Color.White);
        }
    }
}
