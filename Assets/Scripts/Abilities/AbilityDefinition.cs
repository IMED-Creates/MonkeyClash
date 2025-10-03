using TowerArena.Units;
using UnityEngine;

namespace TowerArena.Abilities
{
    public abstract class AbilityDefinition : ScriptableObject
    {
        [SerializeField] private string abilityId;
        [SerializeField] private string displayName;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private float cooldownSeconds = 15f;
        [SerializeField] private bool requiresTargetPosition;
        [SerializeField] private bool requiresTargetBalloon;

        public string AbilityId => abilityId;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public float CooldownSeconds => Mathf.Max(0f, cooldownSeconds);
        public bool RequiresTargetPosition => requiresTargetPosition;
        public bool RequiresTargetBalloon => requiresTargetBalloon;

        public abstract void Activate(UnitBase unit, AbilityActivationContext context);
    }
}
