using UnityEngine;

[OnServerBuild(ComponentMode.Delete)]
public class MonsterFloatInfo : MonoBehaviour
{
    [SerializeField] private TextMesh nameText;
    [SerializeField] private SpriteRenderer hpBarFillSpriteRenderer;
    private MonsterConfig monsterConfig;
    public void Init(MonsterConfig monsterConfig)
    {
        this.monsterConfig = monsterConfig;
        LocalizationSystem.RegisterLanguageEvent(OnLanguageChanged);
        OnLanguageChanged(LocalizationSystem.LanguageType);
    }

    private void OnLanguageChanged(LanguageType type)
    {
        nameText.text = monsterConfig.nameDic[type];
    }
    private void OnDisable()
    {
        LocalizationSystem.UnregisterLanguageEvent(OnLanguageChanged);
    }
    public void UpdateHp(float fillAmount)
    {
        hpBarFillSpriteRenderer.transform.localScale = new Vector3(fillAmount, 1, 1);
    }
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}
