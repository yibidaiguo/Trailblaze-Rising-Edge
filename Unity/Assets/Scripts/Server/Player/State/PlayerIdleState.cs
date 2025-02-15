using UnityEngine;
public class PlayerIdleState : PlayerStateBase
{
    public override void Enter()
    {
        serverController.PlayAnimation("Idle");
    }
    public override void Update()
    {
        if (serverController.inputData.attack)
        {
            serverController.ChangeState(PlayerState.Attack);
            return;
        }
        if (serverController.inputData.jump)
        {
            serverController.ChangeState(PlayerState.Jump);
            return;
        }
        if (serverController.inputData.moveDir != Vector3.zero)
        {
            serverController.ChangeState(PlayerState.Move);
            return;
        }
        if (!serverController.characterController.isGrounded)
        {
            serverController.characterController.Move(new Vector3(0, -9.8F * Time.deltaTime, 0));
        }
    }
}
