using TowerArena.Spells;
using UnityEngine;

namespace TowerArena.Cards
{
    [CreateAssetMenu(menuName = "TowerArena/Cards/Spell", fileName = "SpellCard")]
    public class SpellCard : CardBase
    {
        [SerializeField] private SpellEffectDefinition spellEffect;

        public SpellEffectDefinition SpellEffect => spellEffect;

        public override void OnPlay(CardPlayContext context, CardPlayParameters parameters)
        {
            base.OnPlay(context, parameters);
            spellEffect?.Execute(context, parameters);
        }
    }
}
