using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public class World 
    {
        public static List<IWorldItem> WorldList;

        public static void CreateWorld();
        public static void ClearWorld();
        public static void EndWorld();

        public static void Update();
        public static void Draw(SpriteBatch sb, Texture2D tex);
    }
}
