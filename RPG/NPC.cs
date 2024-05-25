using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
