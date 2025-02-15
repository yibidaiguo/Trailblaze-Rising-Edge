using JKFrame;
using UnityEngine;
public class PlayerStateBase : StateBase
{
    protected PlayerServerController serverController;
    public PlayerController mainController { get => serverController.mainController; }
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        serverController = (PlayerServerController)owner;
    }


}
