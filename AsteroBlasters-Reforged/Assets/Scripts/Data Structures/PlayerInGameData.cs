using Unity.Netcode;

namespace DataStructure
{
    /// <summary>
    /// Structure being used for transfering the data between the machines in order to execute impact damage
    /// </summary>
    struct PlayerInGameData : INetworkSerializable
    {
        public ulong Id;
        public float ImpactVelocity;

        /// <summary>
        /// Implementation of <c>INetworkSerializable</c> interface. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializer"></param>
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref ImpactVelocity);
        }
    }
}
