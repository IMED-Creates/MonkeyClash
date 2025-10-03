using UnityEngine;

namespace TowerArena.Cards
{
    public abstract class CardBase : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private CardType cardType;
        [SerializeField] private CardRarity rarity;
        [SerializeField] private int elixirCost;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public CardType CardType => cardType;
        public CardRarity Rarity => rarity;
        public int ElixirCost => elixirCost;

        public virtual bool CanPlay(CardPlayContext context, CardPlayParameters parameters)
        {
            return context.PlayerState.CurrentElixir >= elixirCost;
        }

        public virtual void OnPlay(CardPlayContext context, CardPlayParameters parameters)
        {
            context.PlayerState.SpendElixir(elixirCost);
        }

        public bool CanPlay(CardPlayContext context)
        {
            return CanPlay(context, default);
        }

        public void Play(CardPlayContext context, CardPlayParameters parameters)
        {
            if (!CanPlay(context, parameters))
            {
                throw new System.InvalidOperationException($"Cannot play card {displayName}");
            }

            OnPlay(context, parameters);
        }
    }
}
