using TowerArena.Cards;
using UnityEngine;

namespace TowerArena.Spells
{
    public abstract class SpellEffectDefinition : ScriptableObject
    {
        public abstract void Execute(CardPlayContext context, CardPlayParameters parameters);
    }
}
