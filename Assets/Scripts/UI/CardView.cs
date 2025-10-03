using System;
using TowerArena.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace TowerArena.UI
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Text nameText;
        [SerializeField] private Text costText;
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private Image rarityFrame;

        private Action<int> clickCallback;
        private int handIndex;

        public CardBase BoundCard { get; private set; }

        private void Awake()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClicked);
            }
        }

        public void Bind(CardBase card, int index, Action<int> onClick)
        {
            BoundCard = card;
            handIndex = index;
            clickCallback = onClick;

            if (nameText != null)
            {
                nameText.text = card != null ? card.DisplayName : string.Empty;
                CartoonyUIUtility.ApplyTextTheme(nameText, true);
            }

            if (costText != null)
            {
                costText.text = card != null ? card.ElixirCost.ToString() : string.Empty;
                CartoonyUIUtility.ApplyTextTheme(costText, false);
            }

            if (iconImage != null)
            {
                iconImage.sprite = card != null ? card.Icon : null;
                iconImage.enabled = iconImage.sprite != null;
            }

            if (card != null)
            {
                CartoonyUIUtility.ApplyCardTheme(background, rarityFrame, card.Rarity);
            }
            else
            {
                if (background != null)
                {
                    background.color = Color.clear;
                }
                if (rarityFrame != null)
                {
                    rarityFrame.color = Color.clear;
                }
            }

            if (button != null)
            {
                button.interactable = card != null && clickCallback != null;
            }
        }

        public void Clear()
        {
            Bind(null, -1, null);
        }

        private void OnButtonClicked()
        {
            clickCallback?.Invoke(handIndex);
        }
    }
}
