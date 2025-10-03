using TowerArena.Enemies;
using UnityEngine;

namespace TowerArena.Combat
{
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField] private float speed = 8f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private float splashRadius = 0f;

        private DamageContext damageContext;
        private BalloonInstance target;
        private float lifeTimer;

        public void Initialize(DamageContext context, BalloonInstance target)
        {
            damageContext = context;
            this.target = target;
            lifeTimer = lifetime;
            damageContext.SplashRadius = splashRadius;
        }

        private void Update()
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            if (target == null || !target.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            var direction = (target.TargetTransform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target.TargetTransform.position) < 0.1f)
            {
                ResolveHit();
            }
        }

        private void ResolveHit()
        {
            if (damageContext.IsSplash && damageContext.SplashRadius > 0f)
            {
                var hits = Physics.OverlapSphere(transform.position, damageContext.SplashRadius);
                foreach (var collider in hits)
                {
                    if (collider.TryGetComponent<BalloonInstance>(out var balloon))
                    {
                        if (balloon.CanBeTargetedBy(damageContext))
                        {
                            balloon.ApplyDamage(damageContext);
                        }
                    }
                }
            }
            else
            {
                target.ApplyDamage(damageContext);
            }

            Destroy(gameObject);
        }
    }
}
