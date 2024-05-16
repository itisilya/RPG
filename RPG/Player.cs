using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public class Player
    {
        private Animation playerAnimation;
        public Vector2 position;

        public Player(Texture2D spriteSheet, Vector2 position, int frameWidth, int frameHeight)
            {
                this.position = position;
                
            }
    }
}
