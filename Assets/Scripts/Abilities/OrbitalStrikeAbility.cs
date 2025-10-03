using TowerArena.Cards;
using TowerArena.Combat;
using TowerArena.Units;
using UnityEngine;

namespace TowerArena.Abilities
{
    [CreateAssetMenu(menuName = "TowerArena/Abilities/Orbital Strike", fileName = "OrbitalStrikeAbility")]
    public class OrbitalStrikeAbility : AbilityDefinition
    {
        [SerializeField] private float radius = 3f;
        [SerializeField] private float damage = 20f;
        [SerializeField] private bool canBreakLead = true;

        public override void Activate(UnitBase unit, AbilityActivationContext context)
        {
            var position = context.UseTargetPosition ? context.TargetPosition : unit.transform.position;
            var match = unit.MatchManager;
            if (match == null)
            {
                return;
            }

            var damageContext = new DamageContext
            {
                Damage = damage,
                Pierce = 999,
                CanHitCamo = true,
                CanBreakLead = canBreakLead,
                IsSplash = true,
                SplashRadius = radius,
                SourceUnit = unit,
                Origin = position
            };

            match.ApplyAreaEffect(position, radius, balloon =>
            {
                if (!balloon.CanBeTargetedBy(damageContext))
                {
                    return;
                }
                balloon.ApplyDamage(damageContext);
            });
        }
    }
}
