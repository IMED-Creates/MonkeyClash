using System.Collections.Generic;
using UnityEngine;

namespace TowerArena.Upgrades
{
    [CreateAssetMenu(menuName = "TowerArena/Upgrades/Upgrade Path", fileName = "UpgradePath")]
    public class UpgradePathDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private List<UpgradeTierDefinition> tiers;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public IReadOnlyList<UpgradeTierDefinition> Tiers => tiers;
    }
}
