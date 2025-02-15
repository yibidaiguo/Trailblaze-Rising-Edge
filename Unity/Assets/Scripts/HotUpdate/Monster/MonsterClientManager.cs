using JKFrame;

public class MonsterClientManager : SingletonMono<MonsterClientManager>
{
    public void Init()
    {
        EventSystem.AddTypeEventListener<SpawnMonsterEvent>(OnSpawnMonsterEvent);
    }

    private void OnSpawnMonsterEvent(SpawnMonsterEvent arg)
    {

    }
}
