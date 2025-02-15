using UnityEngine;

public class PlayerJumpState : PlayerStateBase
{
    public override void Enter()
    {
        serverController.inputData.jump = false;
        serverController.PlayAnimation("JumpStart");
        serverController.mainController.View.rootMotionAction += OnRootMotion;
    }
    public override void Update()
    {
        if (serverController.CheckAnimationState("JumpStart", out float time) && time >= 0.95f)
        {
            serverController.ChangeState(PlayerState.AirDown);
        }
    }

    public override void Exit()
    {
        serverController.mainController.View.rootMotionAction -= OnRootMotion;
    }
    private void OnRootMotion(Vector3 deltaPostion, Quaternion deltaRotation)
    {
        // 应用一个Y轴上升的系数
        deltaPostion.y *= serverController.jumpHeightMultiply;

        Vector3 moveDir = serverController.inputData.moveDir;
        // 应用玩家的输入
        if (moveDir != Vector3.zero)
        {
            mainController.View.transform.rotation = Quaternion.RotateTowards(mainController.View.transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * serverController.rotateSpeed);
            Vector3 forward = Time.deltaTime * serverController.rootMotionMoveSpeedMultiply * mainController.View.transform.forward;
            deltaPostion.x = forward.x;
            deltaPostion.z = forward.z;
        }
        serverController.characterController.Move(deltaPostion);
    }
}
