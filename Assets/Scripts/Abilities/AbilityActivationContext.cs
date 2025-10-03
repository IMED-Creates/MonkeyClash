using TowerArena.Enemies;
using UnityEngine;

namespace TowerArena.Abilities
{
    public struct AbilityActivationContext
    {
        public Vector3 TargetPosition;
        public bool UseTargetPosition;
        public BalloonInstance TargetBalloon;
        public bool UseTargetBalloon;
    }
}
