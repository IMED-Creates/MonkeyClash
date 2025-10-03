using TowerArena.Cards;
using TowerArena.Combat;
using UnityEngine;

namespace TowerArena.Enemies
{
    public class BalloonInstance : MonoBehaviour, ITargetable
    {
        private BalloonType type;
        private float currentHealth;
        private float speed;
        private int pathIndex;
        private Vector3[] path;
        private Match.MatchManager matchManager;
        private Match.PlayerSide targetSide;
        private int laneIndex;
        private bool escaped;
        private float camoRevealTimer;
        private float slowTimer;
        private float slowMultiplier = 1f;

        public BalloonType Type => type;
        public bool IsAlive => currentHealth > 0f && !escaped;

        public Match.PlayerSide TargetSide => targetSide;
        public int LaneIndex => laneIndex;
        public Transform TargetTransform => transform;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => type != null ? type.MaxHealth : 1f;
        public int PathIndex => pathIndex;
        public float PathProgress => pathIndex + (path != null && pathIndex < path.Length
            ? Vector3.Distance(transform.position, path[pathIndex])
            : 0f);
        public bool IsCamoRevealed => camoRevealTimer > 0f;

        public void Initialize(BalloonType balloonType, Vector3[] pathPoints, Match.MatchManager manager, Match.PlayerSide side, int lane)
        {
            type = balloonType;
            path = pathPoints;
            matchManager = manager;
            targetSide = side;
            laneIndex = lane;
            currentHealth = type.MaxHealth;
            speed = type.MovementSpeed;
            pathIndex = 0;
            transform.position = path.Length > 0 ? path[0] : transform.position;
            camoRevealTimer = 0f;
            slowTimer = 0f;
            slowMultiplier = 1f;
        }

        private void Update()
        {
            if (!IsAlive || path == null || path.Length == 0)
            {
                return;
            }

            TickStatusEffects();
            MoveAlongPath();
        }

        private void TickStatusEffects()
        {
            if (camoRevealTimer > 0f)
            {
                camoRevealTimer -= Time.deltaTime;
            }

            if (slowTimer > 0f)
            {
                slowTimer -= Time.deltaTime;
                if (slowTimer <= 0f)
                {
                    slowMultiplier = 1f;
                }
            }
        }

        private void MoveAlongPath()
        {
            if (pathIndex >= path.Length)
            {
                Escape();
                return;
            }

            var target = path[pathIndex];
            var step = speed * slowMultiplier * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                pathIndex++;
            }

            if (pathIndex >= path.Length)
            {
                Escape();
            }
        }

        public bool CanBeTargetedBy(DamageContext context)
        {
            if (!IsAlive)
            {
                return false;
            }

            var isCamo = HasTrait(BalloonTraits.Camo);
            if (isCamo && camoRevealTimer <= 0f && !context.CanHitCamo)
            {
                return false;
            }

            if (HasTrait(BalloonTraits.Lead) && !context.CanBreakLead)
            {
                return false;
            }

            return true;
        }

        public void ApplyDamage(DamageContext context)
        {
            if (!CanBeTargetedBy(context))
            {
                return;
            }

            currentHealth -= Mathf.Max(0f, context.Damage);

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        public bool HasTrait(BalloonTraits trait)
        {
            return (type.Traits & trait) != 0;
        }

        private void Die()
        {
            matchManager.OnBalloonPopped(this);
            Destroy(gameObject);
        }

        private void Escape()
        {
            escaped = true;
            matchManager.OnBalloonEscaped(this);
            Destroy(gameObject);
        }

        public void RevealCamo(float duration)
        {
            if (duration <= 0f)
            {
                return;
            }

            camoRevealTimer = Mathf.Max(camoRevealTimer, duration);
        }

        public void ApplySlow(float multiplier, float duration)
        {
            if (duration <= 0f)
            {
                return;
            }

            slowMultiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
            slowTimer = Mathf.Max(slowTimer, duration);
        }
    }
}
