using MongoDB.Bson.Serialization.Attributes;
using Unity.Netcode;
[BsonKnownTypes(typeof(WeaponData), typeof(ConsumableData), typeof(MaterialData))]
public abstract class ItemDataBase : INetworkSerializable
{
    public string id; // 对应物品的ID，同时也是配置在Addressables中的key
    public abstract ItemType GetItemType();
    public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
    }
    public abstract ItemDataBase Copy();
}
