using TowerArena.Cards;
using TowerArena.Combat;
using UnityEngine;

namespace TowerArena.Spells
{
    [CreateAssetMenu(menuName = "TowerArena/Spells/Area Damage", fileName = "AreaDamageSpell")]
    public class AreaDamageSpell : SpellEffectDefinition
    {
        [SerializeField] private float radius = 3f;
        [SerializeField] private float damage = 5f;
        [SerializeField] private bool canBreakLead;

        public override void Execute(CardPlayContext context, CardPlayParameters parameters)
        {
            if (!parameters.UseTargetPosition)
            {
                Debug.LogWarning("Area damage spell requires target position");
                return;
            }

            var match = context.MatchManager;
            var damageContext = new DamageContext
            {
                Damage = damage,
                Pierce = 999,
                CanHitCamo = true,
                CanBreakLead = canBreakLead,
                IsSplash = true,
                SplashRadius = radius,
                Origin = parameters.TargetPosition
            };

            match.ApplyAreaEffect(parameters.TargetPosition, radius, balloon =>
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
