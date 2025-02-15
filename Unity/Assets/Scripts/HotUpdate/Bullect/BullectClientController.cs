using JKFrame;
using UnityEngine;

public class BullectClientController : MonoBehaviour, IBullectClientController
{
    public BullectController mainController { get; private set; }
    private BullectConfig config => mainController.config;
    public void FirstInit()
    {
        mainController = GetComponent<BullectController>();
    }

    public void Init() { }


    public void OnNetworkDespawn()
    {
    }

    public void OnNetworkSpawn()
    {
    }

    public void PlayHitEffect(Vector3 point)
    {
        PlayEffect(point, config.hitEffect);
    }

    public void PlayReleaseEffect()
    {
        PlayEffect(transform.position, config.releaseEffect);
    }

    private void PlayEffect(Vector3 point, SkillEffect skillEffect)
    {
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
}
