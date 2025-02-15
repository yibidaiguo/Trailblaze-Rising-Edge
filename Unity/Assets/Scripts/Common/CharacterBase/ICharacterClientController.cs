using UnityEngine;

public interface ICharacterClientController : INetworkController
{
    public void StartSkill(int skillIndex);
    public void StartSkillHit();
    public void PlaySkillHitEffect(Vector3 point);
}
