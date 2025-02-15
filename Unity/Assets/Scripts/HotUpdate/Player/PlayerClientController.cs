using JKFrame;
using UnityEngine;

public class PlayerClientController : CharacterClientControllerBase<PlayerController>, IPlayerClientController
{
    public Transform cameraLooakTarget { get; private set; }
    public Transform cameraFollowTarget { get; private set; }
    public Transform floatInfoPoint { get; private set; }
    private PlayerFloatInfo floatInfo;
    public bool canControl; // 是否可以控制

    public override void FirstInit() // 第一次被添加组件时调用
    {
        base.FirstInit();
        cameraLooakTarget = transform.Find("CameraLookat");
        cameraFollowTarget = transform.Find("CameraFollow");
        floatInfoPoint = transform.Find("FloatPoint");
        mainController.View.footStepAction += View_footStepAction;
    }
    public override void Init()
    {
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (mainController.IsOwner) // 本地玩家看不到自己的名字
        {
            if (floatInfo != null) floatInfo.gameObject.SetActive(false);
        }
        else
        {
            if (floatInfo == null) floatInfo = ResSystem.InstantiateGameObject<PlayerFloatInfo>(floatInfoPoint, "PlayerFloatInfo");
            floatInfo.UpdateName(mainController.playerName.Value.ToString());
        }
        OnHpChanged(0, mainController.currentHp.Value);
    }
    private void View_footStepAction()
    {
        AudioClip audioClip = ClientGlobal.Instance.Config.playerFootStepAudios[Random.Range(0, ClientGlobal.Instance.Config.playerFootStepAudios.Length)];
        AudioSystem.PlayOneShot(audioClip, transform.position);
    }
    protected override void OnHpChanged(float previousValue, float newValue)
    {
        float fillAmount = newValue / ClientGlobal.Instance.Config.playerMaxHp;
        if (mainController.IsOwner)
        {
            UISystem.Show<UI_PlayerInfoWindow>().UpdateHP(fillAmount);
        }
        else
        {
            floatInfo.UpdateHp(fillAmount);
        }
    }

    #region 输入
    private void Update()
    {
        if (!mainController.IsSpawned && gameObject.activeInHierarchy)
        {
            NetManager.Instance.DestroyObject(mainController.NetworkObject);
        }
        if (!mainController.IsOwner) return;
        switch (mainController.currentState.Value)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                UpdateMoveInput();
                UpdateJumpInput();
                UpdateAttackInput();
                break;
            case PlayerState.Jump:
                UpdateMoveInput();
                break;
            case PlayerState.AirDown:
                UpdateMoveInput();
                UpdateAttackInput();
                break;
            case PlayerState.Attack:
                UpdateMoveInput();
                UpdateAttackInput();
                break;
        }
    }
    private Vector3 lastInputDir = Vector3.zero;

    private void UpdateMoveInput()
    {
        Vector3 inputDir = Vector3.zero;
        if (canControl)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            inputDir = new Vector3(h, 0, v);
        }
        if (inputDir == Vector3.zero && lastInputDir == Vector3.zero) return;
        lastInputDir = inputDir;
        float cameraEulerAngleY = Camera.main.transform.eulerAngles.y;
        // 四元数和向量相乘：让这个向量按照四元数所表达的角度进行旋转后得到一个新的向量
        mainController.SendInputMoveDirServerRpc(Quaternion.Euler(0, cameraEulerAngleY, 0) * inputDir);
    }

    private void UpdateJumpInput()
    {
        if (!canControl) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mainController.SendJumpInputServerRpc();
        }
    }

    private bool lastAttackInput = false;
    private void UpdateAttackInput()
    {
        if (!canControl)
        {
            if (lastAttackInput)
            {
                SetAttackInput(false);
            }
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            SetAttackInput(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetAttackInput(false);
        }
    }

    private void SetAttackInput(bool value)
    {
        mainController.SendAttackInputServerRpc(value);
        lastAttackInput = value;
    }


    #endregion
}
