using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Game
{
    class Field
    {
        public int X { get { return this.x; } }
        public int Y { get { return this.y; } }
        public FieldTypes Type { get { return this.type; } }
        public Field Connection { get { return this.connection; } }
        public Rectangle Hitbox { get { return this.hitbox; } }

        private int x,y;
        private FieldTypes type;
        private Field connection;
        private Rectangle hitbox;
        
        public Field(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.type = FieldTypes.Path;
            this.connection = null;
            this.hitbox = new Rectangle(x * Data.Size, y * Data.Size, Data.Size, Data.Size);
        }
        public Field(int x, int y, FieldTypes type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            this.connection = null;
            this.hitbox = new Rectangle(x * Data.Size, y * Data.Size, Data.Size, Data.Size);
        }
        public Field(int x, int y, FieldTypes type, Field Connection)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            this.connection = null;
            this.hitbox = new Rectangle(x * Data.Size, y * Data.Size, Data.Size, Data.Size);
        }

        public void Use(Map parrent)
        {
            switch(this.type)
            {
                case FieldTypes.Button:
                    if (connection != null)
                        this.connection.Use(parrent);
                    break;
                case FieldTypes.Trap:
                    if (connection != null)
                        this.connection.Use(parrent);
                    this.type = FieldTypes.Death;
                    break;
                case FieldTypes.Coin:
                    Data.Coins++;
                    this.type = FieldTypes.Path;
                    break;
                case FieldTypes.Energy:
                    Data.Energy++;
                    this.type = FieldTypes.Path;
                    break;
                case FieldTypes.Destination:
                    this.type = FieldTypes.NewGame;
                    break;
                default:
                    break;
            }
        }
        public bool SetHelper()
        {
            if(type == FieldTypes.Path)
            {
                this.type = FieldTypes.Helper;
                return true;
            }
            return false;
        }
        public bool SetCoin()
        {
            if (type == FieldTypes.Path)
            {
                this.type = FieldTypes.Coin;
                return true;
            }
            return false;
        }
        public bool SetEnergy()
        {
            if (type == FieldTypes.Path)
            {
                this.type = FieldTypes.Energy;
                return true;
            }
            return false;
        }

        public void Start()
        {

        }
    }
}
