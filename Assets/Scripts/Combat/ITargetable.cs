using UnityEngine;

namespace TowerArena.Combat
{
    public interface ITargetable
    {
        Transform TargetTransform { get; }
        bool IsAlive { get; }
        bool CanBeTargetedBy(DamageContext context);
        void ApplyDamage(DamageContext context);
    }
}
