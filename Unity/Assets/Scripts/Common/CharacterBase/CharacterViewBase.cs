using Sirenix.OdinInspector;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public abstract class CharacterViewBase : SerializedMonoBehaviour
{
#if UNITY_SERVER || UNITY_EDITOR
    [SerializeField] private Animator animator;

    public event Action<Vector3, Quaternion> rootMotionAction;

    private void OnAnimatorMove()
    {
        rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation);
    }
#endif
    #region 动画事件
    public event Action footStepAction;
    private void FootStep()
    {
        footStepAction?.Invoke();
    }

    public event Action startSkillHitAction;
    public event Action stopSkilllHitAction;
    public event Action shootAction;
    private void StartSkillHit()
    {
        startSkillHitAction?.Invoke();
    }
    private void StopSkillHit()
    {
        stopSkilllHitAction?.Invoke();
    }
    private void Shoot()
    {
        shootAction?.Invoke();
    }
    #endregion

    #region Editor
#if UNITY_EDITOR
    [Button, ContextMenu(nameof(SetAniamtorSettings))]
    public void SetAniamtorSettings()
    {
        AnimatorController animatorController = (AnimatorController)animator.runtimeAnimatorController;
        animatorController.parameters = null;
        AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
        stateMachine.anyStateTransitions = null;
        foreach (ChildAnimatorState state in stateMachine.states)
        {
            string triggerName = state.state.name;
            AnimatorControllerParameter parameter = new AnimatorControllerParameter
            {
                name = triggerName,
                type = AnimatorControllerParameterType.Trigger
            };
            animatorController.AddParameter(parameter);
            AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(state.state);
            transition.AddCondition(AnimatorConditionMode.If, 0.0f, triggerName);
        }
        UnityEditor.EditorUtility.SetDirty(animatorController);
        UnityEditor.AssetDatabase.SaveAssetIfDirty(animatorController);
    }
#endif
    #endregion
}
