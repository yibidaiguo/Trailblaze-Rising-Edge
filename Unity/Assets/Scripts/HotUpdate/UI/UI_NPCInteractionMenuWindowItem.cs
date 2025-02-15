using JKFrame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_NPCInteractionMenuWindowItem : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Button button;

    private string optionKey;
    public void Init(string optionKey, UnityAction onSelected)
    {
        this.optionKey = optionKey;
        OnLanguageChanged(LocalizationSystem.LanguageType);
        LocalizationSystem.RegisterLanguageEvent(OnLanguageChanged);
        button.onClick.AddListener(OnClick);
        button.onClick.AddListener(onSelected);
    }

    private void OnClick()
    {
        UISystem.Close<UI_NPCInteractionMenuWindow>();
    }

    private void OnLanguageChanged(LanguageType language)
    {
        text.text = LocalizationSystem.GetContent<LocalizationStringData>(optionKey, language).content;
    }

    public void Destroy()
    {
        LocalizationSystem.UnregisterLanguageEvent(OnLanguageChanged);
        button.onClick.RemoveAllListeners();
        this.GameObjectPushPool();
    }
}
