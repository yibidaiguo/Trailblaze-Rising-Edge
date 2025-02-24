using JKFrame;

public class
    MonsterController : CharacterControllerBase<MonsterView, IMonsterClientController, IMonsterServerController>
{
    public MonsterConfig monsterConfig;
    public NetVaribale<MonsterState> currentState = new NetVaribale<MonsterState>(MonsterState.None);

    public override void OnNetworkSpawn()
    {
#if !UNITY_SERVER || UNITY_EDITOR
        if (IsClient) EventSystem.TypeEventTrigger(new SpawnMonsterEvent { newMonster = this });
#endif
        base.OnNetworkSpawn();
    }
}