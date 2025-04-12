using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

/*public struct NetworkArrayParam<ArrT> : INetworkSerializable where ArrT : INetworkSerializable
{
    public NetworkArrayParam(ArrT[] arr)
    {
        this.arr = arr;
    }
    ArrT[] arr;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
            fastBufferWriter.WriteValueSafe(arr.Length);

            for (int i = 0; i < arr.Length; i++)
            {
                fastBufferWriter.WriteValueSafe(arr[i]);
            }
        }

        if (serializer.IsReader)
        {
            FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
            fastBufferReader.ReadValueSafe(out int length);

            arr = new ArrT[length];

            for (int i = 0; i < length; i++)
            {
                fastBufferReader.ReadValueSafe(out arr[i]);
            }
        }
    }
    public ArrT[] array
    {
        get => arr;
        set => arr = value;
    }
}
*/