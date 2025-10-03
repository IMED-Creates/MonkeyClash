using TowerArena.Support;
using UnityEngine;

namespace TowerArena.Cards
{
    [CreateAssetMenu(menuName = "TowerArena/Cards/Support", fileName = "SupportCard")]
    public class SupportCard : CardBase
    {
        [SerializeField] private SupportEffectDefinition supportEffect;

        public SupportEffectDefinition SupportEffect => supportEffect;

        public override void OnPlay(CardPlayContext context, CardPlayParameters parameters)
        {
            base.OnPlay(context, parameters);
            supportEffect?.Apply(context, parameters);
        }
    }
}
