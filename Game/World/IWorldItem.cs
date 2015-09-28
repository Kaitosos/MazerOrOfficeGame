using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public interface IWorldItem
    {
        Color Color { get; set; }
        float Size { get; }
        Vector2 Position { get; }
        Rectangle Hitbox { get; }
        int TouchDamage { get; }
        int LivePoints { get; set; }
        List<IWorldItem> TouchedThisFrame { get; }
        TimeSpan LiveTime { get; }
        WorldItemType Type { get; }
        
        void Update(GameTime gametime);

        void Touch(IWorldItem partner);

        void Death();

        void Draw(SpriteBatch sb, Texture2D tex, SpriteFont font);


    }
}
