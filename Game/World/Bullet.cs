using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class Bullet : IWorldItem
    {
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
            get { return this.size; }
        }
        public Vector2 Position
        {
            get { return this.position; }
        }
        public Rectangle Hitbox
        {
            get { return this.hitbox; }
        }
        public int TouchDamage
        {
            get { return this.touchDamage; }
        }
        public int LivePoints
        {
            get
            {
                return this.livePoints;
            }
            set
            {
                this.livePoints = value;
            }
        }
        public List<IWorldItem> TouchedThisFrame
        {
            get { throw new NotImplementedException(); }
        }
        public TimeSpan LiveTime
        {
            get { return this.liveTime; }
        }
        public WorldItemType Type
        {
            get { return this.type; }
        }

        private Color color;
        private float size;
        private Vector2 position;
        private Vector2 direction;
        private Rectangle hitbox;
        private int touchDamage;
        private int livePoints;
        private List<IWorldItem> touchedThisFrame;
        private TimeSpan liveTime;
        private WorldItemType type;

        public Bullet(Vector2 position, Vector2 direction, int size, int touchDamage, int livePoints, TimeSpan liveTime, WorldItemType bulletType)
        {
            this.position = position;
            this.direction = direction;
            this.size = size;
            this.touchDamage = touchDamage;
            this.livePoints = livePoints;
            this.liveTime = liveTime;
            this.type = bulletType;
            switch(bulletType)
            {
                case WorldItemType.EnemyBullet:
                    this.type = bulletType;
                    this.color = GameColors.BEnemy;
                    break;
                case WorldItemType.PlayerBullet:
                    this.type = bulletType;
                    this.color = GameColors.BPlayer;
                    break;
                default:
                    this.type = WorldItemType.Enemy;
                    this.color = Color.Black;
                    this.livePoints = -1;
                    break;
            }
        }

        public void Update(GameTime gametime)
        {
            this.position += direction;
            this.hitbox.X = (int)position.X;
            this.hitbox.Y = (int)position.Y;
        }

        public void Touch(IWorldItem partner)
        {
            switch(partner.Type)
            {
                case WorldItemType.Enemy:
                case WorldItemType.EnemyBullet:
                case WorldItemType.Spawner:
                    if(this.type == WorldItemType.PlayerBullet)
                    {
                        if (!touchedThisFrame.Contains(partner))
                        {
                            touchedThisFrame.Add(partner);
                            partner.TouchedThisFrame.Add(this);
                            partner.LivePoints -= this.touchDamage;
                            this.livePoints -= partner.TouchDamage;
                        }
                    }
                    break;
                case WorldItemType.Player:
                case WorldItemType.PlayerBullet:
                    if(this.type == WorldItemType.EnemyBullet)
                    {
                        if (!touchedThisFrame.Contains(partner))
                        {
                            touchedThisFrame.Add(partner);
                            partner.TouchedThisFrame.Add(this);
                            partner.LivePoints -= this.touchDamage;
                            this.livePoints -= partner.TouchDamage;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void Death()
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch sb, Texture2D tex, SpriteFont font)
        {
            throw new NotImplementedException();
        }
    }
}
