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

        private Color color;
        private float size;
        private Vector2 position;
        private Rectangle hitbox;
        private int touchDamage;
        private int livePoints;
        private List<IWorldItem> touchedThisFrame;
        private TimeSpan liveTime;
        private WorldItemType type;
        private Field parrent;
        private DateTime lastSpawn;

        public MonsterSpawner(int x, int y, int touchDamage, int livePoints, TimeSpan timeBetweenSpawns, Field parrent)
        {
            this.position = new Vector2(x * Data.BlockSize, y * Data.BlockSize);
            this.hitbox = new Rectangle(x * Data.BlockSize, y * Data.BlockSize, Data.BlockSize, Data.BlockSize);
            this.color = GameColors.EBasic;
            this.touchDamage = touchDamage;
            this.livePoints = livePoints;
            this.liveTime = timeBetweenSpawns;
            this.parrent = parrent;
            this.type = WorldItemType.Spawner;
            this.touchedThisFrame = new List<IWorldItem>();
            this.lastSpawn = DateTime.Now;
        }

        public void Update(GameTime gametime)
        {
            float minPlayerDistance = float.MaxValue;
            foreach(IWorldItem iwi in World.WorldList)
            {
                if(iwi.Type == WorldItemType.Player)
                {
                    float tempdist = Vector2.Distance(this.position,iwi.Position);
                    if(tempdist < minPlayerDistance)
                    {
                        minPlayerDistance = tempdist;
                    }
                }
            }
            if(minPlayerDistance < Data.BufferDistance / 2)
            {
                if(lastSpawn.Subtract(liveTime) > DateTime.Now)
                {
                    //TODO: Spawn Enemy
                }
            }
        }

        public void Touch(IWorldItem partner)
        {
            switch (partner.Type)
            {
                case WorldItemType.Player:
                case WorldItemType.PlayerBullet:
                    if (this.type == WorldItemType.EnemyBullet)
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

        }

        public void Draw(SpriteBatch sb, Texture2D tex, SpriteFont font)
        {
        }
    }
}
