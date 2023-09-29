using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Structure containing the data about player input.
/// Is serializable to make it possible to send it through the network.
/// </summary>
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
