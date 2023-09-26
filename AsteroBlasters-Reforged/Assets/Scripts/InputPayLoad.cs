using Unity.Netcode;
using UnityEngine;

public struct InputPayLoad : INetworkSerializable
{
    public int tick;

    public bool movePressed;
    public Vector2 rotationVector;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref movePressed);
        serializer.SerializeValue(ref rotationVector);
    }
}
