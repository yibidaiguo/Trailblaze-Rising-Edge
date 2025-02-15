using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_TaskWindowItem : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text progressText;

    private TaskConfig taskConfig;
    public void Init(TaskConfig taskConfig, TaskData taskData)
    {
        LocalizationSystem.RegisterLanguageEvent(OnLanguageChanged);
        Set(taskConfig, taskData);
    }

    public void Set(TaskConfig taskConfig, TaskData taskData)
    {
        this.taskConfig = taskConfig;
        OnLanguageChanged(LocalizationSystem.LanguageType);
        UpdateProgreessText(taskData.taskProgress);
    }
    private void OnDestroy()
    {
        LocalizationSystem.UnregisterLanguageEvent(OnLanguageChanged);
    }

    private void OnLanguageChanged(LanguageType type)
    {
        titleText.text = taskConfig.nameDic[type];
        descriptionText.text = taskConfig.descriptionDic[type];
    }

    public void UpdateProgreessText(int curr)
    {
        int count = taskConfig.taskInfo.GetCount();
        if (count <= 0)
        {
            progressText.text = "";
        }
        else progressText.text = $"{curr}/{count}";
    }


}
