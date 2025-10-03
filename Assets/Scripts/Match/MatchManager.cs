using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerArena.Abilities;
using TowerArena.Cards;
using TowerArena.Combat;
using TowerArena.Decks;
using TowerArena.Enemies;
using TowerArena.Units;
using UnityEngine;

namespace TowerArena.Match
{
    public class MatchManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private MatchSettings matchSettings;
        [SerializeField] private List<LaneDefinition> lanes;
        [SerializeField] private List<WaveDefinition> waves;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private AbilityCatalog abilityCatalog;

        [Header("Scene References")]
        [SerializeField] private Transform defenderSpawnRoot;
        [SerializeField] private Transform attackerSpawnRoot;
        [SerializeField] private Transform unitContainer;
        [SerializeField] private Transform balloonContainer;
        [SerializeField] private Transform projectileContainer;
        [SerializeField] private UI.PostMatchSummaryUI postMatchSummary;

        private readonly List<BalloonInstance> activeBalloons = new();
        private readonly Dictionary<PlayerSide, List<UnitBase>> deployedUnits = new();

        public PlayerMatchState DefenderState { get; private set; }
        public PlayerMatchState AttackerState { get; private set; }
        public float MatchTime { get; private set; }
        public event Action<PlayerSide> OnSideDefeated;
        public event Action<BalloonInstance> OnBalloonSpawned;

        private Coroutine waveRoutine;
        private bool matchActive;

        public bool MatchActive => matchActive;
        public AbilityCatalog AbilityCatalog => abilityCatalog;

        private void Awake()
        {
            deployedUnits[PlayerSide.Defender] = new List<UnitBase>();
            deployedUnits[PlayerSide.Attacker] = new List<UnitBase>();

            if (deckManager == null)
            {
                deckManager = GetComponent<DeckManager>();
            }
        }

        private void Start()
        {
            InitializeMatch();
        }

        private void InitializeMatch()
        {
            DefenderState = new PlayerMatchState(
                PlayerSide.Defender,
                matchSettings.StartingElixir,
                matchSettings.MaxElixir,
                matchSettings.ElixirRegenRate,
                matchSettings.StartingCurrency,
                matchSettings.CurrencyCap,
                matchSettings.EndpointHealth);

            AttackerState = new PlayerMatchState(
                PlayerSide.Attacker,
                matchSettings.StartingElixir,
                matchSettings.MaxElixir,
                matchSettings.ElixirRegenRate,
                matchSettings.StartingCurrency,
                matchSettings.CurrencyCap,
                matchSettings.EndpointHealth);

            MatchTime = 0f;
            matchActive = true;

            deckManager?.SetupMatch(this, DefenderState, AttackerState);

            if (waveRoutine != null)
            {
                StopCoroutine(waveRoutine);
            }

            waveRoutine = StartCoroutine(RunWaves());
        }

        private void Update()
        {
            if (!matchActive)
            {
                return;
            }

            var deltaTime = Time.deltaTime;
            MatchTime += deltaTime;

            DefenderState.Tick(deltaTime);
            AttackerState.Tick(deltaTime);
        }

        private IEnumerator RunWaves()
        {
            yield return new WaitForSeconds(matchSettings.TimeBeforeFirstWave);

            foreach (var wave in waves.OrderBy(w => w.StartTime))
            {
                var waitTime = Mathf.Max(0f, wave.StartTime - MatchTime);
                if (waitTime > 0f)
                {
                    yield return new WaitForSeconds(waitTime);
                }

                foreach (var entry in wave.Entries)
                {
                    StartCoroutine(SpawnWaveEntry(entry));
                }

                yield return new WaitForSeconds(matchSettings.TimeBetweenWaves);
            }
        }

        private IEnumerator SpawnWaveEntry(WaveEntry entry)
        {
            var lane = lanes.ElementAtOrDefault(entry.LaneIndex);
            if (lane == null || entry.BalloonType == null)
            {
                yield break;
            }

            var pathPoints = lane.PathPoints?.Select(t => t.position).ToArray();
            if (pathPoints == null || pathPoints.Length == 0)
            {
                yield break;
            }

            for (var i = 0; i < entry.Quantity; i++)
            {
                SpawnBalloon(entry.BalloonType, pathPoints, PlayerSide.Defender, entry.LaneIndex);
                yield return new WaitForSeconds(entry.SpawnInterval);
            }
        }

        private void SpawnBalloon(BalloonType type, Vector3[] path, PlayerSide targetSide, int laneIndex)
        {
            var prefab = type.BalloonPrefab != null ? type.BalloonPrefab : CreateFallbackBalloon(type);
            var instance = Instantiate(prefab, balloonContainer);
            if (!instance.TryGetComponent<BalloonInstance>(out var balloon))
            {
                balloon = instance.AddComponent<BalloonInstance>();
            }

            balloon.Initialize(type, path, this, targetSide, laneIndex);
            activeBalloons.Add(balloon);
            OnBalloonSpawned?.Invoke(balloon);
        }

        private GameObject CreateFallbackBalloon(BalloonType type)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = Vector3.one * 0.5f;
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = type.Color;
            }
            go.name = $"Balloon_{type.DisplayName}";
            return go;
        }

        public void SpawnUnit(UnitCard card, PlayerMatchState owner, Vector3 position)
        {
            if (card.UnitPrefab == null)
            {
                Debug.LogWarning($"Unit card {card.DisplayName} has no prefab assigned");
                return;
            }

            var spawnRoot = owner.Side == PlayerSide.Defender ? defenderSpawnRoot : attackerSpawnRoot;
            var spawnPosition = position;
            if (spawnRoot != null)
            {
                spawnPosition += spawnRoot.position;
            }

            var unitInstance = Instantiate(card.UnitPrefab, spawnPosition, Quaternion.identity, unitContainer);
            unitInstance.Initialize(this, owner);
            deployedUnits[owner.Side].Add(unitInstance);
        }

        public void OnBalloonPopped(BalloonInstance balloon)
        {
            activeBalloons.Remove(balloon);
            var reward = balloon.Type.RewardCurrency;
            DefenderState.GainCurrency(reward);
        }

        public void OnBalloonEscaped(BalloonInstance balloon)
        {
            activeBalloons.Remove(balloon);
            ApplyEndpointDamage(balloon.TargetSide, CalculateDamageFromBalloon(balloon.Type));
        }

        private int CalculateDamageFromBalloon(BalloonType type)
        {
            var baseDamage = 1;
            if ((type.Traits & Cards.BalloonTraits.Boss) != 0)
            {
                baseDamage = 5;
            }
            else if ((type.Traits & Cards.BalloonTraits.Fortified) != 0)
            {
                baseDamage = 2;
            }

            return baseDamage;
        }

        private void ApplyEndpointDamage(PlayerSide side, int amount)
        {
            var target = side == PlayerSide.Defender ? DefenderState : AttackerState;
            target.TakeDamage(amount);

            if (target.IsDefeated)
            {
                EndMatch(side);
            }
        }

        private void EndMatch(PlayerSide defeatedSide)
        {
            matchActive = false;
            if (waveRoutine != null)
            {
                StopCoroutine(waveRoutine);
                waveRoutine = null;
            }

            OnSideDefeated?.Invoke(defeatedSide);

            deckManager?.DiscardHand(PlayerSide.Defender);
            deckManager?.DiscardHand(PlayerSide.Attacker);

            if (postMatchSummary != null)
            {
                postMatchSummary.Show(defeatedSide, DefenderState.CurrentHealth, AttackerState.CurrentHealth, MatchTime);
            }
        }

        public IReadOnlyList<LaneDefinition> Lanes => lanes;
        public IReadOnlyDictionary<PlayerSide, List<UnitBase>> DeployedUnits => deployedUnits;

        public IReadOnlyList<BalloonInstance> ActiveBalloons => activeBalloons;
        public Transform ProjectileContainer => projectileContainer != null ? projectileContainer : unitContainer;

        public BalloonInstance FindTarget(PlayerMatchState owner, Vector3 origin, float range, bool canDetectCamo, TargetingPriority priority)
        {
            var sqrRange = range * range;
            var candidates = new List<(BalloonInstance balloon, float sqrDist)>();

            foreach (var balloon in activeBalloons)
            {
                if (balloon == null || !balloon.IsAlive)
                {
                    continue;
                }

                var offset = balloon.transform.position - origin;
                var sqrDistance = offset.sqrMagnitude;
                if (sqrDistance > sqrRange)
                {
                    continue;
                }

                if (!canDetectCamo && balloon.HasTrait(Cards.BalloonTraits.Camo) && !balloon.IsCamoRevealed)
                {
                    continue;
                }

                candidates.Add((balloon, sqrDistance));
            }

            if (candidates.Count == 0)
            {
                return null;
            }

            return priority switch
            {
                TargetingPriority.First => candidates
                    .OrderByDescending(c => c.balloon.PathProgress)
                    .ThenBy(c => c.sqrDist)
                    .Select(c => c.balloon)
                    .FirstOrDefault(),
                TargetingPriority.Last => candidates
                    .OrderBy(c => c.balloon.PathProgress)
                    .ThenBy(c => c.sqrDist)
                    .Select(c => c.balloon)
                    .FirstOrDefault(),
                TargetingPriority.Strong => candidates
                    .OrderByDescending(c => c.balloon.MaxHealth)
                    .ThenBy(c => c.balloon.CurrentHealth)
                    .Select(c => c.balloon)
                    .FirstOrDefault(),
                TargetingPriority.Weak => candidates
                    .OrderBy(c => c.balloon.CurrentHealth)
                    .ThenByDescending(c => c.balloon.PathProgress)
                    .Select(c => c.balloon)
                    .FirstOrDefault(),
                TargetingPriority.Close => candidates
                    .OrderBy(c => c.sqrDist)
                    .Select(c => c.balloon)
                    .FirstOrDefault(),
                _ => candidates
                    .OrderByDescending(c => c.balloon.PathProgress)
                    .Select(c => c.balloon)
                    .FirstOrDefault()
            };
        }

        public void ApplyAreaEffect(Vector3 position, float radius, Action<BalloonInstance> effect)
        {
            if (effect == null)
            {
                return;
            }

            foreach (var balloon in activeBalloons)
            {
                if (balloon == null || !balloon.IsAlive)
                {
                    continue;
                }

                var sqrDist = (balloon.transform.position - position).sqrMagnitude;
                if (sqrDist <= radius * radius)
                {
                    effect(balloon);
                }
            }
        }

        public void ApplyUnitAreaEffect(PlayerSide side, Vector3 position, float radius, Action<UnitBase> effect)
        {
            if (effect == null)
            {
                return;
            }

            if (!deployedUnits.TryGetValue(side, out var units))
            {
                return;
            }

            var radiusSqr = radius * radius;
            foreach (var unit in units)
            {
                if (unit == null)
                {
                    continue;
                }

                var sqrDist = (unit.transform.position - position).sqrMagnitude;
                if (sqrDist <= radiusSqr)
                {
                    effect(unit);
                }
            }
        }
    }
}
