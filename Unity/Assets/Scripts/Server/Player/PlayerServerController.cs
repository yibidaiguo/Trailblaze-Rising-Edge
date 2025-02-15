using JKFrame;
using System;
using UnityEngine;

public class PlayerServerController : CharacterServerControllerBase<PlayerController>, IPlayerServerController, IStateMachineOwner, IHitTarget
{

    public class InputData
    {
        public Vector3 moveDir;
        public bool jump;
        public bool attack;
    }
    public float rootMotionMoveSpeedMultiply { get; private set; } // 动画根运动的系数
    public float ariMoveSpeed { get; private set; }
    public float gravity { get; private set; }
    public float rotateSpeed { get; private set; }
    public float jumpHeightMultiply { get; private set; }
    public CharacterController characterController { get; private set; }
    public InputData inputData { get; private set; }
    public WeaponConfig weaponConfig { get; private set; }
    public bool Living => gameObject.activeInHierarchy && mainController.currentHp.Value > 0;
    public Vector3 centerPos => transform.position + new Vector3(0, 1f, 0);

    public override void FirstInit()
    {
        base.FirstInit();
        characterController = GetComponent<CharacterController>();
        inputData = new InputData();
        rootMotionMoveSpeedMultiply = ServerGlobal.Instance.ServerConfig.rootMotionMoveSpeedMultiply;
        ariMoveSpeed = ServerGlobal.Instance.ServerConfig.playerAriMoveSpeed;
        gravity = ServerGlobal.Instance.ServerConfig.playerGravity;
        rotateSpeed = ServerGlobal.Instance.ServerConfig.playerRotateSpeed;
        jumpHeightMultiply = ServerGlobal.Instance.ServerConfig.playerJumpHeightMultiply;
        mainController.onUpdateWeaponObjectAction += MainController_onUpdateWeaponObjectAction;
    }
    public override void Init()
    {
        base.Init();
        skillConfigIndex = -1; // 为了首次攻击+1后为0
    }

    private void MainController_onUpdateWeaponObjectAction(GameObject obj)
    {
        if (!obj.TryGetComponent(out WeaponController temp))
        {
            temp = obj.AddComponent<WeaponController>();
        }
        weapon = temp;
        weapon.Init("PlayerWeapon", OnHit);
        weaponConfig = ServerResSystem.GetItemConfig<WeaponConfig>(mainController.usedWeaponName.Value.ToString());
    }

    public void ChangeState(PlayerState newState)
    {
        mainController.currentState.Value = newState;
        switch (newState)
        {
            case PlayerState.Idle:
                stateMachine.ChangeState<PlayerIdleState>();
                break;
            case PlayerState.Move:
                stateMachine.ChangeState<PlayerMoveState>();
                break;
            case PlayerState.Jump:
                stateMachine.ChangeState<PlayerJumpState>();
                break;
            case PlayerState.AirDown:
                stateMachine.ChangeState<PlayerAriDownState>();
                break;
            case PlayerState.Attack:
                stateMachine.ChangeState<PlayerAttackState>();
                break;
            case PlayerState.BeHit:
                stateMachine.ChangeState<PlayerBeHitState>();
                break;
        }
    }
    #region 网络
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ChangeState(PlayerState.Idle);
    }
    protected override void OnInitAOI()
    {
        AOIManager.Instance.InitClient(mainController.OwnerClientId, currentAOICoord);
    }
    protected override void OnUpdateAOI(Vector2Int newCoord)
    {
        AOIManager.Instance.UpdateClientChunkCoord(mainController.OwnerClientId, currentAOICoord, newCoord);
    }
    protected override void OnRemoveAOI()
    {
        AOIManager.Instance.RemoveClient(mainController.OwnerClientId, currentAOICoord);
    }
    #endregion

    #region 响应客户端的输入
    public void ReceiveMoveInput(Vector3 moveDir)
    {
        inputData.moveDir = moveDir.normalized;
        // 状态类中根据输入情况进行运算
    }
    public void ReceiveJumpInput()
    {
        switch (mainController.currentState.Value)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                inputData.jump = true;
                break;
            default:
                inputData.jump = false;
                break;
        }
    }
    public void ReceiveAttackInput(bool value)
    {
        switch (mainController.currentState.Value)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
            case PlayerState.Attack:
                inputData.attack = value;
                break;
            default:
                inputData.attack = false;
                break;
        }
    }
    #endregion

    #region 战斗
    public void OnHit(IHitTarget target, Vector3 point)
    {
        // 服务端只处理伤害、AI的状态逻辑
        AttackData attackData = new AttackData
        {
            attackValue = skillConfig.attackValueMultiple * weaponConfig.attackValue,
            repelDistance = skillConfig.repelDistance,
            repelTime = skillConfig.repelTime,
            sourcePosition = transform.position
        };
        // 通知客户端播放效果
        mainController.PlaySkillHitEffectClientRpc(point);
        // 通知怪物受伤
        // 检测关于击杀怪物的任务
        if (target.BeHit(attackData))
        {
            string monsterId = ((MonsterServerController)target).mainController.monsterConfig.name;
            ClientsManager.Instance.CheckAndAddStruckDownTaskProgress(mainController.OwnerClientId, monsterId);
        }
    }

    public bool BeHit(AttackData attackData)
    {
        if (mainController.currentState.Value == PlayerState.Die) return true;
        AddHp(-attackData.attackValue);

        if (mainController.currentHp.Value > 0)
        {
            ChangeState(PlayerState.BeHit);
        }
        else
        {
            ChangeState(PlayerState.Die);
        }
        return mainController.currentHp.Value < 0;
    }

    public void AddHp(float add)
    {
        float hp = mainController.currentHp.Value;
        hp = Mathf.Clamp(hp + add, 0, ServerGlobal.Instance.ServerConfig.playerMaxHp);
        mainController.currentHp.Value = hp;
    }

    public void OnDieAnimationEnd()
    {
        ClientsManager.Instance.OnPlayerDie(this);
        ChangeState(PlayerState.Idle);
    }
    #endregion

}
