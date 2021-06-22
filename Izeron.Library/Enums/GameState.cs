using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Enums
{
    public enum GameState
    {
        InTown,
        Explorirng,
        Fighting,
        Looting,
        BackToTown,
        SellingLoot,
        Healing,
        BuyGears
    }
    public enum GameCommand
    {
        Start,
        End
    }
    public enum GameNotificationState
    {
        All,
        Battle,
        Quest,
        Other
    }
}
