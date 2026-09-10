using System.Threading;
using UnityEngine;

namespace MagicCastle
{
    /// <summary>One automatically aimed target; physical walls block attacks, teammates do not.</summary>
    public sealed class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private AttackKind kind;
        [SerializeField] private TrainingEnemy target;
        [SerializeField] private LineRenderer beam;
        private AttackCooldown cooldown;
        private static long nextAttackId;
        private float hideBeamAt;

        public string Feedback { get; private set; } = "Ready";
        public float Range => kind == AttackKind.Ice ? 8f : 2.8f;

        public void Configure(AttackKind role, TrainingEnemy trainingTarget, LineRenderer attackBeam)
        {
            kind = role;
            target = trainingTarget;
            beam = attackBeam;
            cooldown = new AttackCooldown(kind);
        }

        private void Awake()
        {
            cooldown = new AttackCooldown(kind);
            if (beam != null)
                beam.enabled = false;
        }

        public void TryAttack(double now)
        {
            if (target == null || target.Defeated)
            {
                Feedback = "Target defeated: reset to practice again";
                return;
            }

            Vector3 origin = transform.position;
            Vector3 destination = target.transform.position;
            Vector3 direction = destination - origin;
            float distance = direction.magnitude;
            if (distance > Range || distance < 0.001f)
            {
                Feedback = $"Move closer (range {Range:0.0})";
                return;
            }

            // RaycastAll is deliberately used only on attack presses in this small prototype.
            // Ignore both player bodies; choose the nearest remaining collider, never shoot through walls.
            RaycastHit[] hits = Physics.RaycastAll(origin, direction / distance, distance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Collider nearest = null;
            float nearestDistance = float.PositiveInfinity;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponentInParent<LocalPlayerController>() != null)
                    continue;
                if (hit.distance < nearestDistance)
                {
                    nearest = hit.collider;
                    nearestDistance = hit.distance;
                }
            }
            if (nearest == null || nearest.GetComponentInParent<TrainingEnemy>() != target)
            {
                Feedback = "Blocked: move around the obstacle";
                return;
            }
            if (!cooldown.TryStart(now))
            {
                Feedback = "Cooling down";
                return;
            }

            AttackResult result = target.Hit(Interlocked.Increment(ref nextAttackId), kind, now);
            Feedback = result.Shattered ? $"Shatter: {result.Damage}" : $"Hit: {result.Damage}";
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);
            if (beam != null)
            {
                beam.SetPosition(0, origin);
                beam.SetPosition(1, destination);
                beam.enabled = true;
                hideBeamAt = Time.unscaledTime + 0.12f;
            }
        }

        private void Update()
        {
            // Only the cosmetic flash uses real time. Damage, freeze and cooldown use the session clock.
            if (beam != null && beam.enabled && Time.unscaledTime >= hideBeamAt)
                beam.enabled = false;
        }

        private void OnDisable()
        {
            if (beam != null)
                beam.enabled = false;
        }
    }
}
