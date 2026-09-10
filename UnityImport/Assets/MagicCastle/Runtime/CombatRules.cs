using System;
using System.Collections.Generic;

namespace MagicCastle
{
    public enum AttackKind { Ice, Fire }

    public static class CombatRules
    {
        public const int IceDamage = 10;
        public const int FireDamage = 25;
        public const int ShatterBonus = 35;
        public const double FreezeSeconds = 2.5;

        public static double CooldownSeconds(AttackKind kind)
        {
            if (kind == AttackKind.Ice) return 0.45;
            if (kind == AttackKind.Fire) return 0.65;
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        internal static void RequireTime(double value)
        {
            if (value < 0 || double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    public readonly struct AttackResult
    {
        public readonly bool Applied;
        public readonly bool Shattered;
        public readonly int Damage;

        public AttackResult(bool applied, bool shattered, int damage)
        {
            Applied = applied;
            Shattered = shattered;
            Damage = damage;
        }
    }

    public sealed class TrainingTargetState
    {
        private readonly HashSet<long> appliedAttacks = new HashSet<long>();
        private double frozenUntil;
        public int MaxHealth { get; }
        public int Health { get; private set; }
        public bool Defeated => Health == 0;

        public TrainingTargetState(int maxHealth = 140)
        {
            if (maxHealth <= 0) throw new ArgumentOutOfRangeException(nameof(maxHealth));
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public bool IsFrozen(double now)
        {
            CombatRules.RequireTime(now);
            return !Defeated && now < frozenUntil;
        }

        public AttackResult Apply(long attackId, AttackKind kind, double now)
        {
            CombatRules.RequireTime(now);
            CombatRules.CooldownSeconds(kind); // Validate before any state is changed.
            if (attackId <= 0) throw new ArgumentOutOfRangeException(nameof(attackId));
            if (Defeated || !appliedAttacks.Add(attackId)) return default;

            bool shatter = kind == AttackKind.Fire && IsFrozen(now);
            int requestedDamage;
            if (kind == AttackKind.Ice)
            {
                requestedDamage = CombatRules.IceDamage;
                frozenUntil = now + CombatRules.FreezeSeconds;
            }
            else
            {
                requestedDamage = CombatRules.FireDamage + (shatter ? CombatRules.ShatterBonus : 0);
                frozenUntil = 0;
            }

            int actualDamage = Math.Min(Health, requestedDamage);
            Health -= actualDamage;
            if (Defeated) frozenUntil = 0;
            return new AttackResult(true, shatter, actualDamage);
        }
    }

    public sealed class AttackCooldown
    {
        private readonly double duration;
        private double nextReadyAt;

        public AttackCooldown(AttackKind kind) => duration = CombatRules.CooldownSeconds(kind);

        public bool TryStart(double now)
        {
            CombatRules.RequireTime(now);
            if (now < nextReadyAt) return false;
            nextReadyAt = now + duration;
            return true;
        }
    }

    public sealed class SimulationClock
    {
        public double Now { get; private set; }

        public void Advance(bool running, double delta)
        {
            CombatRules.RequireTime(delta);
            if (!running) return;
            double next = Now + delta;
            CombatRules.RequireTime(next);
            Now = next;
        }
    }
}
