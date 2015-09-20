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
        public bool helper;
        public int HelperCount;

        public Map(int seed, int size, int level)
        {
            this.r = new Random(seed);
            this.Seed = seed;
            this.field = new Field[size];
            this.size = size;
            this.createMap(level);
            this.restart = false;
            this.checkField = new List<Field>();
            RecalcBuffer();
        }

        public bool OnPath(Rectangle r)
        {
            bool onPath = false;
            List<Field> intersects = new List<Field>();
            recalcOnpath--;
            if (recalcOnpath <= 0 || Vector2.Distance(LastBufferCalcPos,new Vector2(r.X, r.Y)) >= Data.ReBufferTravelDistance)
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
            this.helper = false;
            foreach (Field f in intersects)
            {
                if (f.Type == FieldTypes.Helper)
                    this.helper = true;
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
            Data.BrancCount = 0;
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
            int maxBranchSize = 25 + (level / 3);
            if (maxBranchSize > 50)
                maxBranchSize = 50;
            int ConnectionChance = 12 + (size / 120) + (level * level / 8);
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
                else if(surr <= 5)
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
                    do{
                    f = this.field[r.Next(1, size - 1)];
                    }while(surrounding(f.X, f.Y) < 13);
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
        }

        private int surrounding(int x, int y)
        {
            int value = 0;
            bool up = false;
            bool upRight = false;
            bool right = false;
            bool rightDown = false;
            bool left = false;
            bool downLeft = false;
            bool down = false;
            bool upLeft = false;
            foreach (Field f in field)
            {
                if (f != null)
                {
                    if (f.X == x + 1 && f.Y == y + 0)
                        up = true;
                    else if (f.X == x - 1 && f.Y == y + 0)
                        down = true;
                    else if (f.X == x + 0 && f.Y == y + 1)
                        right = true;
                    else if (f.X == x + 0 && f.Y == y - 1)
                        left = true;
                    else if (f.X == x + 1 && f.Y == y + 1)
                        upRight = true;
                    else if (f.X == x - 1 && f.Y == y + 1)
                        rightDown = true;
                    else if (f.X == x - 1 && f.Y == y + 1)
                        downLeft = true;
                    else if (f.X == x - 1 && f.Y == y - 1)
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
            if (rightDown)
                value++;
            if (downLeft)
                value++;
            if (upLeft)
                value++;
            return value;
        }

        public void Draw(SpriteBatch sb, Texture2D tex,bool all)
        {
            if(all)
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
                        }
                    }
                }
            }
            
        }
    }
}
