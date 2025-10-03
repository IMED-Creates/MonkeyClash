using TowerArena.Cards;
using UnityEngine;

namespace TowerArena.Spells
{
    [CreateAssetMenu(menuName = "TowerArena/Spells/Reveal Camo", fileName = "RevealCamoSpell")]
    public class RevealCamoSpell : SpellEffectDefinition
    {
        [SerializeField] private float radius = 5f;
        [SerializeField] private float revealDuration = 5f;

        public override void Execute(CardPlayContext context, CardPlayParameters parameters)
        {
            if (!parameters.UseTargetPosition)
            {
                Debug.LogWarning("Reveal camo spell requires target position");
                return;
            }

            var match = context.MatchManager;
            match.ApplyAreaEffect(parameters.TargetPosition, radius, balloon =>
            {
                if (balloon == null)
                {
                    return;
                }
                balloon.RevealCamo(revealDuration);
            });
        }
    }
}
