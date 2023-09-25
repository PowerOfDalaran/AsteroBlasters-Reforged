using System;
using System.Numerics;
using Unity.Netcode;

public struct InputPayload //: INetworkSerializable, IComparable<InputPayload>
{
    public int tick;
    public Vector3 inputVector;

    public int CompareTo(InputPayload other)
    {
        throw new NotImplementedException();
    }

    //public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    //{
    //    serializer.SerializeValue(ref tick);
    //    serializer.SerializeValue(ref inputVector);
    //}
}
