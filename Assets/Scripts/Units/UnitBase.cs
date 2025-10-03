using System;
using System.Collections.Generic;
using TowerArena.Abilities;
using TowerArena.Cards;
using TowerArena.Match;
using TowerArena.Upgrades;
using UnityEngine;

#nullable enable

namespace TowerArena.Units
{
    public abstract class UnitBase : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected float attackRate = 1f;
        [SerializeField] protected float range = 3f;
        [SerializeField] protected int pierce = 1;
        [SerializeField] protected bool canDetectCamo = false;

        [Header("Targeting")]
        [SerializeField] protected TargetingPriority targetingPriority = TargetingPriority.First;

        [Header("Abilities")]
        [SerializeField] private List<string> startingAbilities = new();

        private float attackCooldown;
        private readonly Dictionary<string, UpgradePathProgress> upgradeProgress = new();
        private readonly HashSet<string> grantedAbilities = new();
        private readonly Dictionary<string, AbilityRuntime> abilityStates = new();
        private bool isParagon;
        private Match.MatchManager matchManager = null!;
        private Match.PlayerMatchState ownerState = null!;

        public float Damage => damage;
        public float AttackRate => attackRate;
        public float Range => range;
        public int Pierce => pierce;
        public bool CanDetectCamo => canDetectCamo;
        public TargetingPriority Targeting => targetingPriority;
        public bool IsParagon => isParagon;

        public IReadOnlyCollection<string> GrantedAbilities => grantedAbilities;
        public IReadOnlyDictionary<string, AbilityDefinition> AbilityDefinitions => _abilityDefinitionsCache ??= BuildAbilityDefinitions();
        public Match.MatchManager MatchManager => matchManager;
        protected Match.PlayerMatchState OwnerState => ownerState;

        private IReadOnlyDictionary<string, AbilityDefinition>? _abilityDefinitionsCache;

        protected virtual void Awake()
        {
            attackCooldown = 1f / Mathf.Max(attackRate, 0.01f);
        }

        protected virtual void Update()
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                TryAttack();
                attackCooldown = 1f / Mathf.Max(attackRate, 0.01f);
            }

            TickAbilities(Time.deltaTime);
        }

        protected abstract void TryAttack();

        public virtual void Initialize(Match.MatchManager manager, Match.PlayerMatchState owner)
        {
            matchManager = manager;
            ownerState = owner;
            abilityStates.Clear();
            grantedAbilities.Clear();
            _abilityDefinitionsCache = null;

            if (startingAbilities != null)
            {
                foreach (var abilityId in startingAbilities)
                {
                    GrantAbility(abilityId);
                }
            }
        }

        public void ApplyUpgrade(UpgradePathDefinition path, UpgradeTierDefinition tier)
        {
            if (!upgradeProgress.TryGetValue(path.Id, out var progress))
            {
                progress = new UpgradePathProgress(path.Id);
                upgradeProgress[path.Id] = progress;
            }

            progress.ApplyTier(tier);
            tier.ApplyToUnit(this);
        }

        public void SetParagonStats(ParagonStats stats)
        {
            damage = stats.Damage;
            attackRate = stats.AttackRate;
            range = stats.Range;
            pierce = stats.Pierce;
            canDetectCamo = stats.CanDetectCamo;
            isParagon = true;
        }

        public bool CanUpgradePath(string pathId, int nextTier)
        {
            if (!upgradeProgress.TryGetValue(pathId, out var progress))
            {
                return nextTier == 1;
            }

            return progress.CanAdvanceToTier(nextTier);
        }

        public UpgradePathProgress? GetProgress(string pathId)
        {
            if (upgradeProgress.TryGetValue(pathId, out var progress))
            {
                return progress;
            }

            return null;
        }

        public void ModifyDamage(float delta)
        {
            if (Mathf.Approximately(delta, 0f))
            {
                return;
            }
            damage = Mathf.Max(0f, damage + delta);
        }

        public void ModifyAttackRate(float delta)
        {
            if (Mathf.Approximately(delta, 0f))
            {
                return;
            }
            attackRate = Mathf.Max(0.01f, attackRate + delta);
        }

        public void ModifyRange(float delta)
        {
            if (Mathf.Approximately(delta, 0f))
            {
                return;
            }
            range = Mathf.Max(0f, range + delta);
        }

        public void ModifyPierce(int delta)
        {
            if (delta == 0)
            {
                return;
            }
            pierce = Mathf.Max(1, pierce + delta);
        }

        public void EnableCamoDetection()
        {
            canDetectCamo = true;
        }

        public void GrantAbility(string abilityId)
        {
            if (!string.IsNullOrEmpty(abilityId))
            {
                if (grantedAbilities.Add(abilityId))
                {
                    RegisterAbility(abilityId);
                }
            }
        }

        public bool CanActivateAbility(string abilityId)
        {
            if (string.IsNullOrEmpty(abilityId))
            {
                return false;
            }

            return abilityStates.TryGetValue(abilityId, out var runtime) && runtime.CooldownRemaining <= 0f;
        }

        public bool TryActivateAbility(string abilityId, AbilityActivationContext context)
        {
            if (!abilityStates.TryGetValue(abilityId, out var runtime))
            {
                return false;
            }

            if (runtime.CooldownRemaining > 0f)
            {
                return false;
            }

            runtime.Definition.Activate(this, context);
            runtime.CooldownRemaining = runtime.Definition.CooldownSeconds;
            abilityStates[abilityId] = runtime;
            return true;
        }

        private void TickAbilities(float deltaTime)
        {
            if (abilityStates.Count == 0)
            {
                return;
            }

            _abilityDefinitionsCache = null;

            var keys = new List<string>(abilityStates.Keys);
            foreach (var key in keys)
            {
                var runtime = abilityStates[key];
                if (runtime.CooldownRemaining > 0f)
                {
                    runtime.CooldownRemaining = Mathf.Max(0f, runtime.CooldownRemaining - deltaTime);
                    abilityStates[key] = runtime;
                }
            }
        }

        private void RegisterAbility(string abilityId)
        {
            if (matchManager == null)
            {
                return;
            }

            var ability = matchManager.AbilityCatalog?.GetAbility(abilityId);
            if (ability == null)
            {
                return;
            }

            abilityStates[abilityId] = new AbilityRuntime
            {
                Definition = ability,
                CooldownRemaining = 0f
            };
            _abilityDefinitionsCache = null;
        }

        private IReadOnlyDictionary<string, AbilityDefinition> BuildAbilityDefinitions()
        {
            if (abilityStates.Count == 0)
            {
                return new Dictionary<string, AbilityDefinition>();
            }

            var dict = new Dictionary<string, AbilityDefinition>();
            foreach (var kvp in abilityStates)
            {
                dict[kvp.Key] = kvp.Value.Definition;
            }
            return dict;
        }

        private struct AbilityRuntime
        {
            public AbilityDefinition Definition;
            public float CooldownRemaining;
        }
    }
}
