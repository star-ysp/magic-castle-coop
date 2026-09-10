using System;

namespace MagicCastle
{
    // Retain a disconnected player's slot until that device returns or a replacement joins.
    public sealed class PlayerSlots
    {
        public const int Capacity = 2;
        private readonly int[] deviceIds = { -1, -1 };
        private readonly bool[] connected = new bool[Capacity];

        public int ConnectedCount => (connected[0] ? 1 : 0) + (connected[1] ? 1 : 0);
        public bool Ready => ConnectedCount == Capacity;

        public int DeviceId(int slot) => deviceIds[slot];
        public bool IsConnected(int slot) => connected[slot];

        public bool TryJoin(int deviceId, out int slot)
        {
            if (deviceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(deviceId));

            for (int i = 0; i < Capacity; i++)
            {
                if (deviceIds[i] == deviceId)
                {
                    slot = i;
                    return false;
                }
            }

            for (int i = 0; i < Capacity; i++)
            {
                if (!connected[i])
                {
                    deviceIds[i] = deviceId;
                    connected[i] = true;
                    slot = i;
                    return true;
                }
            }

            slot = -1;
            return false;
        }

        public void SetConnected(int deviceId, bool value)
        {
            if (deviceId <= 0)
                return;
            for (int i = 0; i < Capacity; i++)
                if (deviceIds[i] == deviceId)
                    connected[i] = value;
        }
    }
}
