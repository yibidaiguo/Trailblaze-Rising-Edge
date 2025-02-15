using JKFrame;
using UnityEngine;

public class MonsterClientController : CharacterClientControllerBase<MonsterController>, IMonsterClientController
{
    public Transform floatInfoPoint { get; private set; }
    private MonsterFloatInfo floatInfo;
    public override void FirstInit()
    {
        base.FirstInit();
        floatInfoPoint = transform.Find("FloatPoint");
        floatInfo = ResSystem.InstantiateGameObject<MonsterFloatInfo>(floatInfoPoint, nameof(MonsterFloatInfo));
        floatInfo.transform.localScale = Vector3.one;
        mainController.View.footStepAction += View_footStepAction;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        floatInfo.Init(mainController.monsterConfig);
        OnHpChanged(0, mainController.currentHp.Value);
    }
    private void View_footStepAction()
    {
        AudioClip audioClip = ClientGlobal.Instance.Config.monsterFootStepAudioList[mainController.monsterConfig.audioGroupIndex][Random.Range(0, ClientGlobal.Instance.Config.playerFootStepAudios.Length)];
        AudioSystem.PlayOneShot(audioClip, transform.position);
    }
    public override void Init()
    {

    }

    protected override void OnHpChanged(float previousValue, float newValue)
    {
        float fillAmount = newValue / mainController.monsterConfig.maxHP;
        floatInfo.UpdateHp(fillAmount);
    }
}
