using System;
using Unity.Collections;
using Unity.Netcode;

namespace DataStructure
{
    /// <summary>
    /// Serializable class for storing data about players
    /// </summary>
    public struct PlayerNetworkData : IEquatable<PlayerNetworkData>, INetworkSerializable
    {
        public ulong clientId;
        public int colorId;
        public FixedString64Bytes playerName;

        /// <summary>
        /// Method comparing players id.
        /// </summary>
        /// <param name="other">Id of the player we want to compare to</param>
        /// <returns>Boolean - whether objects are equal, or not</returns>
        public bool Equals(PlayerNetworkData other)
        {
            return clientId == other.clientId && colorId == other.colorId && playerName == other.playerName;
        }

        /// <summary>
        /// Implementation of <c>INetworkSerializable</c> interface. 
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
}
