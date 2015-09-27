using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    class Enemy : IWorldItem
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
        private Rectangle hitbox;
        private int touchDamage;
        private int livePoints;
        private List<IWorldItem> touchedThisFrame;
        private TimeSpan liveTime;
        private WorldItemType type;



        public void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }

        public void Touch(IWorldItem partner)
        {
            switch (partner.Type)
            {
                case WorldItemType.Player:
                case WorldItemType.PlayerBullet:
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


        public void Draw(SpriteBatch sb, Texture2D tex, SpriteFont font)
        {
            throw new NotImplementedException();
        }


        public void Death()
        {
            throw new NotImplementedException();
        }
    }
}
