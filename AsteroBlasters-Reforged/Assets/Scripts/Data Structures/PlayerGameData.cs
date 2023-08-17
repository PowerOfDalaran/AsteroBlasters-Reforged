using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace DataStructure
{
    public struct PlayerGameData : IEquatable<PlayerGameData>, INetworkSerializable
    {
        public ulong playerId;
        public int killCount;
        public int deathCount;

        public bool Equals(PlayerGameData other)
        {
            return playerId == other.playerId && killCount == other.deathCount && deathCount == other.deathCount;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerId);
            serializer.SerializeValue(ref killCount);
            serializer.SerializeValue(ref deathCount);
        }
    }
}
