using System;
using NUnit.Framework;

namespace MagicCastle.Tests
{
    public sealed class PlayerSlotsTests
    {
        [Test]
        public void TwoDistinctDevicesAreRequiredBeforeReady()
        {
            var slots = new PlayerSlots();
            Assert.That(slots.Ready, Is.False);
            Assert.That(slots.TryJoin(11, out int first), Is.True);
            Assert.That(first, Is.Zero);
            Assert.That(slots.Ready, Is.False);
            Assert.That(slots.TryJoin(22, out int second), Is.True);
            Assert.That(second, Is.EqualTo(1));
            Assert.That(slots.Ready, Is.True);
        }

        [Test]
        public void OneDeviceCannotOccupyBothSlots()
        {
            var slots = new PlayerSlots();
            slots.TryJoin(11, out _);
            Assert.That(slots.TryJoin(11, out _), Is.False);
            Assert.That(slots.ConnectedCount, Is.EqualTo(1));
            Assert.That(slots.DeviceId(1), Is.EqualTo(-1));
        }

        [Test]
        public void ThirdDeviceCannotReplaceConnectedPlayers()
        {
            var slots = new PlayerSlots();
            slots.TryJoin(11, out _);
            slots.TryJoin(22, out _);
            Assert.That(slots.TryJoin(33, out int slot), Is.False);
            Assert.That(slot, Is.EqualTo(-1));
            Assert.That(slots.DeviceId(0), Is.EqualTo(11));
            Assert.That(slots.DeviceId(1), Is.EqualTo(22));
        }

        [Test]
        public void DisconnectAndReconnectPreserveRole()
        {
            var slots = new PlayerSlots();
            slots.TryJoin(11, out _);
            slots.TryJoin(22, out _);
            slots.SetConnected(11, false);
            Assert.That(slots.Ready, Is.False);
            slots.SetConnected(11, true);
            Assert.That(slots.Ready, Is.True);
            Assert.That(slots.DeviceId(0), Is.EqualTo(11));
        }

        [Test]
        public void ReplacementClaimsDisconnectedSlotWithoutMovingOtherPlayer()
        {
            var slots = new PlayerSlots();
            slots.TryJoin(11, out _);
            slots.TryJoin(22, out _);
            slots.SetConnected(11, false);
            Assert.That(slots.TryJoin(33, out int slot), Is.True);
            Assert.That(slot, Is.Zero);
            Assert.That(slots.DeviceId(1), Is.EqualTo(22));
            Assert.That(slots.Ready, Is.True);
        }

        [Test]
        public void ReturningOldDeviceCannotReclaimReassignedSlot()
        {
            var slots = new PlayerSlots();
            slots.TryJoin(11, out _);
            slots.TryJoin(22, out _);
            slots.SetConnected(11, false);
            slots.TryJoin(33, out _);
            slots.SetConnected(11, true);
            Assert.That(slots.TryJoin(11, out _), Is.False);
            Assert.That(slots.DeviceId(0), Is.EqualTo(33));
            Assert.That(slots.DeviceId(1), Is.EqualTo(22));
        }

        [Test]
        public void UnknownDeviceCannotMakeEmptySessionReady()
        {
            var slots = new PlayerSlots();
            slots.SetConnected(-1, true);
            slots.SetConnected(42, true);
            Assert.That(slots.ConnectedCount, Is.Zero);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void InvalidDeviceIdsAreRejected(int deviceId)
        {
            var slots = new PlayerSlots();
            Assert.Throws<ArgumentOutOfRangeException>(() => slots.TryJoin(deviceId, out _));
            Assert.That(slots.ConnectedCount, Is.Zero);
        }
    }
}
