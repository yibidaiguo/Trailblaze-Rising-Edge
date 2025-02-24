using JKFrame;
using System;
using UnityEngine;

public abstract class NPCControllerBase : MonoBehaviour
{
    [SerializeField] protected string configName;
    [SerializeField] protected DialogConfig defaultDialogConfig;
    [SerializeField] protected string taskID;
    [SerializeField] protected GameObject prompt;
    [SerializeField] protected Transform floatInfoPoint;
    public abstract string nameKey { get; }
    private void Start()
    {
#if UNITY_EDITOR
        if (NetManager.Instance.IsServer)
        {
            Destroy(this);
            return;
        }
#endif
        prompt.SetActive(false);
        InitFloatInfo();
    }

    public void InitFloatInfo()
    {
        NPCFloatInfo floatInfo = ResSystem.InstantiateGameObject<NPCFloatInfo>(floatInfoPoint, "NPCFloatInfo");
        floatInfo.transform.localPosition = Vector3.zero;
        floatInfo.Init(nameKey);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerManager.Instance != null && other.gameObject == PlayerManager.Instance.localPlayer.gameObject)
        {
            prompt.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (PlayerManager.Instance != null && other.gameObject == PlayerManager.Instance.localPlayer.gameObject)
        {
            if (prompt != null && Camera.main != null) prompt.transform.LookAt(Camera.main.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerManager.Instance.localPlayer.gameObject)
        {
            prompt.SetActive(false);
            if (ClientUtility.GetWindowActiveState(out UI_NPCInteractionMenuWindow _))
            {
                UISystem.Close<UI_NPCInteractionMenuWindow>();
            }
        }
    }

    private void Update()
    {
        if (prompt.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleInteraction();
            }
        }
    }

    protected void ToggleInteraction()
    {
        if (!ClientUtility.GetWindowActiveState(out UI_NPCInteractionMenuWindow _))
        {
            OnInteraction(UISystem.Show<UI_NPCInteractionMenuWindow>());
        }
        else UISystem.Close<UI_NPCInteractionMenuWindow>();
    }

    protected virtual void OnInteraction(UI_NPCInteractionMenuWindow window)
    {
        window.AddOption(nameKey, MainInteraction);
        window.AddOption("对话", StartDialog);
        window.AddOption("AI对话", OpenAIWindow);
        if (!string.IsNullOrEmpty(taskID) && !PlayerManager.Instance.taskDatas.Contain(taskID))
        {
            window.AddOption("任务", TakeTask);
        }
    }

    private void OpenAIWindow()
    {
        UISystem.Show<UI_DeepSeekDialog>().Show(PlayerManager.Instance.PlayerName, nameKey,defaultDialogConfig);
    }

    protected void StartDialog()
    {
        // 可能存在基于任务的对话
        for (int i = 0; i < PlayerManager.Instance.taskDatas.tasks.Count; i++)
        {
            TaskData taskData = PlayerManager.Instance.taskDatas.tasks[i];
            TaskConfig taskConfig = ResSystem.LoadAsset<TaskConfig>(taskData.taskConfigId);
            if (taskConfig.taskInfo is DialogTaskInfo dialogTaskInfo && dialogTaskInfo.npcID == configName)
            {
                DialogConfig dialogConfig = ResSystem.LoadAsset<DialogConfig>(dialogTaskInfo.dialogConfigId);
                int index = i;
                UISystem.Show<UI_DialogWindow>().Show(dialogConfig, PlayerManager.Instance.PlayerName, nameKey, () => OnTaskDialogEnd(index));
                return;
            }
        }

        UISystem.Show<UI_DialogWindow>().Show(defaultDialogConfig, PlayerManager.Instance.PlayerName, nameKey, null);
    }

    private void OnTaskDialogEnd(int taskIndex)
    {
        // 任务完成
        PlayerManager.Instance.CompleteDialogTask(taskIndex);
    }

    protected abstract void MainInteraction();
    private void TakeTask()
    {
        NetMessageManager.Instance.SendMessageToServer(NetMessageType.C_S_AddTask, new C_S_AddTask
        {
            taskID = taskID
        });
    }
}
