using Unity.Netcode;

public class NetVaribale<T> : NetworkVariable<T>
{
    public NetVaribale(T value = default,
    NetworkVariableReadPermission readPerm = DefaultReadPerm,
    NetworkVariableWritePermission writePerm = DefaultWritePerm)
    : base(value, readPerm, writePerm)
    {
    }
    public override bool IsDirty()
    {
        // 本项目中客户端没有修改网络变量的权力，所以直接过滤且避免(NetworkVariableSerialization<T>.AreEqual为null的情况
        if (NetworkVariableSerialization<T>.AreEqual == null)
        {
            return false;
        }
        return base.IsDirty();
    }
}