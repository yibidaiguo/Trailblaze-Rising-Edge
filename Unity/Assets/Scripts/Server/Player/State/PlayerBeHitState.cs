using UnityEngine;


public class PlayerBeHitState : PlayerStateBase
{
    public bool beHit1;
    private float timer;
    public override void Enter()
    {
        serverController.PlayAnimation("BeHit" + (beHit1 ? "1" : "2"));
        beHit1 = !beHit1;
        timer = ServerResSystem.serverConfig.playerBeHitTime;
    }
    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            serverController.ChangeState(PlayerState.Idle);
        }
    }
}
