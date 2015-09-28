using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
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

        public void Start()
        {
            foreach(Field f in field)
            {
                f.Start();
            }
        }

        public void Draw(SpriteBatch sb, Texture2D tex, bool all, SpriteFont font)
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
                                sb.Draw(tex, f.Hitbox, GameColors.FPath);
                                break;
                            case FieldTypes.Spawn:
                                sb.Draw(tex, f.Hitbox, GameColors.FSpawn);
                                break;
                            case FieldTypes.Destination:
                                sb.Draw(tex, f.Hitbox, GameColors.FDestination);
                                break;
                            case FieldTypes.Helper:
                                sb.Draw(tex, f.Hitbox, GameColors.FHelper);
                                break;
                            case FieldTypes.Energy:
                                sb.Draw(tex, f.Hitbox, GameColors.FEnergy);
                                break;
                            case FieldTypes.Coin:
                                sb.Draw(tex, f.Hitbox, GameColors.FCoin);
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
                                sb.Draw(tex, f.Hitbox, GameColors.FPath);
                                break;
                            case FieldTypes.Spawn:
                                sb.Draw(tex, f.Hitbox, GameColors.FSpawn);
                                break;
                            case FieldTypes.Destination:
                                sb.Draw(tex, f.Hitbox, GameColors.FDestination);
                                break;
                            case FieldTypes.Helper:
                                sb.Draw(tex, f.Hitbox, GameColors.FHelper);
                                break;
                            case FieldTypes.Energy:
                                sb.Draw(tex, f.Hitbox, GameColors.FEnergy);
                                break;
                            case FieldTypes.Coin:
                                sb.Draw(tex, f.Hitbox, GameColors.FCoin);
                                break;
                        }
                    }
                }
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
            int roomDistance = 0;
            bool newBranch = false;
            bool startedBranch = false;
            int maxLenght = 4 + (level / 2);
            if (maxLenght > 16)
                maxLenght = 16;
            int maxBranchSize = size / (level + Data.BranchCount);
            int ConnectionChance = 8 + (size / 2500) + (int)(level * level * 0.01f);//6 + (size / (250 + (level * 10)));
            int RoomCount = level / 5;
            int StartRoomDistance = size / ((RoomCount * 3) + 1);
            int DefaultMinRoomDistance = (size - StartRoomDistance) / ((RoomCount * 2) + 1);
            roomDistance = StartRoomDistance;
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
                    } while (this.surroundingPathDedect(f.X, f.Y) > 12);
                    dir = r.Next(0, 4);
                    newBranch = false;
                    startedBranch = true;
                }
                #endregion
                #region Move Aktual Position and Set
                #region Set Dir
                switch (dir)
                {
                    case 0: //North
                        posY -= 1;
                        break;
                    case 1: //East
                        posX += 1;
                        break;
                    case 2: //South
                        posY += 1;
                        break;
                    case 3: //West
                        posX -= 1;
                        break;
                }
                #endregion
                #region Check Field and Set
                int surr = surroundingPathDedect(posX, posY);
                if (surr <= 2 || startedBranch && surr <= 3)
                {
                    --roomDistance;
                    if (level >= 6 && roomDistance <= 0 && RoomCount > 0)
                    {
                        List<Field> fields = createRoom(posX, posY, dir, 3, 3, i, size, RoomTypes.NormalRoom, new RoomFlags[] { RoomFlags.CrossRoadsBig });
                        if (fields.Count < size - i)
                        {
                            if(fields.Count == 0)
                            {
                                field[i] = new Field(posX, posY);
                                count -= 1;
                                startedBranch = false;
                            }
                            else
                            {
                                field[i] = new Field(posX, posY);
                                foreach (Field f in fields)
                                {
                                    ++i;
                                    field[i] = f;
                                }
                                roomDistance = DefaultMinRoomDistance;
                                newBranch = true;
                                --RoomCount;
                            }
                        }
                    }
                    else
                    {
                        field[i] = new Field(posX, posY);
                        count -= 1;
                        startedBranch = false;
                    }
                }
                else if (surr <= 6)
                {
                    if (r.Next(0, ConnectionChance) == 1)
                        field[i] = new Field(posX, posY);
                    else
                        --i;
                    newBranch = true;
                }
                else
                {
                    --i;
                    newBranch = true;
                }
                #endregion
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
                    } while (surroundingPathDedect(f.X, f.Y) < 13);
                } while (f.SetHelper() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surroundingPathDedect(f.X, f.Y) < 13);
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
                    } while (surroundingPathDedect(f.X, f.Y) < 13);
                } while (f.SetCoin() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surroundingPathDedect(f.X, f.Y) < 13);
                } while (f.SetCoin() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surroundingPathDedect(f.X, f.Y) < 13);
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
                    } while (surroundingPathDedect(f.X, f.Y) < 13);
                } while (f.SetEnergy() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surroundingPathDedect(f.X, f.Y) < 13);
                } while (f.SetEnergy() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surroundingPathDedect(f.X, f.Y) < 13);
                } while (f.SetEnergy() == false);
                do
                {
                    do
                    {
                        f = this.field[r.Next(1, size - 1)];
                    } while (surroundingPathDedect(f.X, f.Y) < 13);
                } while (f.SetEnergy() == false);
                this.EnergyCount += 4;
            }
            #endregion
        }

        private int surroundingPathDedect(int x, int y)
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
            *     -1 -1 UpLeft        +0 -1 Up       +1 -1 UpRight  
            *     -1 +0 Left          +0 +0 Center   +1 +0 Right    
            *     -1 +1 DownLeft      +0 +1 Down     +1 +1 DownRight
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
            *     -1 -1 UpLeft        +0 -1 Up       +1 -1 UpRight  
            *     -1 +0 Left          +0 +0 Center   +1 +0 Right    
            *     -1 +1 DownLeft      +0 +1 Down     +1 +1 DownRight
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
                value++;
            if (downRight)
                value++;
            if (downLeft)
                value++;
            if (upLeft)
                value++;
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
        private List<Field> createRoom(int x, int y, int dir, int hight, int with, int aktualArrayUsage, int maxArrayUsage, RoomTypes type)
        {
            return createRoom(x, y, dir, hight, with, aktualArrayUsage, maxArrayUsage, type, new RoomFlags[] { RoomFlags.None });
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
        private List<Field> createRoom(int x, int y, int dir, int hight, int with, int aktualArrayUsage, int maxArrayUsage, RoomTypes type, RoomFlags[] flags)
        {
            List<Field> value = new List<Field>();
            if (maxArrayUsage - aktualArrayUsage < hight * with)
                return value;
            if (hight <= 2 || with <= 2)
                return value;
            #region example
            /*
             *  New Room; 
             *  UL        UR 
             *  h|FFFFFFFFF
             *  i|FFFFFFFFF
             *  g|FFFFFFFFF
             *  h|FFFFFFFFF
             *  t|FFFFFFFFF
             *   X--------Y
             *  DL        DR
             *     with
             */
            /*    
            *                             Dir 3
            *         x  y                x  y           x  y
            *        -1 -1 UpLeft        +0 -1 Up       +1 -1 UpRight  
            * Dir 2  -1 +0 Left          +0 +0 Center   +1 +0 Right     Dir 4
            *        -1 +1 DownLeft      +0 +1 Down     +1 +1 DownRight
            *                             Dir 1
            */
            #endregion
            Point UpRight, DownRight, UpLelft, DownLeft;
            Point Center;
            #region Set Basics
            #region All = 0
            UpLelft = new Point(x, y);
            UpRight = new Point(x, y);
            DownRight = new Point(x, y);
            DownLeft = new Point(x, y);
            #endregion
            switch (dir)
            {
                case 1:
                    DownLeft = new Point(x - (with / 2), y);
                    DownRight = new Point(DownLeft.X + with, DownLeft.Y);
                    UpLelft = new Point(DownLeft.X, DownLeft.Y - hight);
                    UpRight = new Point(DownLeft.X + with, DownLeft.Y - hight);
                    break;
                case 2:
                    DownLeft = new Point(x, y - (with / 2));
                    DownRight = new Point(DownLeft.X + hight, DownLeft.Y);
                    UpLelft = new Point(DownLeft.X, DownLeft.Y - with);
                    UpRight = new Point(DownLeft.X + with, DownLeft.Y - with);
                    break;
                case 3:
                    DownLeft = new Point(x - (with / 2), y + hight);
                    DownRight = new Point(DownLeft.X + with, DownLeft.Y);
                    UpLelft = new Point(DownLeft.X, DownLeft.Y - hight);
                    UpRight = new Point(DownLeft.X + with, DownLeft.Y - hight);
                    break;
                case 4:
                    DownLeft = new Point(x - hight, y + (with / 2));
                    DownRight = new Point(DownLeft.X + hight, DownLeft.Y);
                    UpLelft = new Point(DownLeft.X, DownLeft.Y - with);
                    UpRight = new Point(DownLeft.X + with, DownLeft.Y - with);
                    break;
                default:
                    DownLeft = new Point(x - (with / 2), y);
                    DownRight = new Point(DownLeft.X + with, DownLeft.Y);
                    UpLelft = new Point(DownLeft.X, DownLeft.Y - hight);
                    UpRight = new Point(DownLeft.X + with, DownLeft.Y - hight);
                    break;
            }
            int XMax = DownRight.X - DownLeft.X;
            int YMax = DownLeft.Y - UpLelft.Y;
            #region Check Region
            if (surroundingPathDedect(UpRight.X, UpRight.Y) >= 10)
                return value;
            if (surroundingPathDedect(DownRight.X, DownRight.Y) >= 10)
                return value;
            if (surroundingPathDedect(UpLelft.X, UpLelft.Y) >= 10)
                return value;
            if (surroundingPathDedect(DownLeft.X, DownLeft.Y) >= 10)
                return value;
            #endregion
            #endregion
            #region Set Fields & Center Point
            for (int X = 0; X < XMax; ++X)
            {
                for (int Y = 0; Y < YMax; ++Y)
                {
                    value.Add(new Field(UpLelft.X + X, UpLelft.Y + Y));
                }
            }
            int centerPos = (value.Count / 2) - ((value.Count + 1) % 2);
            Center = new Point(value[centerPos].X, value[centerPos].Y);
            #endregion
            #region Check Space for Room
            foreach (Field f in value)
            {
                if (surrounding(f.X, f.Y) >= 10)
                {
                    value.Clear();
                    return value;
                }
            }
            #endregion
            #region RoomTypes
            switch (type)
            {
                case RoomTypes.SaveRoom:
                    break;
                case RoomTypes.NormalRoom:
                    break;
                case RoomTypes.MSpawnerRoom:
                    break;
                case RoomTypes.BossRoom:
                    break;
            }
            #endregion
            #region Room Flags
            foreach (RoomFlags rf in flags)
            {
                switch (rf)
                {
                    case RoomFlags.ConectingEdges:
                        #region ConnectiongEdges
                        value.Add(new Field(UpLelft.X - 1, UpLelft.Y));
                        value.Add(new Field(UpLelft.X, UpLelft.Y - 1));
                        value.Add(new Field(DownRight.X - 1, DownRight.Y));
                        value.Add(new Field(DownRight.X, DownRight.Y + 1));
                        value.Add(new Field(UpRight.X + 1, UpRight.Y));
                        value.Add(new Field(UpRight.X, UpRight.Y - 1));
                        value.Add(new Field(DownLeft.X + 1, DownLeft.Y));
                        value.Add(new Field(DownLeft.X, DownLeft.Y + 1));
                        break;
                        #endregion
                    case RoomFlags.CrossRoadsBig:
                        #region CrossRoadsBig
                        for (int i = 0; i < 4; ++i) //dir 3
                        {
                            Field temp = new Field(Center.X, Center.Y - i - (with / 2));
                            if (surrounding(temp.X, temp.Y) < 10)
                                value.Add(temp);
                            else
                                break;
                        }
                        for (int i = 0; i < 4; ++i) //dir 4
                        {
                            Field temp = new Field(Center.X + i + (hight / 2), Center.Y);
                            if (surrounding(temp.X, temp.Y) < 10)
                                value.Add(temp);
                            else
                                break;
                        }
                        for (int i = 0; i < 4; ++i) //dir 1
                        {
                            Field temp = new Field(Center.X, Center.Y + i + (with / 2));
                            if (surrounding(temp.X, temp.Y) < 10)
                                value.Add(temp);
                            else
                                break;
                        }
                        for (int i = 0; i < 4; ++i) //dir 2
                        {
                            Field temp = new Field(Center.X - i - (hight / 2), Center.Y);
                            if (surrounding(temp.X, temp.Y) < 10)
                                value.Add(temp);
                            else
                                break;
                        }
                        break;
                        #endregion
                    case RoomFlags.CrossRoadsSmall:
                        #region CrossRoadsSmall
                        for (int i = 0; i < 2; ++i) //dir 3
                        {
                            Field temp = new Field(Center.X, Center.Y - i - (with / 2));
                            if(surrounding(temp.X,temp.Y) < 10)
                                value.Add(temp);
                            else
                                break;
                        }
                        for (int i = 0; i < 2; ++i) //dir 4
                        {
                            Field temp = new Field(Center.X + i + (hight / 2), Center.Y);
                            if (surrounding(temp.X, temp.Y) < 10)
                                value.Add(temp);
                            else
                                break;
                        }
                        for (int i = 0; i < 2; ++i) //dir 1
                        {
                            Field temp = new Field(Center.X, Center.Y + i + (with / 2));
                            if (surrounding(temp.X, temp.Y) < 10)
                                value.Add(temp);
                            else
                                break;
                        }
                        for (int i = 0; i < 2; ++i) //dir 2
                        {
                            Field temp = new Field(Center.X - i - (hight / 2), Center.Y);
                            if (surrounding(temp.X, temp.Y) < 10)
                                value.Add(temp);
                            else
                                break;
                        }
                        break;
                        #endregion
                    case RoomFlags.HasCoins:
                        #region HasCoins
                        break;
                        #endregion
                    case RoomFlags.HasEnergy:
                        #region HasEnergy
                        break;
                        #endregion
                    case RoomFlags.HasHelper:
                        #region HasHelper
                        break;
                        #endregion
                    case RoomFlags.HasMSpawner:
                        #region HasMSpawner
                        break;
                        #endregion
                    case RoomFlags.Traped:
                        #region Traped
                        break;
                        #endregion
                }
            }
            #endregion
            return value;
        }

    }
}