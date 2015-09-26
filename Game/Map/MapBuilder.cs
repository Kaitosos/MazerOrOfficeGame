using System;
using System.Collections.Generic;
using System.Threading;

namespace Game
{
    class MapBuilder
    {
        public static void CreateNextMap()
        {
            do
            {
                if(Data.NextMaps.Count < Data.ForwardCreatedMaps)
                {
                    int level = (Data.Levels + Data.NextMaps.Count + 1);
                    int blocks = (Data.BlockPerLevel + (level / Data.LevelTilBlockPerLevelResize) * Data.BlockPerLevelAdd) * level;
                    Data.NextMaps.Enqueue(new Map(DateTime.Now.Second * DateTime.Now.Millisecond * DateTime.Now.Hour, blocks, level));
                }
                else
                    Thread.Sleep(250);
            } while (true);
        }
    }
}
