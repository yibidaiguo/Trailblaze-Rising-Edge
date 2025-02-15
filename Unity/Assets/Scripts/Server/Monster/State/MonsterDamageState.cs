using UnityEngine;

public class MonsterDamageState : MonsterStateBase
{
    private float repelTimer;
    private float repelSpeed;
    private Vector3 repelDir;
    public override void Enter()
    {
        serverController.PlayAnimation("Damage");
    }

    public void SetAttackData(AttackData attackData)
    {
        repelTimer = attackData.repelTime;
        repelSpeed = attackData.repelTime > 0 ? attackData.repelDistance / attackData.repelTime : 0;
        repelDir = serverController.transform.position - attackData.sourcePosition;
        repelDir.Normalize();
    }

    public override void Update()
    {
        repelTimer -= Time.deltaTime;
        // 击退完成了，并且动画也结束了
        if (repelTimer <= 0 && (serverController.CheckAnimationState("Damage", out float normalizedTime) && normalizedTime >= 0.95f))
        {
            if (serverController.CheckTargetPlayer())
            {
                serverController.ChangeState(MonsterState.Pursuit);
            }
            else
            {
                serverController.ChangeState(MonsterState.Patrol);
            }
        }
        else if (repelTimer > 0)
        {
            Vector3 motion = repelSpeed * repelDir;
            motion.y = -5f;
            serverController.characterController.Move(motion * Time.deltaTime);
        }
    }
}