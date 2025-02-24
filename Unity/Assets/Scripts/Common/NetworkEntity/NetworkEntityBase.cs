using System;
using System.Collections.Generic;
using Unity.Netcode;

public class NetworkEntityBase : NetworkBehaviour
{
    // 客户端/服务端的专属控制器
    public INetworkController sideController { get; protected set; }

    // Key:Enity类型 Value:sideController类型
    public static Dictionary<Type, Type> sideControllerTypeDic = new Dictionary<Type, Type>();

    public void SetSideController(INetworkController sideController)
    {
        this.sideController = sideController;
    }

    public virtual void Init()
    {
        if (sideController == null)
        {
            sideController = (INetworkController)gameObject.AddComponent(sideControllerTypeDic[this.GetType()]);
            sideController.FirstInit();
        }

        sideController.Init();
    }

    public override void OnNetworkSpawn()
    {

        base.OnNetworkSpawn();
        sideController.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        sideController.OnNetworkDespawn();
    }
}