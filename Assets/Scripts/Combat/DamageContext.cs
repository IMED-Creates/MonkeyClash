using TowerArena.Cards;
using UnityEngine;

namespace TowerArena.Combat
{
    public struct DamageContext
    {
        public float Damage;
        public int Pierce;
        public bool CanHitCamo;
        public bool CanBreakLead;
        public bool IsSplash;
        public float SplashRadius;
        public Units.UnitBase SourceUnit;
        public Vector3 Origin;
    }
}
