using TowerArena.Cards;
using TowerArena.Combat;
using TowerArena.Enemies;
using TowerArena.Match;
using UnityEngine;

namespace TowerArena.Units
{
    public class ProjectileUnit : UnitBase
    {
        [Header("Projectile Settings")]
        [SerializeField] private ProjectileBase projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private bool canBreakLead;
        [SerializeField] private bool splashDamage;
        [SerializeField] private float splashRadius = 0.5f;

        protected override void TryAttack()
        {
            if (MatchManager == null)
            {
                return;
            }

            var origin = firePoint != null ? firePoint.position : transform.position;
            var target = MatchManager.FindTarget(OwnerState, origin, range, canDetectCamo, targetingPriority);
            if (target == null)
            {
                return;
            }

            FireProjectile(target, origin);
        }

        private void FireProjectile(BalloonInstance target, Vector3 origin)
        {
            if (projectilePrefab == null)
            {
                ApplyDirectDamage(target, origin);
                return;
            }

            var projectile = Object.Instantiate(projectilePrefab, origin, Quaternion.identity, MatchManager.ProjectileContainer);
            var context = CreateDamageContext(origin);
            projectile.Initialize(context, target);
        }

        private void ApplyDirectDamage(BalloonInstance target, Vector3 origin)
        {
            var context = CreateDamageContext(origin);
            if (context.IsSplash && context.SplashRadius > 0f)
            {
                MatchManager.ApplyAreaEffect(origin, context.SplashRadius, balloon => balloon.ApplyDamage(context));
            }
            else
            {
                target.ApplyDamage(context);
            }
        }

        private DamageContext CreateDamageContext(Vector3 origin)
        {
            return new DamageContext
            {
                Damage = damage,
                Pierce = pierce,
                CanHitCamo = canDetectCamo,
                CanBreakLead = canBreakLead,
                IsSplash = splashDamage,
                SplashRadius = splashDamage ? splashRadius : 0f,
                SourceUnit = this,
                Origin = origin
            };
        }
    }
}
