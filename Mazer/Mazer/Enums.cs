using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mazer
{
    public enum FieldTypes
    {
        Path,
        Button,
        Spawn,
        Destination,
        Trap,
        Death,
        NewGame,
        Helper,
        Coin
    }

    public enum Aktion
    {
        Offpath,
        OnPath,
        Death,
        Finished,
    }
}
