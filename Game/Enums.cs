using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
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
        Coin,
        Energy,
        Spawner,
        SavePoint
    }

    public enum Aktion
    {
        Offpath,
        OnPath,
        Death,
        Finished,
    }

    public enum RoomTypes
    {
        SaveRoom,
        MSpawnerRoom,
        BossRoom,
        NormalRoom
    }

    public enum RoomFlags
    {
        ConectingEdges,
        CrossRoadsSmall,
        CrossRoadsBig,
        HasCoins,
        HasEnergy,
        Traped,
        HasMSpawner,
        HasHelper,
        None,

    }

    public enum WorldItemType
    {
        Enemy,
        EnemyBullet,
        PlayerBullet
    }
}
