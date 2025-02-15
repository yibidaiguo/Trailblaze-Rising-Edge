using Cinemachine;
using JKFrame;
using UnityEngine;

public class CameraManager : SingletonMono<CameraManager>
{
    [SerializeField] private CinemachineFreeLook cinemachine;
    private string xInputAxisName = "Mouse X";
    private string yInputAxisName = "Mouse Y";
    public void Init(Transform cameraLooakTarget, Transform cameraFollowTarget)
    {
        cinemachine.LookAt = cameraLooakTarget;
        cinemachine.Follow = cameraFollowTarget;
        SetControlState(true);
    }
    public void SetControlState(bool control)
    {
        cinemachine.m_XAxis.m_InputAxisName = control ? xInputAxisName : "";
        cinemachine.m_YAxis.m_InputAxisName = control ? yInputAxisName : "";
        if (!control)
        {
            cinemachine.m_XAxis.m_InputAxisValue = 0;
            cinemachine.m_YAxis.m_InputAxisValue = 0;
        }
        ImmediatelyPosition();
    }
    public void ImmediatelyPosition()
    {
        if (cinemachine.Follow != null)
        {
            cinemachine.transform.position = cinemachine.Follow.transform.position;
        }
    }
}
