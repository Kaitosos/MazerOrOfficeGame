using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    class Player : IWorldItem
    {
        public Rectangle Hitbox { get { return this.hitbox; } }
        public Vector2 Position { get { return new Vector2(position.X, position.Y); } }
        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
            }
        }
        public float Size
        {
            get { return Data.BlockSize / Data.PlayerRelationToBlocksize; }
        }
        public int TouchDamage
        {
            get { return this.touchDamage; }
        }
        public int LivePoints
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public List<IWorldItem> TouchedThisFrame
        {
            get { return this.touchedThisFrame; }
        }
        public TimeSpan LiveTime
        {
            get { return TimeSpan.MaxValue; }
        }
        public WorldItemType Type
        {
            get { return WorldItemType.Player; }
        }

        private Color color;
        private bool hurt;
        private Rectangle hitbox;
        private Point position;
        private double last;
        private int touchDamage;
        private int livePoints;
        private List<IWorldItem> touchedThisFrame;
        private KeyboardState oldState;

        public Player()
        {
            this.position = new Point(0, 0);
            this.hitbox = new Rectangle(0, 0, Data.BlockSize / Data.PlayerRelationToBlocksize, Data.BlockSize / Data.PlayerRelationToBlocksize);
            this.updateHitbox();
            this.last = 0f;
            this.touchDamage = 10;
            this.livePoints = 100;
            this.color = GameColors.PNormal;
            this.hurt = false;
            this.touchedThisFrame = new List<IWorldItem>();
            oldState = Keyboard.GetState();
        }

        public void Update(GameTime gt)
        {
            this.hurt = false;
            this.touchedThisFrame.Clear();
            if (gt.TotalGameTime.TotalMilliseconds - last >= 33)
            {
                this.updateInput();
                last = gt.TotalGameTime.TotalMilliseconds;
            }
        }

        public void Draw(SpriteBatch sb, Texture2D tex)
        {
            sb.Draw(tex, hitbox, color);
        }

        public void Touch(IWorldItem partner)
        {

            switch (partner.Type)
            {
                case WorldItemType.Spawner:
                case WorldItemType.EnemyBullet:
                case WorldItemType.Enemy: 
                    if (!touchedThisFrame.Contains(partner))
                    {
                        touchedThisFrame.Add(partner);
                        partner.TouchedThisFrame.Add(this);
                        partner.LivePoints -= this.touchDamage;
                        this.livePoints -= partner.TouchDamage;
                    }
                    break;
                default:
                    break;
            }
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
            if(Keyboard.GetState().IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
            {
                World.WorldList.Add(new Bullet(this.Position, new Vector2(0, -8), 8, 10, 5, new TimeSpan(0, 0, 20), WorldItemType.PlayerBullet));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right) && oldState.IsKeyUp(Keys.Right))
            {
                World.WorldList.Add(new Bullet(this.Position, new Vector2(-8, 0), 8, 10, 5, new TimeSpan(0, 0, 20), WorldItemType.PlayerBullet));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left) && oldState.IsKeyUp(Keys.Left))
            {
                World.WorldList.Add(new Bullet(this.Position, new Vector2(8, 0), 8, 10, 5, new TimeSpan(0, 0, 20), WorldItemType.PlayerBullet));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down))
            {
                World.WorldList.Add(new Bullet(this.Position, new Vector2(0, 8), 8, 10, 5, new TimeSpan(0, 0, 20), WorldItemType.PlayerBullet));
            }
            oldState = Keyboard.GetState();
        }

        public void Death()
        { }

        public void Draw(SpriteBatch sb, Texture2D tex, SpriteFont font)
        { }
    }
}
