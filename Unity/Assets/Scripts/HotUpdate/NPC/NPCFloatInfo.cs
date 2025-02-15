using UnityEngine;

public class NPCFloatInfo : MonoBehaviour
{
    [SerializeField] private TextMesh nameText;
    private string nameKey;
    public void Init(string nameKey)
    {
#if UNITY_EDITOR
        if (NetManager.Instance.IsServer) return;
#endif
        this.nameKey = nameKey;
        UpdateName(LocalizationSystem.LanguageType);
        LocalizationSystem.RegisterLanguageEvent(UpdateName);
    }
    private void OnDisable()
    {
#if UNITY_EDITOR
        if (NetManager.Instance.IsServer) return;
#endif
        LocalizationSystem.UnregisterLanguageEvent(UpdateName);
    }

    private void UpdateName(LanguageType language)
    {
#if UNITY_EDITOR
        if (NetManager.Instance.IsServer) return;
#endif
        nameText.text = LocalizationSystem.GetContent<LocalizationStringData>(nameKey, language).content;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (NetManager.Instance.IsServer) return;
#endif
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}
