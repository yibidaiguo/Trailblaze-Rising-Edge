using UnityEngine;

public class ServerGameSceneManager : MonoBehaviour
{
    void Start()
    {
        DatabaseManager.Instance.Init();
        AOIManager.Instance.Init();
        ClientsManager.Instance.Init();
        ServerMapManager.Instance.Init();
    }
}
