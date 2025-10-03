using System.Collections.Generic;
using TowerArena.Cards;
using UnityEngine;

namespace TowerArena.Decks
{
    [CreateAssetMenu(menuName = "TowerArena/Decks/Deck", fileName = "DeckDefinition")]
    public class DeckDefinition : ScriptableObject
    {
        [SerializeField] private string deckId;
        [SerializeField] private string displayName;
        [SerializeField] private List<CardBase> cards;

        public string DeckId => deckId;
        public string DisplayName => displayName;
        public IReadOnlyList<CardBase> Cards => cards;
    }
}
