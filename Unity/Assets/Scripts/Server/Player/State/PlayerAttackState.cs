using System;
using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private SkillConfig skillConfig => serverController.skillConfig;
    public override void Enter()
    {
        PlayerView view = mainController.View;
        view.startSkillHitAction += View_startSkillHitAction;
        view.stopSkilllHitAction += View_stopSkilllHitAction;
        view.rootMotionAction += OnRootMotion;
        StartAttack();
    }
    public override void Exit()
    {
        PlayerView view = mainController.View;
        view.startSkillHitAction -= View_startSkillHitAction;
        view.stopSkilllHitAction -= View_stopSkilllHitAction;
        view.rootMotionAction -= OnRootMotion;
        View_stopSkilllHitAction();
    }

    public override void Update()
    {
        if (serverController.CheckAnimationState(skillConfig.animationName, out float normalizedTime))
        {
            if (serverController.inputData.moveDir != Vector3.zero && normalizedTime < skillConfig.rotateNormalizedTime)
            {
                mainController.View.transform.rotation = Quaternion.RotateTowards(mainController.View.transform.rotation, Quaternion.LookRotation(serverController.inputData.moveDir), Time.deltaTime * serverController.rotateSpeed);
            }
            if (normalizedTime >= skillConfig.switchNormalizedTime && serverController.inputData.attack)
            {
                StartAttack();
            }
            else if (normalizedTime >= skillConfig.endNormalizedTime)
            {
                serverController.ChangeState(serverController.inputData.moveDir == Vector3.zero ? PlayerState.Idle : PlayerState.Move);
            }
        }
    }

    private void StartAttack()
    {
        int attackIndex = serverController.skillConfigIndex + 1;
        if (attackIndex >= mainController.skillConfigList.Count) attackIndex = 0;
        serverController.skillConfigIndex = attackIndex;
        serverController.PlayAnimation(skillConfig.animationName);
        mainController.StartSkillClientRpc(attackIndex);
    }

    private void View_startSkillHitAction()
    {
        serverController.weapon?.StartHit();
        mainController.StartSkillHitClientRpc();
    }

    private void View_stopSkilllHitAction()
    {
        serverController.weapon?.StopHit();
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
