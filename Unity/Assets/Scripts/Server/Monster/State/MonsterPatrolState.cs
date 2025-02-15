using UnityEngine;

public class MonsterPatrolState : MonsterStateBase
{
    private float timer;
    public override void Enter()
    {
        serverController.PlayAnimation("Move");
        timer = Random.Range(config.maxPatrolTime / 2f, config.maxPatrolTime);
        Vector3 point = serverController.GetPatrolPoint();
        serverController.StartMove();
        serverController.navMeshAgent.SetDestination(point);
    }
    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 || (!serverController.navMeshAgent.isPathStale && serverController.navMeshAgent.remainingDistance <= 1f))
        {
            serverController.ChangeState(MonsterState.Idle);
            return;
        }
        serverController.RecoverHP();
        // 搜索玩家
        PlayerServerController player = serverController.SearchPlayer();
        if (player != null)
        {
            serverController.SetTargetPlayer(player);
            serverController.ChangeState(MonsterState.Pursuit);
        }
    }
    public override void Exit()
    {
        serverController.StopMove();
    }
}
