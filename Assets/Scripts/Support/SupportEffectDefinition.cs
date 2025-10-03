using TowerArena.Cards;

namespace TowerArena.Support
{
    public abstract class SupportEffectDefinition : UnityEngine.ScriptableObject
    {
        public abstract void Apply(CardPlayContext context, CardPlayParameters parameters);
    }
}
