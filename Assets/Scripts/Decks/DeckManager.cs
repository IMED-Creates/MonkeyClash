using System;
using System.Collections.Generic;
using TowerArena.Cards;
using TowerArena.Match;
using UnityEngine;

namespace TowerArena.Decks
{
    public class DeckManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private DeckDefinition defenderDeck;
        [SerializeField] private DeckDefinition attackerDeck;

        [Header("Hand Rules")]
        [SerializeField] private int initialHandSize = 4;
        [SerializeField] private int maxHandSize = 6;
        [SerializeField] private float autoDrawInterval = 6f;

        private readonly Dictionary<PlayerSide, DeckRuntimeState> deckStates = new();

        public event Action<PlayerSide, CardBase> OnCardDrawn;
        public event Action<PlayerSide, CardBase> OnCardPlayed;
        public event Action<PlayerSide, CardBase> OnCardDiscarded;
        public event Action<PlayerSide> OnHandUpdated;

        private void Awake()
        {
            if (matchManager == null)
            {
                matchManager = GetComponent<MatchManager>();
            }
        }

        private void Update()
        {
            if (matchManager == null || !matchManager.MatchActive)
            {
                return;
            }

            var delta = Time.deltaTime;
            foreach (var kvp in deckStates)
            {
                var state = kvp.Value;
                state.DrawTimer -= delta;
                if (state.DrawTimer <= 0f)
                {
                    if (DrawCard(state.Side))
                    {
                        state.DrawTimer = autoDrawInterval;
                    }
                    else
                    {
                        // No card drawn; prevent tight loop by resetting timer next frame
                        state.DrawTimer = 1f;
                    }
                }
            }
        }

        public void SetupMatch(MatchManager manager, PlayerMatchState defenderState, PlayerMatchState attackerState)
        {
            matchManager = manager;
            deckStates.Clear();

            deckStates[PlayerSide.Defender] = CreateRuntimeState(PlayerSide.Defender, defenderDeck, defenderState);
            deckStates[PlayerSide.Attacker] = CreateRuntimeState(PlayerSide.Attacker, attackerDeck, attackerState);

            foreach (var state in deckStates.Values)
            {
                for (var i = 0; i < initialHandSize; i++)
                {
                    DrawCard(state.Side, suppressTimerReset: true);
                }
                state.DrawTimer = autoDrawInterval;
            }
        }

        public IReadOnlyList<CardBase> GetHand(PlayerSide side)
        {
            return TryGetState(side, out var state) ? state.Hand : Array.Empty<CardBase>();
        }

        public bool TryPlayCard(PlayerSide side, int handIndex, CardPlayParameters parameters)
        {
            if (!TryGetState(side, out var state))
            {
                return false;
            }

            if (handIndex < 0 || handIndex >= state.Hand.Count)
            {
                return false;
            }

            var card = state.Hand[handIndex];
            var playerState = state.PlayerState;
            var context = new CardPlayContext(playerState, matchManager, this);

            if (!card.CanPlay(context, parameters))
            {
                return false;
            }

            try
            {
                card.Play(context, parameters);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to play card {card.DisplayName}: {ex.Message}");
                return false;
            }

            state.Hand.RemoveAt(handIndex);
            state.DiscardPile.Add(card);
            OnCardPlayed?.Invoke(side, card);
            OnHandUpdated?.Invoke(side);
            return true;
        }

        public bool DrawCard(PlayerSide side, bool suppressTimerReset = false)
        {
            if (!TryGetState(side, out var state))
            {
                return false;
            }

            if (state.Hand.Count >= maxHandSize)
            {
                return false;
            }

            if (state.DrawPile.Count == 0)
            {
                RefillDrawPile(state);
            }

            if (state.DrawPile.Count == 0)
            {
                return false;
            }

            var cardIndex = state.DrawPile.Count - 1;
            var card = state.DrawPile[cardIndex];
            state.DrawPile.RemoveAt(cardIndex);
            state.Hand.Add(card);
            OnCardDrawn?.Invoke(side, card);
            OnHandUpdated?.Invoke(side);

            if (!suppressTimerReset)
            {
                state.DrawTimer = autoDrawInterval;
            }

            return true;
        }

        public void DiscardHand(PlayerSide side)
        {
            if (!TryGetState(side, out var state))
            {
                return;
            }

            foreach (var card in state.Hand)
            {
                state.DiscardPile.Add(card);
                OnCardDiscarded?.Invoke(side, card);
            }
            state.Hand.Clear();
            OnHandUpdated?.Invoke(side);
        }

        private DeckRuntimeState CreateRuntimeState(PlayerSide side, DeckDefinition definition, PlayerMatchState playerState)
        {
            var state = new DeckRuntimeState(side, playerState, autoDrawInterval);

            if (definition == null || definition.Cards == null || definition.Cards.Count == 0)
            {
                Debug.LogWarning($"Deck definition missing for {side}. No cards will be available.");
            }
            else
            {
                state.DrawPile.AddRange(definition.Cards);
            }

            Shuffle(state.DrawPile, state.Random);
            return state;
        }

        private void RefillDrawPile(DeckRuntimeState state)
        {
            if (state.DiscardPile.Count == 0)
            {
                return;
            }

            state.DrawPile.AddRange(state.DiscardPile);
            state.DiscardPile.Clear();
            Shuffle(state.DrawPile, state.Random);
        }

        private static void Shuffle<T>(IList<T> list, System.Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private bool TryGetState(PlayerSide side, out DeckRuntimeState state)
        {
            if (!deckStates.TryGetValue(side, out state))
            {
                Debug.LogWarning($"Deck runtime state not initialized for {side}");
                return false;
            }

            return true;
        }

        private class DeckRuntimeState
        {
            public PlayerSide Side { get; }
            public PlayerMatchState PlayerState { get; }
            public List<CardBase> DrawPile { get; } = new();
            public List<CardBase> Hand { get; } = new();
            public List<CardBase> DiscardPile { get; } = new();
            public float DrawTimer { get; set; }
            public System.Random Random { get; } = new System.Random();

            public DeckRuntimeState(PlayerSide side, PlayerMatchState playerState, float initialTimer)
            {
                Side = side;
                PlayerState = playerState;
                DrawTimer = initialTimer;
            }
        }
    }
}
