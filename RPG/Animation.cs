using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace RPG
{
    public class Animation
    {
        private Texture2D spriteSheet;
        public int frameWidth;
        public int frameHeight;
        private int currentFrame;
        private int totalFrames;//количество кадров для 1 анимации

        private float timer;
        private float interval = 100f;//интервал между сменой фрейма в миллисекундах
        public Animation(Texture2D spriteSheet, int frameWidth, int frameHeight)
        {
            this.spriteSheet = spriteSheet;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            totalFrames = spriteSheet.Width / frameWidth;
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame == totalFrames)
                    currentFrame = 0;
                timer = 0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 velocity, int row = 1)
        {
            if (velocity.Y > 0) row = 1; //вниз
            if (velocity.Y < 0) //вверх
                row = 4;
            if (velocity.X < 0) //влево
                row = 2;
            if (velocity.X > 0) //вправо
                row = 3;
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, (row - 1) * frameHeight, frameWidth, frameHeight);
            spriteBatch.Draw(spriteSheet, position, sourceRect, Color.White);
        }
    }
}
