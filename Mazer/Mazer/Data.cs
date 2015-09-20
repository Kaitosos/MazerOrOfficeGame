using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mazer
{
    class Data
    {
        public static int Death;
        public static int Time;
        public static int Levels;
        public static int TraveldDistance;
        public static int LastSeed;
        public static int Windowsize;
        public static int BufferTime;
        public static int BufferDistance;
        public static int BlockPerLevel;
        public static int Size;
        public static int BrancCount;
        public static int ReBufferTravelDistance;
        public static int ForwardCreatedMaps;
        public static Queue<Map> NextMaps;

        public static void SetNull()
        {
            Death = 0;
            Time = 0;
            Levels = 0;
            TraveldDistance = 0;
            LastSeed = 0;
            Windowsize = 256;
            BufferTime = 250;
            BufferDistance = 1024;
            BlockPerLevel = 50;
            Size = 64;
            BrancCount = 0;
            ForwardCreatedMaps = 2;
            ReBufferTravelDistance = BufferDistance - (Windowsize / 2);
            NextMaps = new Queue<Map>(ForwardCreatedMaps);
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
            File.WriteAllText("save.txt", Death + "," + Time + "," + Levels + "," + TraveldDistance + "," + LastSeed);
        }
    }
}
