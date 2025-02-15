using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerLanuch : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 30;
        InitServers();
        SceneManager.LoadScene("GameScene");
    }

    private void InitServers()
    {
        ServerResSystem.InstantiateNetworkManager().FirstInit();
        NetManager.Instance.InitServer();
        Debug.Log("InitServers Succeed");
    }
}
