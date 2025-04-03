using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderboardEntitySate : INetworkSerializable, IEquatable<LeaderboardEntitySate>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public int Coins;

    public bool Equals(LeaderboardEntitySate other)
    {
        return ClientId == other.ClientId && PlayerName.Equals(other.PlayerName) && Coins == other.Coins;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Coins);
    }
}
