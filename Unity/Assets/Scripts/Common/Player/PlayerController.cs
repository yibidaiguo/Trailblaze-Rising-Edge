using JKFrame;
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
// 公共
public partial class PlayerController : CharacterControllerBase<PlayerView, IPlayerClientController, IPlayerServerController>
{
    #region 静态成员
    private static Func<string, GameObject> getWeaponFunc;
    public static void SetGetWeaponFunc(Func<string, GameObject> func)
    {
        getWeaponFunc = func;
    }
    #endregion

    public NetVaribale<PlayerState> currentState = new NetVaribale<PlayerState>(PlayerState.None);
    public NetVaribale<FixedString32Bytes> usedWeaponName = new NetVaribale<FixedString32Bytes>();
    public NetVaribale<FixedString32Bytes> playerName = new NetVaribale<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
#if !UNITY_SERVER || UNITY_EDITOR
        if (IsClient) EventSystem.TypeEventTrigger(new SpawnPlayerEvent { newPlayer = this });
#endif
        base.OnNetworkSpawn();
        usedWeaponName.OnValueChanged = OnUsedWeaponNameChanged;
        UpdateWeaponObject(usedWeaponName.Value.ToString());
    }
    private void OnUsedWeaponNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        UpdateWeaponObject(newValue.ToString());
    }

    public event Action<GameObject> onUpdateWeaponObjectAction;
    private void UpdateWeaponObject(string weaponID)
    {
        if (string.IsNullOrWhiteSpace(weaponID)) return;
        GameObject weaponGameObject = getWeaponFunc.Invoke(weaponID);
        View.SetWeapon(weaponGameObject);
        onUpdateWeaponObjectAction?.Invoke(weaponGameObject);
    }

    #region ServerRpc
    // 相当于调用服务端上自身的本体
    [ServerRpc(RequireOwnership = true)]
    public void SendInputMoveDirServerRpc(Vector3 moveDir)
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverController.ReceiveMoveInput(moveDir);
#endif
    }
    [ServerRpc(RequireOwnership = true)]
    public void SendJumpInputServerRpc()
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverController.ReceiveJumpInput();
#endif
    }
    [ServerRpc(RequireOwnership = true)]
    public void SendAttackInputServerRpc(bool value)
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverController.ReceiveAttackInput(value);
#endif
    }
    #endregion

}

// 客户端
#if !UNITY_SERVER || UNITY_EDITOR
public partial class PlayerController
{

}
#endif

#if UNITY_SERVER || UNITY_EDITOR
// 服务端
public partial class PlayerController : IStateMachineOwner
{
    public void UpdateWeaponNetVar(string weaponID)
    {
        usedWeaponName.Value = weaponID;
    }
}
#endif

