using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public class MonsterSpawner : IWorldItem
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

        public Color color;
        public float size;
        public Vector2 position;
        public Rectangle hitbox;
        public int touchDamage;
        public int livePoints;
        public List<IWorldItem> touchedThisFrame;
        public TimeSpan liveTime;
        public WorldItemType type;
        private Field parrent;

        public void Update(Microsoft.Xna.Framework.GameTime gametime)
        {
            throw new NotImplementedException();
        }

        public void Touch(IWorldItem partner)
        {
            throw new NotImplementedException();
        }

        public void Death()
        {
        }

        public void Draw(SpriteBatch sb, Texture2D tex, SpriteFont font)
        {
        }
    }
}
