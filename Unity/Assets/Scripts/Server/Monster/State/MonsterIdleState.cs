using UnityEngine;

public class MonsterIdleState : MonsterStateBase
{
    private float timer;
    public override void Enter()
    {
        serverController.PlayAnimation("Idle");
        timer = Random.Range(config.maxIdleTime / 2f, config.maxIdleTime);
    }
    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            serverController.ChangeState(MonsterState.Patrol);
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
}
