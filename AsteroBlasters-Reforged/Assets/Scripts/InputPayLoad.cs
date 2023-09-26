using Unity.Netcode;

public struct InputPayLoad : INetworkSerializable
{
    public int tick;

    public bool movePressed;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref movePressed);
    }
}
