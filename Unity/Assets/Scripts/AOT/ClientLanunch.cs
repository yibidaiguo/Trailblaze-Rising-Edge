using JKFrame;
using UnityEngine;

public class ClientLanunch : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
        GetComponent<HotUpdateSystem>().StartHotUpdate(null, (bool succeed) =>
        {
            if (succeed)
            {
                OnHotUpdateSucceed();
            }
        });
    }

    private void OnHotUpdateSucceed()
    {
        ResSystem.InstantiateGameObject("ClientGlobal");
    }

}
