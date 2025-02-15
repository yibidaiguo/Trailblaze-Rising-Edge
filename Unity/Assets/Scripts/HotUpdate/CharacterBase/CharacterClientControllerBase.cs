using JKFrame;
using UnityEngine;


public abstract class CharacterClientControllerBase<M> : MonoBehaviour, ICharacterClientController where M : CharacterControllerBase
{
    public M mainController { get; private set; }
    protected SkillConfig currentSkillConfig;

    public virtual void FirstInit() // 第一次被添加组件时调用
    {
        this.mainController = GetComponent<M>();
        mainController.SetSideController(this);
    }
    public abstract void Init();
    public virtual void OnNetworkSpawn()
    {
        mainController.currentHp.OnValueChanged = OnHpChanged;
    }
    protected abstract void OnHpChanged(float previousValue, float newValue);
    public virtual void OnNetworkDespawn() { }
    #region 战斗
    public void StartSkill(int skillIndex)
    {
        currentSkillConfig = mainController.skillConfigList[skillIndex];
        PlaySkillEffect(currentSkillConfig.releaseEffect);
    }

    public void StartSkillHit()
    {
        PlaySkillEffect(currentSkillConfig.startHitEffect);
    }

    private void PlaySkillEffect(SkillEffect skillEffect)
    {
        if (skillEffect == null) return;
        if (skillEffect.audio != null)
        {
            AudioSystem.PlayOneShot(skillEffect.audio, transform.position);
        }
        if (skillEffect.prefab != null)
        {
            GameObject effectObj = GlobalUtility.GetOrInstantiate(skillEffect.prefab, null);
            effectObj.transform.position = mainController.CharacterView.transform.TransformPoint(skillEffect.offset);
            effectObj.transform.rotation = mainController.CharacterView.transform.rotation * Quaternion.Euler(skillEffect.rotation);
            effectObj.transform.localScale = skillEffect.scale;
        }
    }

    public void PlaySkillHitEffect(Vector3 point)
    {
        SkillEffect skillEffect = currentSkillConfig.hitEffect;
        if (skillEffect == null) return;
        if (skillEffect.audio != null)
        {
            AudioSystem.PlayOneShot(skillEffect.audio, point);
        }
        if (skillEffect.prefab != null)
        {
            GameObject effectObj = GlobalUtility.GetOrInstantiate(skillEffect.prefab, null);
            effectObj.transform.position = point;
            effectObj.transform.localScale = skillEffect.scale;
        }
    }
    #endregion
}
