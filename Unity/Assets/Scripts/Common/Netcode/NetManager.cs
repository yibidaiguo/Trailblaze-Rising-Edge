using JKFrame;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetManager : NetworkManager
{
    public static NetManager Instance { get; private set; }
    public UnityTransport unityTransport { get; private set; }
    public NetMessageManager netMessageManager { get; private set; }
    private Dictionary<GameObject, NetworkPrefabInstanceHandler> prefabHandlerDic;
    public void FirstInit()
    {
        Instance = this;
        unityTransport = GetComponent<UnityTransport>();
        netMessageManager = GetComponent<NetMessageManager>();
        prefabHandlerDic = new Dictionary<GameObject, NetworkPrefabInstanceHandler>(NetworkConfig.Prefabs.Prefabs.Count);
        foreach (NetworkPrefab item in NetworkConfig.Prefabs.Prefabs)
        {
            NetworkPrefabInstanceHandler handler = new NetworkPrefabInstanceHandler(item.Prefab);
            prefabHandlerDic.Add(item.Prefab, handler);
            PrefabHandler.AddHandler(item.Prefab, handler);
        }
    }

    public bool InitClient()
    {
        bool result = StartClient();
        netMessageManager.Init();
        return result;
    }
    public void StopClient()
    {
        Shutdown();
    }
    public void InitServer()
    {
        StartServer();
        netMessageManager.Init();
    }

    public T SpawnObject<T>(ulong clientID, GameObject prefab, Vector3 position, Quaternion rotation) where T : INetworkController
    {
        NetworkObject networkObject = prefabHandlerDic[prefab].Instantiate(clientID, position, rotation);
        T controller = networkObject.GetComponent<T>();
        //networkObject.SpawnWithOwnership(clientID);
        //networkObject.NetworkShow(clientID);
        return controller;
    }

    //public NetworkObject SpawnObject(ulong clientID, GameObject prefab, Vector3 position, Quaternion rotation)
    //{
    //    NetworkObject networkObject = prefabHandlerDic[prefab].Instantiate(clientID, position, rotation);
    //    networkObject.SpawnWithOwnership(clientID);
    //    networkObject.NetworkShow(clientID);
    //    return networkObject;
    //}

    public void DestroyObject(NetworkObject networkObject)
    {
        if (networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
        else
        {
            networkObject.GameObjectPushPool();
        }
    }
}
