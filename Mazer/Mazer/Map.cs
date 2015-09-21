using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mazer
{
    class Map
    {
        private Field[] field;
        private List<Field> checkField;
        public int Seed;
        private Random r;
        private Field destination;
        private Field start;
        private int size;
        private int recalcOnpath;
        private Vector2 LastBufferCalcPos;

        public Field Destination { get { return destination; } }
        public bool restart;
        public bool Helper;
        public int HelperCount;
        public bool Coins;
        public int CoinsCount;
        public bool Energy;
        public int EnergyCount;



        public Map(int seed, int size, int level)
        {
            this.r = new Random(seed);
            this.Seed = seed;
            this.field = new Field[size];
            this.size = size;
            this.restart = false;
            this.Coins = false;
            this.Energy = false;
            this.CoinsCount = 0;
            this.EnergyCount = 0;
            this.createMap(level);
            this.checkField = new List<Field>();
            RecalcBuffer();
        }

        public bool OnPath(Rectangle r)
        {
            bool onPath = false;
            List<Field> intersects = new List<Field>();
            recalcOnpath--;
            if (recalcOnpath <= 0 || Vector2.Distance(LastBufferCalcPos, new Vector2(r.X, r.Y)) >= Data.ReBufferTravelDistance)
            {
                recalcOnpath = Data.BufferTime;
                this.checkField.Clear();
                Vector2 playerOriginV2 = new Vector2(r.Center.X, r.Center.Y);
                foreach (Field f in field)
                {
                    if (Vector2.Distance(playerOriginV2, new Vector2(f.Hitbox.Center.X, f.Hitbox.Center.Y)) <= Data.BufferDistance)
                        this.checkField.Add(f);
                }
                LastBufferCalcPos = new Vector2(r.X, r.Y);
            }
            if (r.Intersects(destination.Hitbox))
            {
                restart = true;
                return true;
            }
            foreach (Field f in checkField)
            {
                if (r.Intersects(f.Hitbox))
                {
                    intersects.Add(f);
                    onPath = true;
                }
            }
            this.Helper = false;
            foreach (Field f in intersects)
            {
                if (f.Type == FieldTypes.Helper)
                    this.Helper = true;
                f.Use(this);
            }
            return onPath;
        }

        public void RecalcBuffer()
        {
            this.recalcOnpath = Data.BufferTime;
            Vector2 playerOriginV2 = new Vector2(0, 0);
            foreach (Field f in field)
            {
                if (Vector2.Distance(playerOriginV2, new Vector2(f.Hitbox.Center.X, f.Hitbox.Center.Y)) <= Data.BufferDistance)
                    this.checkField.Add(f);
            }
        }

        private void createMap(int level)
        {
            #region Createsettings & Vars
            int posX = 0;
            int posY = 0;
            int dir = 0;
            int ldir = 0;
            int count = 0;
            bool newBranch = false;
            bool startedBranch = false;
            int maxLenght = 4 + (level / 2);
            if (maxLenght > 16)
                maxLenght = 16;
            int maxBranchSize = size / (level + Data.BranchCount);
            int ConnectionChance = 6 + (size / 100);// +(level / 4);
            #endregion
            this.field[0] = new Field(posX, posY, FieldTypes.Spawn);
            this.start = this.field[0];
            #region set Path
            for (int i = 1; i < size - 1; ++i)
            {
                #region RND Direction
                if (count <= 0)
                {
                    ldir = dir;
                    dir = r.Next(0, 4);
                    while (dir % 2 == ldir % 2)
                    {
                        dir = r.Next(0, 4);
                    }
                    count = r.Next(1, maxLenght);
                }
                #endregion
                #region Create new Branch
                if (i % maxBranchSize == 0 || newBranch)
                {
                    Field f;
                    do
                    {
                        f = this.field[r.Next(0, i)];
                        posX = f.X;
                        posY = f.Y;
                    } while (this.surrounding(f.X, f.Y) > 12);
                    dir = r.Next(0, 4);
                    newBranch = false;
                    startedBranch = true;
                }
                #endregion
                #region Move Aktual Position and Set
                switch (dir)
                {
                    case 0:
                        posY -= 1;
                        break;
                    case 1:
                        posX -= 1;
                        break;
                    case 2:
                        posY += 1;
                        break;
                    case 3:
                        posX += 1;
                        break;
                }
                int surr = surrounding(posX, posY);
                if (surr <= 2 || startedBranch && surr <= 3)
                {
                    field[i] = new Field(posX, posY);
                    count -= 1;
                    startedBranch = false;
                }
                else if (surr <= 6)
                {
                    if (r.Next(0, ConnectionChance) == 1)
                        field[i] = new Field(posX, posY);
                    else
                        i--;
                    newBranch = true;
                }
                else
                {
                    i--;
                    newBranch = true;
                }
                #endregion
            }
            #endregion
            #region Destiantion
            {
                bool set = false;
                while (!set)
                {
                    switch (dir)
                    {
                        case 0:
                            posY -= 1;
                            break;
                        case 1:
                            posX -= 1;
                            break;
                        case 2:
                            posY += 1;
                            break;
                        case 3:
                            posX += 1;
                            break;
                    }
                    bool free = true;
                    foreach (Field f in field)
                    {
                        if (f != null && f.X == posX && f.Y == posY)
                            free = false;
                    }
                    if (free)
                    {
                        set = true;
                        field[size - 1] = new Field(posX, posY, FieldTypes.Destination);
                        destination = field[size - 1];
                    }
                }
            }
            #endregion
            #region set helper
            this.HelperCount = 0;
            for (int i = 0; i < size / 150; ++i)
            {
                Field f;
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surrounding(f.X, f.Y) < 13);
                } while (f.SetHelper() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surrounding(f.X, f.Y) < 13);
                } while (f.SetHelper() == false);
                this.HelperCount += 2;
            }
            #endregion
            #region SetCoins
            for (int i = 0; i < level / 6; ++i)
            {
                this.Coins = true;
                Field f;
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surrounding(f.X, f.Y) < 13);
                } while (f.SetCoin() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surrounding(f.X, f.Y) < 13);
                } while (f.SetCoin() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surrounding(f.X, f.Y) < 13);
                } while (f.SetCoin() == false);
                this.CoinsCount += 3;
            }
            #endregion
            #region Set Energy
            for (int i = 0; i < level / 9; ++i)
            {
                this.Energy = true;
                Field f;
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surrounding(f.X, f.Y) < 13);
                } while (f.SetEnergy() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surrounding(f.X, f.Y) < 13);
                } while (f.SetEnergy() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surrounding(f.X, f.Y) < 13);
                } while (f.SetEnergy() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surrounding(f.X, f.Y) < 13);
                } while (f.SetEnergy() == false);
                this.EnergyCount += 4;
            }
            #endregion
        }

        private int surrounding(int x, int y)
        {
            int value = 0;
            bool up = false;
            bool right = false;
            bool left = false;
            bool down = false;
            bool upRight = false;
            bool downRight = false;
            bool downLeft = false;
            bool upLeft = false;
            /*
             *     -1 -1 UpRight       +0 -1 Up       +1 -1 UpLeft
             *     -1 +0 Right         +0 +0 Center   +1 +0 Left
             *     -1 +1 DownRight     +0 +1 Down     +1 +1 DownLeft
             */
            foreach (Field f in field)
            {
                if (f != null)
                {
                    if (f.X == x + 0 && f.Y == y - 1)
                        up = true;
                    else if (f.X == x + 0 && f.Y == y + 1)
                        down = true;
                    else if (f.X == x - 1 && f.Y == y + 0)
                        right = true;
                    else if (f.X == x + 1 && f.Y == y + 0)
                        left = true;
                    else if (f.X == x - 1 && f.Y == y - 1)
                        upRight = true;
                    else if (f.X == x - 1 && f.Y == y + 1)
                        downRight = true;
                    else if (f.X == x + 1 && f.Y == y + 1)
                        downLeft = true;
                    else if (f.X == x + 1 && f.Y == y - 1)
                        upLeft = true;
                    else if (f.X == x && f.Y == y)
                        value = 10;
                }
            }
            if (up)
                value++;
            if (right)
                value++;
            if (left)
                value++;
            if (down)
                value++;
            if (upRight)
            {
                if (up || right)
                    value++;
                else
                    value += 10;
            }
            if (downRight)
            {
                if (down || right)
                    value++;
                else
                    value += 10;
            }
            if (downLeft)
            {
                if (down || left)
                    value++;
                else
                    value += 10;
            }
            if (upLeft)
            {
                if (up || left)
                    value++;
                else
                    value += 10;
            }
            return value;
        }

        /// <summary>
        /// Calculate a Room, Check if its free, 
        /// </summary>
        /// <param name="x"> start X position</param>
        /// <param name="y"> start Y position</param>
        /// <param name="dir"> direction of the room</param>
        /// <param name="hight">hight of the Room</param>
        /// <param name="with">with of the Room</param>
        /// <param name="aktualArrayUsage">Fields in field</param>
        /// <param name="maxArrayUsage">Max Fields in field</param>
        /// <param name="type">Type of the Room</param>
        /// <returns>A List withe Fields for the Room, empty if imposible</returns>
        private List<Field> createRoom(int x, int y, int dir, int hight, int with,int aktualArrayUsage, int maxArrayUsage, RoomTypes type)
        {
            return createRoom(x,y,dir,hight,with,aktualArrayUsage,maxArrayUsage,type,new RoomFlags[]{RoomFlags.None});
        }
        /// <summary>
        /// Calculate a Room, Check if its free, 
        /// </summary>
        /// <param name="x"> start X position</param>
        /// <param name="y"> start Y position</param>
        /// <param name="dir"> direction of the room</param>
        /// <param name="hight">hight of the Room</param>
        /// <param name="with">with of the Room</param>
        /// <param name="aktualArrayUsage">Fields in field</param>
        /// <param name="maxArrayUsage">Max Fields in field</param>
        /// <param name="type">Type of the Room</param>
        /// <param name="flags">Extraflaggs of the room</param>
        /// <returns>A List withe Fields for the Room, empty if imposible</returns>
        private List<Field> createRoom(int x, int y, int dir, int hight, int with,int aktualArrayUsage, int maxArrayUsage, RoomTypes type, RoomFlags[] flags)
        {
            int startX, startY;
            List<Field> value = new List<Field>();
            if (maxArrayUsage - aktualArrayUsage < hight * with)
                return value;
            switch(dir)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
            return value;
        }

        public void Draw(SpriteBatch sb, Texture2D tex, bool all)
        {
            if (all)
            {
                foreach (Field f in field)
                {
                    if (f != null)
                    {
                        switch (f.Type)
                        {
                            case FieldTypes.Path:
                                sb.Draw(tex, f.Hitbox, Color.Gray);
                                break;
                            case FieldTypes.Spawn:
                                sb.Draw(tex, f.Hitbox, Color.DarkGray);
                                break;
                            case FieldTypes.Destination:
                                sb.Draw(tex, f.Hitbox, Color.LimeGreen);
                                break;
                            case FieldTypes.Helper:
                                sb.Draw(tex, f.Hitbox, Color.Blue);
                                break;
                            case FieldTypes.Energy:
                                sb.Draw(tex, f.Hitbox, Color.CornflowerBlue);
                                break;
                            case FieldTypes.Coin:
                                sb.Draw(tex, f.Hitbox, Color.Gold);
                                break;
                        }
                    }
                }
            }
            else
            {
                foreach (Field f in checkField)
                {
                    if (f != null)
                    {
                        switch (f.Type)
                        {
                            case FieldTypes.Path:
                                sb.Draw(tex, f.Hitbox, Color.Gray);
                                break;
                            case FieldTypes.Spawn:
                                sb.Draw(tex, f.Hitbox, Color.DarkGray);
                                break;
                            case FieldTypes.Destination:
                                sb.Draw(tex, f.Hitbox, Color.LimeGreen);
                                break;
                            case FieldTypes.Helper:
                                sb.Draw(tex, f.Hitbox, Color.Blue);
                                break;
                            case FieldTypes.Energy:
                                sb.Draw(tex, f.Hitbox, Color.CornflowerBlue);
                                break;
                            case FieldTypes.Coin:
                                sb.Draw(tex, f.Hitbox, Color.Gold);
                                break;
                        }
                    }
                }
            }

        }
    }
}
