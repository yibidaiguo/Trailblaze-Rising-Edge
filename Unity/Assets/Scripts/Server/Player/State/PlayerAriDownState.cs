using UnityEngine;
public class PlayerAriDownState : PlayerStateBase
{
    private bool onEndAnimation;

    public override void Enter()
    {
        onEndAnimation = false;
        serverController.PlayAnimation("JumpLoop");
    }

    public override void Update()
    {
        if (onEndAnimation)
        {
            if (serverController.inputData.moveDir != Vector3.zero)
            {
                serverController.ChangeState(PlayerState.Move);
            }
            else
            {
                if (serverController.CheckAnimationState("JumpEnd", out float time) && time >= 0.95f)
                {
                    serverController.ChangeState(PlayerState.Idle);
                }
            }
        }
        else
        {
            Vector3 inputDir = serverController.inputData.moveDir.normalized * serverController.ariMoveSpeed;
            Vector3 deltaPostion = new Vector3(inputDir.x, serverController.gravity, inputDir.z);
            // 应用玩家的输入
            if (inputDir != Vector3.zero)
            {
                mainController.View.transform.rotation = Quaternion.RotateTowards(mainController.View.transform.rotation, Quaternion.LookRotation(inputDir), Time.deltaTime * serverController.rotateSpeed);
            }
            serverController.characterController.Move(deltaPostion * Time.deltaTime);

            if (serverController.characterController.isGrounded)
            {
                serverController.PlayAnimation("JumpEnd");
                onEndAnimation = true;
            }
        }
    }
}

