using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientTestManager : MonoBehaviour
{
    private void OnGUI()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.localPlayer != null && NetManager.Instance.IsConnectedClient)
        {
            // 延迟
            GUILayout.Label("Delay:" + ClientRTTInfo.Instance.rttMs);
            // 当前坐标
            GUILayout.Label("Position:" + PlayerManager.Instance.localPlayer.transform.position);
            // 服务端对象数量
            if (NetManager.Instance.SpawnManager.OwnershipToObjectsTable.TryGetValue(NetManager.ServerClientId, out Dictionary<ulong, NetworkObject> temp))
            {
                GUILayout.Label("ServerObjects:" + temp.Count);
            }
            // 其他客户端对象数量
            int clientObjects = 0;

            foreach (KeyValuePair<ulong, Dictionary<ulong, NetworkObject>> item in NetManager.Instance.SpawnManager.OwnershipToObjectsTable)
            {
                if (item.Key != NetManager.ServerClientId && item.Key != NetManager.Instance.LocalClientId)
                {
                    clientObjects += item.Value.Count;
                }
            }
            GUILayout.Label("OtherClintObjects:" + clientObjects);
        }
    }
}
