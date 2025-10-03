using System;

namespace TowerArena.Cards
{
    [Flags]
    public enum BalloonTraits
    {
        None = 0,
        Camo = 1 << 0,
        Lead = 1 << 1,
        Fortified = 1 << 2,
        Boss = 1 << 3,
        Fast = 1 << 4
    }

    public enum CardType
    {
        Unit,
        Spell,
        Support
    }

    public enum CardRarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Mythic
    }

    public enum TargetingPriority
    {
        First,
        Last,
        Strong,
        Weak,
        Close
    }
}
