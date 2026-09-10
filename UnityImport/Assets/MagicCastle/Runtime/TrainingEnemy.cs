using UnityEngine;

namespace MagicCastle
{
    /// <summary>Stationary target for proving the cooperative combat loop before adding AI.</summary>
    public sealed class TrainingEnemy : MonoBehaviour
    {
        [SerializeField] private Renderer body;
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material frozenMaterial;
        [SerializeField] private Material defeatedMaterial;
        private TrainingTargetState state = new TrainingTargetState();

        public int Health => state.Health;
        public int MaxHealth => state.MaxHealth;
        public bool Defeated => state.Defeated;
        public string LastHit { get; private set; } = "Land ice, then fire within 2.5 seconds.";

        public void Configure(Renderer targetBody, Material normal, Material frozen, Material defeated)
        {
            body = targetBody;
            normalMaterial = normal;
            frozenMaterial = frozen;
            defeatedMaterial = defeated;
        }

        public bool IsFrozen(double now) => state.IsFrozen(now);

        public AttackResult Hit(long attackId, AttackKind kind, double now)
        {
            AttackResult result = state.Apply(attackId, kind, now);
            if (result.Applied)
                LastHit = result.Shattered ? $"SHATTER! {result.Damage} damage" :
                    $"{kind}: {result.Damage} damage";
            Tick(now);
            return result;
        }

        public void Tick(double now)
        {
            if (body == null)
                return;
            Material desired = state.Defeated ? defeatedMaterial :
                state.IsFrozen(now) ? frozenMaterial : normalMaterial;
            if (desired != null && body.sharedMaterial != desired)
                body.sharedMaterial = desired;
        }

        public void ResetTarget()
        {
            state = new TrainingTargetState();
            LastHit = "Target reset. Land ice, then fire.";
            if (body != null)
                body.sharedMaterial = normalMaterial;
        }
    }
}
