using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public class World 
    {
        public static List<IWorldItem> WorldList;

        public static void CreateWorld()
        {
            WorldList = new List<IWorldItem>();
        }
        public static void ClearWorld()
        {
            WorldList.Clear();
        }
        public static void EndWorld()
        {
            WorldList = null;
        }

        public static void Update(GameTime gametime)
        {
            #region Update Items (and delete if death)
            List<IWorldItem> del = new List<IWorldItem>();
            foreach(IWorldItem iwi in WorldList)
            {
                iwi.Update(gametime);
                if (iwi.LivePoints <= 0)
                    del.Add(iwi);
            }
            foreach (IWorldItem iwi in del)
                WorldList.Remove(iwi);
            del.Clear();
            #endregion
            #region Collide
            for (int all = 0; all < WorldList.Count; ++all)
            {
                for (int i = all; i < WorldList.Count; ++i)
                {
                    if(WorldList[all].Hitbox.Intersects(WorldList[i].Hitbox))
                    {
                        WorldList[all].Touch(WorldList[i]);
                    }
                }
            }
            #endregion
        }

        public static void Draw(SpriteBatch sb, Texture2D tex, SpriteFont font)
        {
            foreach(IWorldItem iwi in WorldList)
            {
                iwi.Draw(sb, tex, font);
            }
        }
    }
}
