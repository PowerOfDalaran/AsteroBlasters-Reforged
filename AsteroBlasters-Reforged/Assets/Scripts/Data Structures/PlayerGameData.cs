using System;
using Unity.Netcode;

namespace DataStructure
{
    /// <summary>
    /// Serializable class for storing data about players performance in the match
    /// </summary>
    public struct PlayerGameData : IEquatable<PlayerGameData>, INetworkSerializable
    {
        public ulong playerId;
        public int killCount;
        public int deathCount;

        /// <summary>
        /// Method comparing players id.
        /// </summary>
        /// <param name="other">Id of the player we want to compare to</param>
        /// <returns>Boolean - whether objects are equal, or not</returns>
        public bool Equals(PlayerGameData other)
        {
            return playerId == other.playerId && killCount == other.deathCount && deathCount == other.deathCount;
        }

        /// <summary>
        /// Implementation of <c>INetworkSerializable</c> interface. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializer"></param>
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerId);
            serializer.SerializeValue(ref killCount);
            serializer.SerializeValue(ref deathCount);
        }
    }
}
