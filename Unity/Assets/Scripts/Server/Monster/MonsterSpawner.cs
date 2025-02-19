using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[OnClientBuild(ComponentMode.Delete)]
public  class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public float patrolRange = 10;
    public float interval = 10;
    private float halfPatrolRange;
    private MonsterServerController[] monsters;
    private float timer;
    public void Init()
    {
#if UNITY_EDITOR
        if (!NetManager.Instance.IsServer) return;
#endif
        timer = interval;
        monsters = new MonsterServerController[monsterPrefabs.Length];
        halfPatrolRange = patrolRange / 2f;
        for (int i = 0; i < monsterPrefabs.Length; i++)
        {
            Spawn(i);
        }
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (!NetManager.Instance.IsServer) return;
#endif
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = interval;
            for (int i = 0; i < monsters.Length; i++)
            {
                if (monsters[i] == null)
                {
                    Spawn(i);
                    return;
                }
            }
        }
    }

    private void Spawn(int index)
    {
        MonsterServerController serverController = NetManager.Instance.SpawnObject<MonsterServerController>(NetManager.ServerClientId, monsterPrefabs[index], GetPatrolPoint(), Quaternion.identity);
        serverController.SetMonsterSpawner(this, index);
        serverController.mainController.NetworkObject.SpawnWithOwnership(NetManager.ServerClientId);
        monsters[index] = serverController;
    }

    public Vector3 GetPatrolPoint()
    {
        Vector3 point = transform.position + new Vector3(Random.Range(-halfPatrolRange, halfPatrolRange), 0, Random.Range(-halfPatrolRange, halfPatrolRange));
        if (NavMesh.SamplePosition(point, out NavMeshHit hitInfo, 10f, NavMesh.AllAreas))
        {
            return hitInfo.position;
        }
        else
        {
            return transform.position;
        }
    }

    public void OnMonsterDie(int index)
    {
        monsters[index] = null;
    }
}

