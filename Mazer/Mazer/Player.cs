using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Mazer
{
    class Player
    {
        public Rectangle Hitbox { get { return this.hitbox; } }
        public Vector2 Position { get { return new Vector2(position.X, position.Y); } }

        private Rectangle hitbox;
        private Point position;
        private double last;

        public Player()
        {
            this.position = new Point(0, 0);
            this.hitbox = new Rectangle(0, 0, 32, 32);
            this.updateHitbox();
            this.last = 0f;
        }

        private void updateHitbox()
        {
            this.hitbox.X = this.position.X;
            this.hitbox.Y = this.position.Y;
        }
        private void updateInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                position.Y -= 8;
                Data.TraveldDistance++;
                updateHitbox();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                position.Y += 8;
                Data.TraveldDistance++;
                updateHitbox();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.X -= 8;
                Data.TraveldDistance++;
                updateHitbox();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.X += 8;
                Data.TraveldDistance++;
                updateHitbox();
            }
        }

        public void Update(GameTime gt)
        {
            if (gt.TotalGameTime.TotalMilliseconds - last >= 33)
            {
                this.updateInput();
                last = gt.TotalGameTime.TotalMilliseconds;
            }
        }

        public void Draw(SpriteBatch sb, Texture2D tex)
        {
            sb.Draw(tex, hitbox, Color.LightBlue);
        }
    }
}
