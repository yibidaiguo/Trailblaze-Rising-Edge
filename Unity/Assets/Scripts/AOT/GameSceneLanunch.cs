using JKFrame;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameSceneLanunch : MonoBehaviour
{
    IEnumerator Start()
    {
        while (NetworkManager.Singleton == null)
        {
            yield return CoroutineTool.WaitForFrame();
        }

        EventSystem.TypeEventTrigger<GameSceneLanunchEvent>(default);
        Destroy(gameObject);
    }
}
