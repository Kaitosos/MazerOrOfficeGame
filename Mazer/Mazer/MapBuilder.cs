﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Mazer
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
                    int blocks = (Data.BlockPerLevel + (level / 10) * 25) * level;
                    Data.NextMaps.Enqueue(new Map(DateTime.Now.Second * DateTime.Now.Millisecond * DateTime.Now.Hour, blocks, level));
                }
                Thread.Sleep(250);
            } while (true);
        }
    }
}
