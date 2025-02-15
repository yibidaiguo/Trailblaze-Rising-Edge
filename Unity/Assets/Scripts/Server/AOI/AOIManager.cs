using JKFrame;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AOIManager : SingletonMono<AOIManager>
{
    private readonly static Vector2Int defaultCoord;
    [SerializeField] private float chunkSize = 50;
    [SerializeField] private int visualChunkRange = 1;// 如果是1就是刚好周围一圈，九宫格
    // <chunkCoord,clinetIDs>
    private Dictionary<Vector2Int, HashSet<ulong>> chunkClientDic = new Dictionary<Vector2Int, HashSet<ulong>>();
    // <chunkCoord,serverObjectIDs>
    private Dictionary<Vector2Int, HashSet<NetworkObject>> chunkServerObjectDic = new Dictionary<Vector2Int, HashSet<NetworkObject>>();

    static AOIManager()
    {
        defaultCoord = new Vector2Int(int.MinValue, int.MinValue);
    }

    public Vector2Int GetCoordByWorldPostion(Vector3 worldPostion)
    {
        return new Vector2Int((int)(worldPostion.x / chunkSize), (int)(worldPostion.z / chunkSize));
    }

    public void Init()
    {
    }


    #region Client

    public void InitClient(ulong clientID, Vector2Int chunkCoord)
    {
        UpdateClientChunkCoord(clientID, defaultCoord, chunkCoord);
    }

    // 更新玩家在AOI地图块上的坐标
    public void UpdateClientChunkCoord(ulong clientID, Vector2Int oldCoord, Vector2Int newCoord)
    {
        if (oldCoord == newCoord) return;
        // 从旧的地图块中移除
        RemoveClient(clientID, oldCoord);

        // 是否跨地图块移动
        if (Vector2Int.Distance(oldCoord, newCoord) > 1.5f) // 超过单个格子移动的极限距离 所以是传送性质的位移
        {
            for (int x = -visualChunkRange; x <= visualChunkRange; x++)
            {
                for (int y = -visualChunkRange; y <= visualChunkRange; y++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x + x, oldCoord.y + y);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x + x, newCoord.y + y);
                    ShowAndHideForChunk(clientID, hideChunkCoord, showChunkCoord);
                }
            }
        }
        else // 正常一个格子的移动距离
        {
            // 上，旧的最下面一行隐藏，新的最上一行显示
            if (newCoord.y > oldCoord.y)
            {
                for (int i = -visualChunkRange; i <= visualChunkRange; i++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x + i, oldCoord.y - visualChunkRange);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x + i, newCoord.y + visualChunkRange);
                    ShowAndHideForChunk(clientID, hideChunkCoord, showChunkCoord);
                }
            }
            // 下，旧的最下面一行显示，新的最上一行隐藏
            else if (newCoord.y < oldCoord.y)
            {
                for (int i = -visualChunkRange; i <= visualChunkRange; i++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x + i, oldCoord.y + visualChunkRange);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x + i, newCoord.y - visualChunkRange);
                    ShowAndHideForChunk(clientID, hideChunkCoord, showChunkCoord);
                }
            }

            // 左，旧的最右边面一列隐藏，新的最左边一列显示
            if (newCoord.x < oldCoord.x)
            {
                for (int i = -visualChunkRange; i <= visualChunkRange; i++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x + visualChunkRange, oldCoord.y + i);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x - visualChunkRange, newCoord.y + i);
                    ShowAndHideForChunk(clientID, hideChunkCoord, showChunkCoord);
                }
            }
            // 右，旧的最右边面一列显示，新的最左边一列隐藏
            else if (newCoord.x > oldCoord.x)
            {
                for (int i = -visualChunkRange; i <= visualChunkRange; i++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x - visualChunkRange, oldCoord.y + i);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x + visualChunkRange, newCoord.y + i);
                    ShowAndHideForChunk(clientID, hideChunkCoord, showChunkCoord);
                }
            }
        }

        // 把客户端加入到当前新块
        if (!chunkClientDic.TryGetValue(newCoord, out HashSet<ulong> newCoordClientIDs))
        {
            newCoordClientIDs = ResSystem.GetOrNew<HashSet<ulong>>();
            chunkClientDic.Add(newCoord, newCoordClientIDs);
        }
        newCoordClientIDs.Add(clientID);
    }

    // 为某个地图块下的全部客户端，显示与隐藏某个客户端
    private void ShowAndHideForChunk(ulong clientID, Vector2Int hideChunkCoord, Vector2Int showChunkCoord)
    {
        ShowClientForChunkClients(clientID, showChunkCoord);
        HideClientForChunkClients(clientID, hideChunkCoord);
        ShowChunkServerObjectForClient(clientID, showChunkCoord);
        HideChunkServerObjectForClient(clientID, hideChunkCoord);
    }

    // 某个客户端和某个地图块的客户端们全部互相可见
    private void ShowClientForChunkClients(ulong clientID, Vector2Int chunkCoord)
    {
        if (chunkClientDic.TryGetValue(chunkCoord, out HashSet<ulong> clientIDs))
        {
            foreach (ulong newClientID in clientIDs)
            {
                ClientMutualShow(clientID, newClientID);
            }
        }
    }

    // 某个客户端和某个地图块的客户端们全部互相不可见
    private void HideClientForChunkClients(ulong clientID, Vector2Int chunkCoord)
    {
        if (chunkClientDic.TryGetValue(chunkCoord, out HashSet<ulong> clientIDs))
        {
            foreach (ulong newClientID in clientIDs)
            {
                ClientMutualHide(clientID, newClientID);
            }
        }
    }

    public void RemoveClient(ulong clientID, Vector2Int coord)
    {
        if (chunkClientDic.TryGetValue(coord, out HashSet<ulong> clientIDs))
        {
            // 如果当前坐标下没有玩家，则回收容器
            if (clientIDs.Remove(clientID) && clientIDs.Count == 0)
            {
                clientIDs.ObjectPushPool();
                chunkClientDic.Remove(coord);
            }
        }
    }

    // 客户端互相可见
    private void ClientMutualShow(ulong clientA, ulong clientB)
    {
        if (clientA == clientB) return;
        if (NetManager.Instance.SpawnManager.OwnershipToObjectsTable.TryGetValue(clientA, out Dictionary<ulong, NetworkObject> aNetWorObjectDic)
            && NetManager.Instance.SpawnManager.OwnershipToObjectsTable.TryGetValue(clientB, out Dictionary<ulong, NetworkObject> bNetWorObjectDic))
        {
            // A可见B
            foreach (NetworkObject aItem in aNetWorObjectDic.Values)
            {
                if (!aItem.IsNetworkVisibleTo(clientB)) aItem.NetworkShow(clientB);
            }
            // B可见A
            foreach (NetworkObject bItem in bNetWorObjectDic.Values)
            {
                if (!bItem.IsNetworkVisibleTo(clientA)) bItem.NetworkShow(clientA);
            }
        }
    }
    // 客户端互相不可见
    private void ClientMutualHide(ulong clientA, ulong clientB)
    {
        if (clientA == clientB) return;
        if (NetManager.Instance.SpawnManager.OwnershipToObjectsTable.TryGetValue(clientA, out Dictionary<ulong, NetworkObject> aNetWorObjectDic)
            && NetManager.Instance.SpawnManager.OwnershipToObjectsTable.TryGetValue(clientB, out Dictionary<ulong, NetworkObject> bNetWorObjectDic))
        {
            // A可见B
            foreach (NetworkObject aItem in aNetWorObjectDic.Values)
            {
                if (aItem.IsNetworkVisibleTo(clientB)) aItem.NetworkHide(clientB);
            }
            // B可见A
            foreach (NetworkObject bItem in bNetWorObjectDic.Values)
            {
                if (bItem.IsNetworkVisibleTo(clientA)) bItem.NetworkHide(clientA);
            }
        }
    }

    // 某个地图块的全部服务端对象对某个客户端可见
    private void ShowChunkServerObjectForClient(ulong clientID, Vector2Int chunkCoord)
    {
        if (chunkServerObjectDic.TryGetValue(chunkCoord, out HashSet<NetworkObject> serverObjects))
        {
            foreach (NetworkObject serverObject in serverObjects)
            {
                if (!serverObject.IsNetworkVisibleTo(clientID))
                {
                    serverObject.NetworkShow(clientID);
                }
            }
        }

    }
    // 某个地图块的全部服务端对象对某个客户端不可见
    private void HideChunkServerObjectForClient(ulong clientID, Vector2Int chunkCoord)
    {
        if (chunkServerObjectDic.TryGetValue(chunkCoord, out HashSet<NetworkObject> serverObjects))
        {
            foreach (NetworkObject serverObject in serverObjects)
            {
                if (serverObject.IsNetworkVisibleTo(clientID))
                {
                    serverObject.NetworkHide(clientID);
                }
            }
        }
    }
    #endregion

    #region Server

    public void InitServerObject(NetworkObject serverObject, Vector2Int chunkCoord)
    {
        UpdateServerObjectChunkCoord(serverObject, defaultCoord, chunkCoord);
    }
    public void UpdateServerObjectChunkCoord(NetworkObject serverObject, Vector2Int oldCoord, Vector2Int newCoord)
    {
        if (oldCoord == newCoord) return;
        // 从旧的地图块中移除
        RemoveServerObject(serverObject, oldCoord);

        // 是否跨地图块移动
        if (Vector2Int.Distance(oldCoord, newCoord) > 1.5f) // 超过单个格子移动的极限距离 所以是传送性质的位移
        {
            for (int x = -visualChunkRange; x <= visualChunkRange; x++)
            {
                for (int y = -visualChunkRange; y <= visualChunkRange; y++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x + x, oldCoord.y + y);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x + x, newCoord.y + y);
                    ShowAndHideChunkClientsForServerObject(serverObject, hideChunkCoord, showChunkCoord);
                }
            }
        }
        else // 正常一个格子的移动距离
        {
            // 上，旧的最下面一行隐藏，新的最上一行显示
            if (newCoord.y > oldCoord.y)
            {
                for (int i = -visualChunkRange; i <= visualChunkRange; i++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x + i, oldCoord.y - visualChunkRange);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x + i, newCoord.y + visualChunkRange);
                    ShowAndHideChunkClientsForServerObject(serverObject, hideChunkCoord, showChunkCoord);
                }
            }
            // 下，旧的最下面一行显示，新的最上一行隐藏
            else if (newCoord.y < oldCoord.y)
            {
                for (int i = -visualChunkRange; i <= visualChunkRange; i++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x + i, oldCoord.y + visualChunkRange);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x + i, newCoord.y - visualChunkRange);
                    ShowAndHideChunkClientsForServerObject(serverObject, hideChunkCoord, showChunkCoord);
                }
            }

            // 左，旧的最右边面一列隐藏，新的最左边一列显示
            if (newCoord.x < oldCoord.x)
            {
                for (int i = -visualChunkRange; i <= visualChunkRange; i++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x + visualChunkRange, oldCoord.y + i);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x - visualChunkRange, newCoord.y + i);
                    ShowAndHideChunkClientsForServerObject(serverObject, hideChunkCoord, showChunkCoord);
                }
            }
            // 右，旧的最右边面一列显示，新的最左边一列隐藏
            else if (newCoord.x > oldCoord.x)
            {
                for (int i = -visualChunkRange; i <= visualChunkRange; i++)
                {
                    Vector2Int hideChunkCoord = new Vector2Int(oldCoord.x - visualChunkRange, oldCoord.y + i);
                    Vector2Int showChunkCoord = new Vector2Int(newCoord.x + visualChunkRange, newCoord.y + i);
                    ShowAndHideChunkClientsForServerObject(serverObject, hideChunkCoord, showChunkCoord);
                }
            }
        }

        // 把服务端对象加入到当前新块
        if (!chunkServerObjectDic.TryGetValue(newCoord, out HashSet<NetworkObject> serverObjects))
        {
            serverObjects = ResSystem.GetOrNew<HashSet<NetworkObject>>();
            chunkServerObjectDic.Add(newCoord, serverObjects);
        }
        serverObjects.Add(serverObject);
    }

    public void RemoveServerObject(NetworkObject serverObject, Vector2Int chunkCoord)
    {
        if (chunkServerObjectDic.TryGetValue(chunkCoord, out HashSet<NetworkObject> serverObjects))
        {
            serverObjects.Remove(serverObject);
        }
    }

    private void ShowAndHideChunkClientsForServerObject(NetworkObject serverObject, Vector2Int hideChunkCoord, Vector2Int showChunkCoord)
    {
        ShowChunkClientsForServerObject(serverObject, showChunkCoord);
        HideChunkClientsForServerObject(serverObject, hideChunkCoord);
    }

    // 为一个服务端对象显示某个地图块下的全部客户端
    private void ShowChunkClientsForServerObject(NetworkObject serverObject, Vector2Int chunkCoord)
    {
        if (chunkClientDic.TryGetValue(chunkCoord, out HashSet<ulong> clientIDs))
        {
            foreach (ulong clientID in clientIDs)
            {
                if (!serverObject.IsNetworkVisibleTo(clientID))
                {
                    serverObject.NetworkShow(clientID);
                }
            }
        }
    }

    // 为一个服务端对象隐藏某个地图块下的全部客户端
    private void HideChunkClientsForServerObject(NetworkObject serverObject, Vector2Int chunkCoord)
    {
        if (chunkClientDic.TryGetValue(chunkCoord, out HashSet<ulong> clientIDs))
        {
            foreach (ulong clientID in clientIDs)
            {
                if (serverObject.IsNetworkVisibleTo(clientID))
                {
                    serverObject.NetworkHide(clientID);
                }
            }
        }
    }

    #endregion

}
