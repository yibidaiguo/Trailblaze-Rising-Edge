using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class CharacterControllerBase : NetworkEntityBase
{
    public List<SkillConfig> skillConfigList = new List<SkillConfig>();
    public NetVaribale<float> currentHp = new NetVaribale<float>();
    public abstract CharacterViewBase CharacterView { get; }

}
// 公共
public abstract partial class CharacterControllerBase<V, C, S> : CharacterControllerBase where V : CharacterViewBase where C : ICharacterClientController where S : ICharacterServerController
{
    [SerializeField] protected V view;
    public V View { get => view; }
    public override CharacterViewBase CharacterView => view;

    #region ClientRPC
    [ClientRpc]
    public void StartSkillClientRpc(int skillIndex)
    {
#if !UNITY_SERVER || UNITY_EDITOR
        clientController.StartSkill(skillIndex);
#endif
    }
    [ClientRpc]
    public void StartSkillHitClientRpc()
    {
#if !UNITY_SERVER || UNITY_EDITOR
        clientController.StartSkillHit();
#endif
    }
    [ClientRpc]
    public void PlaySkillHitEffectClientRpc(Vector3 point)
    {
#if !UNITY_SERVER || UNITY_EDITOR
        clientController.PlaySkillHitEffect(point);
#endif
    }
    #endregion
}

// 客户端
#if !UNITY_SERVER || UNITY_EDITOR
public abstract partial class CharacterControllerBase<V, C, S>
{
    protected C clientController => (C)sideController;
}
#endif


// 服务端
#if UNITY_SERVER || UNITY_EDITOR
public abstract partial class CharacterControllerBase<V, C, S>
{
    protected S serverController => (S)sideController;
}
#endif