using System.Collections;
using TowerArena.Cards;
using TowerArena.Match;
using TowerArena.Units;
using UnityEngine;

namespace TowerArena.Support
{
    [CreateAssetMenu(menuName = "TowerArena/Support/Zone Buff", fileName = "SupportZoneEffect")]
    public class SupportZoneEffect : SupportEffectDefinition
    {
        [SerializeField] private float radius = 4f;
        [SerializeField] private float damageBuff = 1f;
        [SerializeField] private float attackRateBuff = -0.2f;
        [SerializeField] private float duration = 10f;

        public override void Apply(CardPlayContext context, CardPlayParameters parameters)
        {
            if (!parameters.UseTargetPosition)
            {
                Debug.LogWarning("Support zone effect requires target position");
                return;
            }

            var match = context.MatchManager;
            var position = parameters.TargetPosition;
            var side = context.PlayerState.Side;

            match.ApplyUnitAreaEffect(side, position, radius, unit =>
            {
                if (unit == null)
                {
                    return;
                }

                unit.StartCoroutine(ApplyBuff(unit));
            });
        }

        private IEnumerator ApplyBuff(UnitBase unit)
        {
            unit.ModifyDamage(damageBuff);
            unit.ModifyAttackRate(attackRateBuff);

            yield return new WaitForSeconds(duration);

            unit.ModifyDamage(-damageBuff);
            unit.ModifyAttackRate(-attackRateBuff);
        }
    }
}
