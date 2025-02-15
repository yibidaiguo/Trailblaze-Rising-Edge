using JKFrame;
using System.Collections;
using Unity.Netcode.Components;
using UnityEngine;

public abstract class CharacterServerControllerBase<M> : MonoBehaviour, ICharacterServerController, IStateMachineOwner where M : CharacterControllerBase
{
    public Animator animator { get; protected set; }
    public NetworkAnimator networkAnimator { get; protected set; }
    public M mainController { get; protected set; }
    public Vector2Int currentAOICoord { get; protected set; }
    public WeaponController weapon { get; protected set; }

    protected StateMachine stateMachine;
    public int skillConfigIndex { get; set; }
    public SkillConfig skillConfig => mainController.skillConfigList[skillConfigIndex];
    public virtual void FirstInit()
    {
        this.mainController = GetComponent<M>();
        animator = mainController.CharacterView.GetComponent<Animator>();
        networkAnimator = animator.GetComponent<NetworkAnimator>();
        stateMachine = new StateMachine();
        mainController.SetSideController(this);
    }
    public virtual void Init()
    {
        currentAnimation = "Idle";
    }
    public virtual void OnNetworkSpawn()
    {
        stateMachine.Init(this);
        StartCoroutine(DoInitAOI());
    }
    public virtual void OnNetworkDespawn()
    {
        stateMachine.Destroy();
        OnRemoveAOI();
    }

    #region AOI
    private IEnumerator DoInitAOI()
    {
        yield return CoroutineTool.WaitForFrame();
        currentAOICoord = AOIManager.Instance.GetCoordByWorldPostion(transform.position);
        OnInitAOI();
    }
    protected abstract void OnInitAOI();
    protected abstract void OnRemoveAOI();

    public void UpdateAOICoord()
    {
        Vector2Int newCoord = AOIManager.Instance.GetCoordByWorldPostion(transform.position);
        if (newCoord != currentAOICoord) // 发生了地图块坐标变化
        {
            OnUpdateAOI(newCoord);
            currentAOICoord = newCoord;
        }
    }

    protected abstract void OnUpdateAOI(Vector2Int newCoord);
    #endregion

    #region 动画
    public string currentAnimation { get; protected set; }
    public void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName) return;
        networkAnimator.ResetTrigger(currentAnimation);
        currentAnimation = animationName;
        networkAnimator.SetTrigger(animationName);
    }
    public bool CheckAnimationState(string stateName, out float normalizedTime)
    {
        // 优先下一个状态
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);
        if (nextInfo.IsName(stateName))
        {
            normalizedTime = nextInfo.normalizedTime;
            return true;
        }
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        normalizedTime = currentInfo.normalizedTime;
        return currentInfo.IsName(stateName);
    }
    #endregion


}
