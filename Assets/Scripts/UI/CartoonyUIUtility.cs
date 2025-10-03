using TowerArena.Art;
using TowerArena.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace TowerArena.UI
{
    public static class CartoonyUIUtility
    {
        public static void ApplyCardTheme(Image background, Image rarityFrame, CardRarity rarity)
        {
            if (background != null)
            {
                background.color = Color.Lerp(CartoonyPalette.BackgroundLight, CartoonyPalette.BackgroundDark, 0.35f);
            }

            if (rarityFrame != null)
            {
                rarityFrame.color = CartoonyPalette.GetRarityColor(rarity);
            }
        }

        public static void ApplyAccent(Image image, Color accent)
        {
            if (image != null)
            {
                image.color = accent;
            }
        }

        public static void ApplyTextTheme(Text text, bool isHeadline)
        {
            if (text == null)
            {
                return;
            }

            text.color = isHeadline ? CartoonyPalette.AccentYellow : Color.white;
            text.fontStyle = isHeadline ? FontStyle.Bold : FontStyle.Normal;
        }
    }
}
