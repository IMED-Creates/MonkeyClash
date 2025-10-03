using System.Collections.Generic;
using UnityEngine;

namespace TowerArena.Abilities
{
    [CreateAssetMenu(menuName = "TowerArena/Abilities/Catalog", fileName = "AbilityCatalog")]
    public class AbilityCatalog : ScriptableObject
    {
        [SerializeField] private List<AbilityDefinition> abilities;
        private Dictionary<string, AbilityDefinition> lookup;

        public AbilityDefinition GetAbility(string abilityId)
        {
            if (string.IsNullOrEmpty(abilityId))
            {
                return null;
            }

            EnsureLookup();
            return lookup.TryGetValue(abilityId, out var ability) ? ability : null;
        }

        private void EnsureLookup()
        {
            if (lookup != null)
            {
                return;
            }

            lookup = new Dictionary<string, AbilityDefinition>();
            if (abilities == null)
            {
                return;
            }

            foreach (var ability in abilities)
            {
                if (ability == null || string.IsNullOrEmpty(ability.AbilityId))
                {
                    continue;
                }

                lookup[ability.AbilityId] = ability;
            }
        }
    }
}
