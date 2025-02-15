using JKFrame;
using System.Collections;
using UnityEngine;

public class ClientGameSceneManager : MonoBehaviour
{
    void Start()
    {
        ClientMapManager.Instance.Init();
        MonsterClientManager.Instance.Init();
        PlayerManager.Instance.Init();
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        UI_LoadingWindow loadingWindow = UISystem.Show<UI_LoadingWindow>();
        loadingWindow.Set("Loading...");
        // 申请进入游戏
        NetMessageManager.Instance.SendMessageToServer(MessageType.C_S_EnterGame, default(C_S_EnterGame));
        float progress = 0;
        loadingWindow.UpdateProgress(progress, 100);
        yield return CoroutineTool.WaitForFrame();
        while (!ClientMapManager.Instance.IsLoadingCompleted())
        {
            yield return CoroutineTool.WaitForFrame();
            if (progress < 99)
            {
                progress += 0.1f;
                loadingWindow.UpdateProgress(progress, 100);
            }
        }
        progress = 99;
        loadingWindow.UpdateProgress(progress, 100);
        while (!PlayerManager.Instance.IsLoadingCompleted())
        {
            yield return CoroutineTool.WaitForFrame();
        }
        progress = 100;
        loadingWindow.UpdateProgress(progress, 100);
        UISystem.Close<UI_LoadingWindow>();
        UISystem.Show<UI_ChatWindow>();
    }

}
