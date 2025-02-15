using UnityEngine;
public class PlayerMoveState : PlayerStateBase
{
    public override void Enter()
    {
        serverController.PlayAnimation("Move");
        serverController.mainController.View.rootMotionAction += OnRootMotion;
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
        if (serverController.inputData.moveDir == Vector3.zero)
        {
            serverController.ChangeState(PlayerState.Idle);
            return;
        }
        // 旋转
        mainController.View.transform.rotation = Quaternion.RotateTowards(mainController.View.transform.rotation, Quaternion.LookRotation(serverController.inputData.moveDir), Time.deltaTime * serverController.rotateSpeed);
    }
    public override void Exit()
    {
        serverController.mainController.View.rootMotionAction -= OnRootMotion;
    }

    private void OnRootMotion(Vector3 deltaPostion, Quaternion deltaRotation)
    {
        serverController.animator.speed = serverController.rootMotionMoveSpeedMultiply;
        deltaPostion.y -= 9.8f * Time.deltaTime; // 模拟重力
        serverController.characterController.Move(deltaPostion);
        // 更新AOI
        if (deltaPostion != Vector3.zero)
        {
            serverController.UpdateAOICoord();
        }
    }
}
