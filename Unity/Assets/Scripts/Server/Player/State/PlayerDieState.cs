using UnityEngine;

public class PlayerDieState : PlayerStateBase
{
    private float timer;
    public override void Enter()
    {
        serverController.PlayAnimation("Die");
        timer = ServerResSystem.serverConfig.playerBeHitTime;
    }
    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            serverController.OnDieAnimationEnd();
        }
    }
}
