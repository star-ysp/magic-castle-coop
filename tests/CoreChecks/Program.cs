using System;
using MagicCastle;

internal static class Program
{
    private static int passed;
    private static int failed;

    private static int Main()
    {
        Check("two devices required", () =>
        {
            var slots = new PlayerSlots();
            Require(!slots.Ready);
            Require(slots.TryJoin(1, out int first) && first == 0 && !slots.Ready);
            Require(slots.TryJoin(2, out int second) && second == 1 && slots.Ready);
        });
        Check("duplicate device rejected", () =>
        {
            var slots = new PlayerSlots();
            slots.TryJoin(1, out _);
            Require(!slots.TryJoin(1, out _) && slots.ConnectedCount == 1);
        });
        Check("third device cannot steal slot", () =>
        {
            var slots = ReadySlots();
            Require(!slots.TryJoin(3, out _) && slots.DeviceId(0) == 1 && slots.DeviceId(1) == 2);
        });
        Check("disconnect and reconnect preserve role", () =>
        {
            var slots = ReadySlots();
            slots.SetConnected(1, false);
            Require(!slots.Ready);
            slots.SetConnected(1, true);
            Require(slots.Ready && slots.DeviceId(0) == 1);
        });
        Check("replacement cannot be displaced by returning old device", () =>
        {
            var slots = ReadySlots();
            slots.SetConnected(1, false);
            Require(slots.TryJoin(3, out int slot) && slot == 0);
            slots.SetConnected(1, true);
            Require(!slots.TryJoin(1, out _) && slots.DeviceId(0) == 3 && slots.DeviceId(1) == 2);
        });
        Check("unknown devices cannot ready empty session", () =>
        {
            var slots = new PlayerSlots();
            slots.SetConnected(-1, true);
            slots.SetConnected(42, true);
            Throws<ArgumentOutOfRangeException>(() => slots.TryJoin(0, out _));
            Require(slots.ConnectedCount == 0);
        });
        Check("fire deals ordinary damage", () =>
        {
            var target = new TrainingTargetState();
            AttackResult hit = target.Apply(1, AttackKind.Fire, 0);
            Require(hit.Applied && !hit.Shattered && hit.Damage == 25 && target.Health == 115);
        });
        Check("ice deals damage and freezes", () =>
        {
            var target = new TrainingTargetState();
            AttackResult hit = target.Apply(1, AttackKind.Ice, 0);
            Require(hit.Damage == 10 && target.Health == 130 && target.IsFrozen(2.49));
        });
        Check("fire consumes freeze exactly once", () =>
        {
            var target = new TrainingTargetState();
            target.Apply(1, AttackKind.Ice, 0);
            AttackResult first = target.Apply(2, AttackKind.Fire, 0.5);
            AttackResult second = target.Apply(3, AttackKind.Fire, 0.5);
            Require(first.Shattered && first.Damage == 60);
            Require(!second.Shattered && second.Damage == 25 && target.Health == 45);
        });
        Check("duplicate collision does not damage twice", () =>
        {
            var target = new TrainingTargetState();
            target.Apply(1, AttackKind.Fire, 0);
            Require(!target.Apply(1, AttackKind.Fire, 1).Applied && target.Health == 115);
        });
        Check("duplicate ice cannot refresh freeze", () =>
        {
            var target = new TrainingTargetState();
            target.Apply(1, AttackKind.Ice, 0);
            Require(!target.Apply(1, AttackKind.Ice, 2).Applied);
            Require(!target.IsFrozen(2.5));
        });
        Check("freeze expires at exact boundary", () =>
        {
            var target = new TrainingTargetState();
            target.Apply(1, AttackKind.Ice, 0);
            AttackResult hit = target.Apply(2, AttackKind.Fire, 2.5);
            Require(!hit.Shattered && hit.Damage == 25);
        });
        Check("new ice hit refreshes freeze", () =>
        {
            var target = new TrainingTargetState();
            target.Apply(1, AttackKind.Ice, 0);
            target.Apply(2, AttackKind.Ice, 2);
            Require(target.Apply(3, AttackKind.Fire, 4).Shattered);
        });
        Check("overkill clamps health and dead targets ignore hits", () =>
        {
            var target = new TrainingTargetState(5);
            Require(target.Apply(1, AttackKind.Fire, 0).Damage == 5);
            Require(target.Defeated && target.Health == 0 && !target.IsFrozen(0));
            Require(!target.Apply(2, AttackKind.Ice, 1).Applied);
        });
        Check("new encounter clears damage and deduplication state", () =>
        {
            var oldTarget = new TrainingTargetState(5);
            oldTarget.Apply(1, AttackKind.Fire, 0);
            var freshTarget = new TrainingTargetState();
            Require(freshTarget.Apply(1, AttackKind.Ice, 1).Applied && freshTarget.Health == 130);
        });
        Check("invalid attack ID is rejected without mutation", () =>
        {
            var target = new TrainingTargetState();
            Throws<ArgumentOutOfRangeException>(() => target.Apply(0, AttackKind.Ice, 0));
            Require(target.Health == 140);
        });
        Check("invalid attack kind does not consume ID", () =>
        {
            var target = new TrainingTargetState();
            Throws<ArgumentOutOfRangeException>(() => target.Apply(1, (AttackKind)999, 0));
            Require(target.Apply(1, AttackKind.Ice, 0).Applied);
        });
        Check("nonfinite time is rejected without damage", () =>
        {
            var target = new TrainingTargetState();
            Throws<ArgumentOutOfRangeException>(() => target.Apply(1, AttackKind.Fire, double.NaN));
            Throws<ArgumentOutOfRangeException>(() => target.Apply(1, AttackKind.Fire, double.PositiveInfinity));
            Require(target.Health == 140);
        });
        Check("cooldown prevents spam and allows exact ready time", () =>
        {
            var cooldown = new AttackCooldown(AttackKind.Fire);
            Require(cooldown.TryStart(0));
            Require(!cooldown.TryStart(0) && !cooldown.TryStart(0.649));
            Require(cooldown.TryStart(0.65));
        });
        Check("pause preserves freeze and cooldown", () =>
        {
            var clock = new SimulationClock();
            var target = new TrainingTargetState();
            var cooldown = new AttackCooldown(AttackKind.Fire);
            target.Apply(1, AttackKind.Ice, clock.Now);
            cooldown.TryStart(clock.Now);
            clock.Advance(false, 30);
            Require(clock.Now == 0 && target.IsFrozen(clock.Now) && !cooldown.TryStart(clock.Now));
            clock.Advance(true, 1);
            Require(cooldown.TryStart(clock.Now) && target.Apply(2, AttackKind.Fire, clock.Now).Shattered);
        });
        Check("invalid clock step leaves time unchanged", () =>
        {
            var clock = new SimulationClock();
            Throws<ArgumentOutOfRangeException>(() => clock.Advance(true, -1));
            Throws<ArgumentOutOfRangeException>(() => clock.Advance(false, double.NaN));
            Require(clock.Now == 0);
        });

        Console.WriteLine($"Core checks: {passed} passed, {failed} failed. Unity integration is not tested by this runner.");
        return failed == 0 ? 0 : 1;
    }

    private static PlayerSlots ReadySlots()
    {
        var slots = new PlayerSlots();
        slots.TryJoin(1, out _);
        slots.TryJoin(2, out _);
        return slots;
    }

    private static void Require(bool condition)
    {
        if (!condition) throw new InvalidOperationException("Expected behavior was not observed.");
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}.");
    }

    private static void Check(string name, Action action)
    {
        try { action(); passed++; Console.WriteLine($"PASS {name}"); }
        catch (Exception error) { failed++; Console.Error.WriteLine($"FAIL {name}: {error}"); }
    }
}
