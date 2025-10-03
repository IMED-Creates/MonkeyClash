using System.Collections.Generic;
using TowerArena.Abilities;
using TowerArena.Cards;
using TowerArena.Decks;
using TowerArena.Match;
using UnityEngine;
using UnityEngine.UI;

namespace TowerArena.UI
{
    public class MatchHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private Transform handContainer;
        [SerializeField] private CardView cardViewPrefab;
        [SerializeField] private Text elixirText;
        [SerializeField] private Text currencyText;
        [SerializeField] private Text waveText;
        [SerializeField] private Text endpointText;
        [SerializeField] private Transform abilityContainer;
        [SerializeField] private AbilityButton abilityButtonPrefab;

        private readonly List<CardView> cardViews = new();
        private readonly List<AbilityButton> abilityButtons = new();
        private PlayerSide observedSide = PlayerSide.Defender;

        private void OnEnable()
        {
            deckManager.OnHandUpdated += HandleHandUpdated;
            deckManager.OnCardPlayed += HandleCardPlayed;
            matchManager.OnSideDefeated += HandleSideDefeated;
        }

        private void OnDisable()
        {
            deckManager.OnHandUpdated -= HandleHandUpdated;
            deckManager.OnCardPlayed -= HandleCardPlayed;
            matchManager.OnSideDefeated -= HandleSideDefeated;
        }

        private void Update()
        {
            if (matchManager == null)
            {
                return;
            }

            var playerState = observedSide == PlayerSide.Defender ? matchManager.DefenderState : matchManager.AttackerState;
            if (playerState == null)
            {
                return;
            }

            if (elixirText != null)
            {
                elixirText.text = $"Elixir: {playerState.CurrentElixir}/{playerState.Elixir.Max}";
            }

            if (currencyText != null)
            {
                currencyText.text = $"Gold: {playerState.CurrentCurrency}";
            }

            if (endpointText != null)
            {
                endpointText.text = $"Endpoint HP: {playerState.CurrentHealth}/{playerState.MaxHealth}";
            }

            if (waveText != null)
            {
                waveText.text = $"Time: {matchManager.MatchTime:0.0}s";
            }

            RefreshAbilities(playerState);
        }

        public void SetObservedSide(PlayerSide side)
        {
            observedSide = side;
            RefreshHand();
        }

        public void RefreshHand()
        {
            if (deckManager == null)
            {
                return;
            }

            var hand = deckManager.GetHand(observedSide);
            EnsureCardViews(hand.Count);

            for (int i = 0; i < cardViews.Count; i++)
            {
                if (i < hand.Count)
                {
                    cardViews[i].gameObject.SetActive(true);
                    cardViews[i].Bind(hand[i], i, OnCardClicked);
                }
                else
                {
                    cardViews[i].gameObject.SetActive(false);
                }
            }
        }

        private void EnsureCardViews(int targetCount)
        {
            while (cardViews.Count < targetCount)
            {
                var view = Instantiate(cardViewPrefab, handContainer);
                cardViews.Add(view);
            }
        }

        private void HandleHandUpdated(PlayerSide side)
        {
            if (side != observedSide)
            {
                return;
            }

            RefreshHand();
        }

        private void HandleCardPlayed(PlayerSide side, CardBase card)
        {
            if (side != observedSide)
            {
                return;
            }

            RefreshHand();
        }

        private void RefreshAbilities(PlayerMatchState playerState)
        {
            if (abilityContainer == null || abilityButtonPrefab == null || playerState == null)
            {
                return;
            }

            var units = matchManager.DeployedUnits.TryGetValue(observedSide, out var list) ? list : null;
            EnsureAbilityButtons(units?.Count ?? 0);

            for (int i = 0; i < abilityButtons.Count; i++)
            {
                if (units != null && i < units.Count)
                {
                    abilityButtons[i].Bind(units[i], OnAbilityTriggered);
                }
                else
                {
                    abilityButtons[i].gameObject.SetActive(false);
                }
            }
        }

        private void EnsureAbilityButtons(int targetCount)
        {
            while (abilityButtons.Count < targetCount)
            {
                var button = Instantiate(abilityButtonPrefab, abilityContainer);
                abilityButtons.Add(button);
            }
        }

        private void OnAbilityTriggered(UnitAbilityRequest request)
        {
            if (request.Unit == null)
            {
                return;
            }

            var abilityId = request.AbilityId;
            if (string.IsNullOrEmpty(abilityId))
            {
                return;
            }

            var context = new AbilityActivationContext
            {
                TargetPosition = request.TargetPosition,
                UseTargetPosition = request.UseTargetPosition,
                TargetBalloon = request.TargetBalloon,
                UseTargetBalloon = request.UseTargetBalloon
            };

            request.Unit.TryActivateAbility(abilityId, context);
        }

        private void HandleSideDefeated(PlayerSide side)
        {
            if (side == observedSide)
            {
                enabled = false;
            }
        }

        private void OnCardClicked(int handIndex)
        {
            var parameters = new CardPlayParameters
            {
                TargetPosition = Vector3.zero,
                UseTargetPosition = true
            };

            deckManager.TryPlayCard(observedSide, handIndex, parameters);
        }
    }
}
