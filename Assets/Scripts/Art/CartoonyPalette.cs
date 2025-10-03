using UnityEngine;

namespace TowerArena.Art
{
    public static class CartoonyPalette
    {
        public static readonly Color BackgroundDark = new(0.11f, 0.15f, 0.28f);
        public static readonly Color BackgroundLight = new(0.23f, 0.33f, 0.57f);
        public static readonly Color AccentYellow = new(1.0f, 0.81f, 0.23f);
        public static readonly Color AccentOrange = new(0.99f, 0.57f, 0.22f);
        public static readonly Color AccentPink = new(0.98f, 0.39f, 0.55f);
        public static readonly Color AccentTeal = new(0.23f, 0.82f, 0.76f);
        public static readonly Color AccentPurple = new(0.58f, 0.38f, 0.94f);
        public static readonly Color AccentGreen = new(0.27f, 0.83f, 0.41f);

        public static Color GetRarityColor(Cards.CardRarity rarity)
        {
            return rarity switch
            {
                Cards.CardRarity.Common => new Color(0.74f, 0.74f, 0.74f),
                Cards.CardRarity.Rare => AccentTeal,
                Cards.CardRarity.Epic => AccentPurple,
                Cards.CardRarity.Legendary => AccentOrange,
                Cards.CardRarity.Mythic => new Color(0.96f, 0.74f, 0.98f),
                _ => Color.white
            };
        }
    }
}
