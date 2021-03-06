﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Game
{
    class Data
    {
        public static int Death;
        public static int Time;
        public static int Levels;
        public static int Energy;
        public static int Coins;
        public static int TraveldDistance;
        public static int LastSeed;
        public static int Windowsize;
        public static int BufferTime;
        public static int BufferDistance;
        public static int BlockPerLevel;
        public static int BlockSize;
        public static int BranchCount;
        public static int LevelTilBlockPerLevelResize;
        public static int BlockPerLevelAdd;
        public static int ReBufferTravelDistance;
        public static int ForwardCreatedMaps;
        public static int PlayerBulletsize;
        public static int PlayerRelationToBlocksize;
        public static Queue<Map> NextMaps;

        public static void SetNull()
        {
            #region Aktive Values
            Data.Energy = 0;
            Data.Coins = 0;
            Data.Death = 0;
            Data.Time = 0;
            Data.Levels = 0;
            Data.TraveldDistance = 0;
            Data.LastSeed = 0;
            Data.Windowsize = 256;
            Data.BlockPerLevel = 50;
            #endregion
            #region Player Passive Values
            Data.BlockSize = 64;
            Data.BranchCount = 2;
            Data.BufferTime = 250;
            Data.BufferDistance = 1024;
            Data.LevelTilBlockPerLevelResize = 8;
            Data.BlockPerLevelAdd = 25;
            Data.ForwardCreatedMaps = 3;
            Data.PlayerBulletsize = 40;
            Data.ReBufferTravelDistance = BufferDistance - (Windowsize / 2);
            Data.PlayerRelationToBlocksize = 2;
            #endregion
            #region Container
            Data.NextMaps = new Queue<Map>(ForwardCreatedMaps);
            #endregion
        }

        public static void Load()
        {
            if (File.Exists("save.txt"))
            {
                string allS = File.ReadAllText("save.txt");
                string[] singleS = allS.Split(',');
                Death = Convert.ToInt32(singleS[0]);
                Time = Convert.ToInt32(singleS[1]);
                Levels = Convert.ToInt32(singleS[2]);
                TraveldDistance = Convert.ToInt32(singleS[3]);
                LastSeed = Convert.ToInt32(singleS[4]);
                Energy = Convert.ToInt32(singleS[5]);
                Coins = Convert.ToInt32(singleS[6]);
            }
            else
            {
                SetNull();
            }
        }

        public static void Save()
        {
            if(File.Exists("save.txt"))
            {
                File.Delete("save.txt");
            }
            File.WriteAllText("save.txt", Death + "," + Time + "," + Levels + "," + TraveldDistance + "," + LastSeed + "," + Energy + "," + Coins);
        }
    }
}