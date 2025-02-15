using JKFrame;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DialogWindow : UI_CustomWindowBase, IPointerClickHandler
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text contentText;

    private DialogConfig dialogConfig;
    private int stepIndex;
    private Action onEnd;
    private string playerName;
    private string npcNameKey;
    private bool haveNextStep => stepIndex < dialogConfig.stepList.Count - 1;
    private DialogStepConfig stepConfig => dialogConfig.stepList[stepIndex];

    public void Show(DialogConfig dialogConfig, string playerName, string npcNameKey, Action onEnd)
    {
        this.dialogConfig = dialogConfig;
        this.playerName = playerName;
        this.npcNameKey = npcNameKey;
        this.stepIndex = 0;
        this.onEnd = onEnd;
        StartDialogStep();
        LocalizationSystem.RegisterLanguageEvent(OnLanguageChanged);
    }

    public override void OnClose()
    {
        base.OnClose();
        LocalizationSystem.UnregisterLanguageEvent(OnLanguageChanged);
    }

    private void StartDialogStep()
    {
        StartCoroutine(DoDialog());
    }

    private IEnumerator DoDialog()
    {
        SetName(LocalizationSystem.LanguageType);
        // 逐字效果
        string target = stepConfig.contentDic[LocalizationSystem.LanguageType];
        string current = "";
        foreach (char item in target)
        {
            current += item;
            yield return CoroutineTool.WaitForSeconds(0.1f);
            contentText.text = current;
        }
    }


    private void OnLanguageChanged(LanguageType languageType)
    {
        StopAllCoroutines();
        SetName(languageType);
        SetContent(languageType);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        if (haveNextStep)
        {
            stepIndex += 1;
            StartDialogStep();
        }
        else
        {
            onEnd?.Invoke();
            UISystem.Close<UI_DialogWindow>();
        }
    }

    private void SetName(LanguageType languageType)
    {
        nameText.text = stepConfig.player ? playerName : LocalizationSystem.GetContent<LocalizationStringData>(npcNameKey, languageType).content;
    }
    private void SetContent(LanguageType languageType)
    {
        contentText.text = stepConfig.contentDic[languageType];
    }
}
