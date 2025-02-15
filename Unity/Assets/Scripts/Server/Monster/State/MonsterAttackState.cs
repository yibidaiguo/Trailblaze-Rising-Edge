using UnityEngine;

public class MonsterAttackState : MonsterStateBase
{
    private SkillConfig skillConfig => serverController.skillConfig;
    public override void Enter()
    {
        MonsterView view = mainController.View;
        view.startSkillHitAction += View_startSkillHitAction;
        view.stopSkilllHitAction += View_stopSkilllHitAction;
        view.shootAction += View_shootAction;
        StartAttack();
    }

    public override void Exit()
    {
        MonsterView view = mainController.View;
        view.startSkillHitAction -= View_startSkillHitAction;
        view.stopSkilllHitAction -= View_stopSkilllHitAction;
        view.shootAction -= View_shootAction;
        View_stopSkilllHitAction();
    }
    public override void Update()
    {
        if (serverController.CheckAnimationState(skillConfig.animationName, out float normalizedTime))
        {
            if (normalizedTime < skillConfig.rotateNormalizedTime)
            {
                if (serverController.CheckTargetPlayer())
                {
                    Vector3 dir = serverController.targetPlayer.transform.position - serverController.transform.position;
                    serverController.transform.rotation = Quaternion.RotateTowards(mainController.View.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * MonsterServerController.attackRotateSpeed);
                }
            }
            else if (normalizedTime >= skillConfig.endNormalizedTime)
            {
                serverController.ChangeState(MonsterState.Pursuit);
            }
        }
    }
    private void StartAttack()
    {
        serverController.OnAttack();
        serverController.skillConfigIndex = Random.Range(0, mainController.skillConfigList.Count);
        serverController.PlayAnimation(skillConfig.animationName);
        mainController.StartSkillClientRpc(serverController.skillConfigIndex);
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
    private void View_shootAction()
    {
        serverController.Shoot();
    }
}
