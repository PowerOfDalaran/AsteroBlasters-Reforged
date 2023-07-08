using Unity.Netcode;

struct PlayerInGameData : INetworkSerializable
{
    public ulong Id;
    public float ImpactVelocity;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref ImpactVelocity);
    }
}
