using System.Collections.Generic;
using TowerArena.Units;
using TowerArena.Upgrades;
using UnityEngine;

namespace TowerArena.Cards
{
    [CreateAssetMenu(menuName = "TowerArena/Cards/Unit", fileName = "UnitCard")]
    public class UnitCard : CardBase
    {
        [Header("Unit Reference")]
        [SerializeField] private UnitBase unitPrefab;
        [SerializeField] private List<UpgradePathDefinition> upgradePaths;
        [SerializeField] private int maxParagonCount = 1;

        public UnitBase UnitPrefab => unitPrefab;
        public IReadOnlyList<UpgradePathDefinition> UpgradePaths => upgradePaths;
        public int MaxParagonCount => maxParagonCount;

        public override bool CanPlay(CardPlayContext context, CardPlayParameters parameters)
        {
            if (!parameters.UseTargetPosition)
            {
                return false;
            }

            return base.CanPlay(context, parameters);
        }

        public override void OnPlay(CardPlayContext context, CardPlayParameters parameters)
        {
            base.OnPlay(context, parameters);
            context.MatchManager.SpawnUnit(this, context.PlayerState, parameters.TargetPosition);
        }
    }
}
