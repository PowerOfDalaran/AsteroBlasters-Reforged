using System;
using Unity.Collections;
using Unity.Netcode;

/// <summary>
/// Serializable class for storing data about players
/// </summary>
public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    public FixedString64Bytes playerName;

    /// <summary>
    /// Method comparing players id.
    /// </summary>
    /// <param name="other">Id of the player we want to compare to</param>
    /// <returns></returns>
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && colorId == other.colorId && playerName == other.playerName;
    }

    /// <summary>
    /// Implementation of <c>INetworkSerializable</c> interface. 
    /// Needs further examination.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
    }
}
