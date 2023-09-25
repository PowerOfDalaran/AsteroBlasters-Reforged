using UnityEngine;
using Unity.Netcode;

public struct InputPayLoad : INetworkSerializable
{
    public int tick;

    public bool moveBool;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref moveBool);
    }
}
