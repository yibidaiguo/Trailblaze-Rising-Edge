using JKFrame;
using UnityEngine;
using UnityEngine.AI;

public class MonsterServerController : CharacterServerControllerBase<MonsterController>, IMonsterServerController, IStateMachineOwner, IHitTarget
{
    public const float recoverHPRate = 0.2f;
    public const float attackRotateSpeed = 1000f;
    public NavMeshAgent navMeshAgent { get; private set; }
    public CharacterController characterController { get; private set; }
    public MonsterSpawner monsterSpawner { get; private set; }
    public MonsterConfig monsterConfig { get => mainController.monsterConfig; }
    private int indexAtSpawner;
    public override void FirstInit()
    {
        base.FirstInit();
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        weapon = GetComponentInChildren<WeaponController>();
        weapon?.Init("MonsterWeapon", OnHit);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        mainController.currentHp.Value = monsterConfig.maxHP;
        ChangeState(MonsterState.Idle);
    }
    public void SetMonsterSpawner(MonsterSpawner monsterSpawner, int indexAtSpawner)
    {
        this.monsterSpawner = monsterSpawner;
        this.indexAtSpawner = indexAtSpawner;
    }

    public void ChangeState(MonsterState newState)
    {
        mainController.currentState.Value = newState;
        switch (newState)
        {
            case MonsterState.Idle:
                stateMachine.ChangeState<MonsterIdleState>();
                break;
            case MonsterState.Patrol:
                stateMachine.ChangeState<MonsterPatrolState>();
                break;
            case MonsterState.Pursuit:
                stateMachine.ChangeState<MonsterPursuitState>();
                break;
            case MonsterState.Damage:
                stateMachine.ChangeState<MonsterDamageState>();
                break;
            case MonsterState.Attack:
                stateMachine.ChangeState<MonsterAttackState>();
                break;
            case MonsterState.Die:
                stateMachine.ChangeState<MonsterDieState>();
                break;
        }
    }

    #region AOI
    protected override void OnInitAOI()
    {
        AOIManager.Instance.InitServerObject(mainController.NetworkObject, currentAOICoord);
    }
    protected override void OnUpdateAOI(Vector2Int newCoord)
    {
        AOIManager.Instance.UpdateServerObjectChunkCoord(mainController.NetworkObject, currentAOICoord, newCoord);
    }
    protected override void OnRemoveAOI()
    {
        AOIManager.Instance.RemoveServerObject(mainController.NetworkObject, currentAOICoord);
    }
    #endregion

    #region 移动控制
    public void StartMove()
    {
        navMeshAgent.enabled = true;
    }
    public void StopMove()
    {
        navMeshAgent.enabled = false;
    }
    public Vector3 GetPatrolPoint()
    {
        return monsterSpawner.GetPatrolPoint();
    }
    #endregion

    #region 搜索玩家
    private float lastSearchPlayerTime;
    private Collider[] hitCollider = new Collider[1];
    private const float searchPlayerInterval = 0.5f;
    public PlayerServerController SearchPlayer(bool checkTime = true)
    {
        if (checkTime)
        {
            if (Time.time - lastSearchPlayerTime < searchPlayerInterval)
            {
                return null;
            }
            lastSearchPlayerTime = Time.time;
        }
        int count = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, 1, 0), monsterConfig.searchPlayerRange, hitCollider, ServerGlobal.Instance.PlayerLayerMask);
        if (count != 0)
        {
            return hitCollider[0].GetComponentInParent<PlayerServerController>();
        }
        return null;
    }

    #endregion
    #region 战斗
    public PlayerServerController targetPlayer { get; private set; }
    public void SetTargetPlayer(PlayerServerController targetPlayer)
    {
        this.targetPlayer = targetPlayer;
    }
    public bool CheckTargetPlayer()
    {
        if (targetPlayer != null)
        {
            if (targetPlayer.Living)
            {
                return true;
            }
            else SetTargetPlayer(null);
        }
        return false;
    }

    public bool BeHit(AttackData attackData)
    {
        if (mainController.currentHp.Value <= 0) return false;
        float hp = mainController.currentHp.Value;
        hp -= attackData.attackValue;
        if (hp < 0) hp = 0;
        mainController.currentHp.Value = hp;
        if (hp <= 0)
        {
            ChangeState(MonsterState.Die);
            return true;
        }
        else
        {
            ChangeState(MonsterState.Damage);
            ((MonsterDamageState)stateMachine.currStateObj).SetAttackData(attackData);
            return false;
        }
    }

    public void RecoverHP()
    {
        float hp = mainController.currentHp.Value;
        if (hp < monsterConfig.maxHP)
        {
            hp += monsterConfig.maxHP * recoverHPRate * Time.deltaTime;
            mainController.currentHp.Value = hp;
        }
    }

    private float lastAttackTime;
    public bool CheckAttack()
    {
        return Time.time - lastAttackTime > monsterConfig.attackCD;
    }
    public void OnAttack()
    {
        lastAttackTime = Time.time;
    }

    public void OnHit(IHitTarget target, Vector3 point)
    {
        // 服务端只处理伤害、AI的状态逻辑
        AttackData attackData = new AttackData
        {
            attackValue = skillConfig.attackValueMultiple * monsterConfig.attackValue,
            repelDistance = skillConfig.repelDistance,
            repelTime = skillConfig.repelTime,
            sourcePosition = transform.position
        };
        // 通知客户端播放效果
        mainController.PlaySkillHitEffectClientRpc(point);
        // 通知玩家受伤
        target.BeHit(attackData);
    }

    public void Die()
    {
        NetManager.Instance.DestroyObject(mainController.NetworkObject);
        monsterSpawner.OnMonsterDie(indexAtSpawner);
    }

    public void Shoot()
    {
        if (targetPlayer == null) return;
        Vector3 pos = transform.TransformPoint(skillConfig.skillBullect.offset);
        Vector3 dir = targetPlayer.centerPos - pos;
        BullectServerController bullect = NetManager.Instance.SpawnObject<BullectServerController>(NetManager.ServerClientId, skillConfig.skillBullect.prefab, pos, Quaternion.LookRotation(dir));
        bullect.mainController.NetworkObject.SpawnWithOwnership(NetManager.ServerClientId);
        AttackData attackData = new AttackData
        {
            attackValue = skillConfig.attackValueMultiple * monsterConfig.attackValue,
            repelDistance = skillConfig.repelDistance,
            repelTime = skillConfig.repelTime,
            sourcePosition = transform.position
        };
        bullect.Init(attackData, "MonsterWeapon");
    }
    #endregion
}
