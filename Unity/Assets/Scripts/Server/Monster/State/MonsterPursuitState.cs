using UnityEngine;

public class MonsterPursuitState : MonsterStateBase
{
    private float timer;
    public override void Enter()
    {
        timer = config.pursuitTime;
        serverController.StartMove();
    }
    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            serverController.ChangeState(MonsterState.Patrol);
            return;
        }
        if (serverController.CheckTargetPlayer())
        {
            // 距离判断
            if (Vector3.Distance(serverController.transform.position, serverController.targetPlayer.transform.position) <= serverController.monsterConfig.attackRange)
            {
                // CD检测
                if (serverController.CheckAttack())
                {
                    serverController.ChangeState(MonsterState.Attack);
                    return;
                }
            }
            serverController.navMeshAgent.SetDestination(serverController.targetPlayer.transform.position);
            serverController.PlayAnimation(serverController.navMeshAgent.velocity == Vector3.zero ? "Idle" : "Move");
        }
        else
        {
            serverController.ChangeState(MonsterState.Patrol);
            return;
        }

    }
    public override void Exit()
    {
        serverController.StopMove();
    }
}
