
using Unity.Netcode;

public abstract class StackableItemDataBase : ItemDataBase
{
    public int count;
    public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        base.NetworkSerialize(serializer);
        serializer.SerializeValue(ref count);
    }
}
