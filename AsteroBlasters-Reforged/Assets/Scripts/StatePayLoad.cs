using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Structure containing the data about player position, rotation, velocity and angular velocity.
/// Is serializable to make it possible to send it through the network.
/// </summary>
public struct StatePayLoad : INetworkSerializable
{
    public int tick;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public float angularVelocity;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref rotation);
        serializer.SerializeValue(ref velocity);
        serializer.SerializeValue(ref angularVelocity);
    }
}
