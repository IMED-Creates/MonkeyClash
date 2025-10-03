using TowerArena.Units;
using UnityEngine;

namespace TowerArena.Upgrades
{
    [CreateAssetMenu(menuName = "TowerArena/Upgrades/Tier", fileName = "UpgradeTier")]
    public class UpgradeTierDefinition : ScriptableObject
    {
        [SerializeField] private int tier;
        [SerializeField] private int cost;
        [SerializeField] private string upgradeName;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private float damageModifier = 0f;
        [SerializeField] private float attackRateModifier = 0f;
        [SerializeField] private float rangeModifier = 0f;
        [SerializeField] private int pierceModifier = 0;
        [SerializeField] private bool enableCamoDetection;
        [SerializeField] private bool grantAbility;
        [SerializeField] private string abilityId;

        public int Tier => tier;
        public int Cost => cost;
        public string UpgradeName => upgradeName;
        public string Description => description;
        public bool GrantsAbility => grantAbility;
        public string AbilityId => abilityId;

        public virtual void ApplyToUnit(UnitBase unit)
        {
            unit.ModifyDamage(damageModifier);
            unit.ModifyAttackRate(attackRateModifier);
            unit.ModifyRange(rangeModifier);
            unit.ModifyPierce(pierceModifier);
            if (enableCamoDetection)
            {
                unit.EnableCamoDetection();
            }
            if (grantAbility && !string.IsNullOrEmpty(abilityId))
            {
                unit.GrantAbility(abilityId);
            }
        }
    }
}
