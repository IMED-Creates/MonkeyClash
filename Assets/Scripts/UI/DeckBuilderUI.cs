using System.Collections.Generic;
using TowerArena.Cards;
using TowerArena.Decks;
using UnityEngine;
using UnityEngine.UI;

namespace TowerArena.UI
{
    public class DeckBuilderUI : MonoBehaviour
    {
        [SerializeField] private DeckDefinition deck;
        [SerializeField] private Transform cardListContainer;
        [SerializeField] private CardView cardViewPrefab;
        [SerializeField] private Text deckTitle;
        [SerializeField] private Text deckCount;

        private readonly List<CardView> cardViews = new();

        private void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (deck == null)
            {
                return;
            }

            if (deckTitle != null)
            {
                deckTitle.text = deck.DisplayName;
            }

            var cards = deck.Cards;
            EnsureViews(cards.Count);

            for (int i = 0; i < cardViews.Count; i++)
            {
                if (i < cards.Count)
                {
                    cardViews[i].gameObject.SetActive(true);
                    cardViews[i].Bind(cards[i], i, null);
                }
                else
                {
                    cardViews[i].gameObject.SetActive(false);
                }
            }

            if (deckCount != null)
            {
                deckCount.text = $"Cards: {cards.Count}";
            }
        }

        private void EnsureViews(int count)
        {
            while (cardViews.Count < count)
            {
                var view = Instantiate(cardViewPrefab, cardListContainer);
                cardViews.Add(view);
            }
        }
    }
}
