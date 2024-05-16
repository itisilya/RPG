using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public class DialogBox
    {
        private Texture2D dialogTexture;
        private Vector2 position;

        public bool IsActive { get; set; }

        public DialogBox(Texture2D dialogTexture, Vector2 position)
        {
            this.dialogTexture = dialogTexture;
            this.position = position;
            IsActive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(dialogTexture, position, Color.White);
            }
        }
    }
}
