using Microsoft.Xna.Framework.Graphics;
using System;
namespace RPG
{
    public class NPC
    {
        private NPCAnimation npcAnimation;
        public Microsoft.Xna.Framework.Vector2 position;

        public NPC(Texture2D spriteSheet, Microsoft.Xna.Framework.Vector2 position, int frameWidth, int frameHeight)
        {
            this.position = position;
            npcAnimation = new NPCAnimation(spriteSheet, frameWidth, frameHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            npcAnimation.Draw(spriteBatch, position); 
        }
    }
}
