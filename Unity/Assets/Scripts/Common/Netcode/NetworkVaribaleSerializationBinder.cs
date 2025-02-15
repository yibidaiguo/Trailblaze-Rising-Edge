using System;
using Unity.Collections;
using Unity.Netcode;
public static class NetworkVaribaleSerializationBinder
{
    public static void Init()
    {
        BindUserNetworkVariableSerialization<PlayerState>();
        BindUserNetworkVariableSerialization<MonsterState>();
        BindFixedString32BytesSerialization();
        BindFloatSerialization();
    }

    public static void BindFloatSerialization()
    {
        UserNetworkVariableSerialization<float>.WriteValue = (FastBufferWriter writer, in float value) =>
        {
            writer.WriteValueSafe(value);
        };
        UserNetworkVariableSerialization<float>.ReadValue = (FastBufferReader reader, out float value) =>
        {
            reader.ReadValueSafe(out value);
        };
        UserNetworkVariableSerialization<float>.DuplicateValue = (in float value, ref float duplicateValue) =>
        {
            duplicateValue = value;
        };
    }
    public static void BindFixedString32BytesSerialization()
    {
        UserNetworkVariableSerialization<FixedString32Bytes>.WriteValue = (FastBufferWriter writer, in FixedString32Bytes value) =>
        {
            writer.WriteValueSafe(value);
        };
        UserNetworkVariableSerialization<FixedString32Bytes>.ReadValue = (FastBufferReader reader, out FixedString32Bytes value) =>
        {
            reader.ReadValueSafe(out value);
        };
        UserNetworkVariableSerialization<FixedString32Bytes>.DuplicateValue = (in FixedString32Bytes value, ref FixedString32Bytes duplicateValue) =>
        {
            duplicateValue = value;
        };
    }
    public static void BindUserNetworkVariableSerialization<T>() where T : unmanaged, Enum
    {
        UserNetworkVariableSerialization<T>.WriteValue = (FastBufferWriter writer, in T value) =>
        {
            writer.WriteValueSafe(value);
        };
        UserNetworkVariableSerialization<T>.ReadValue = (FastBufferReader reader, out T value) =>
        {
            reader.ReadValueSafe(out value);
        };
        UserNetworkVariableSerialization<T>.DuplicateValue = (in T value, ref T duplicateValue) =>
        {
            duplicateValue = value;
        };
    }
}