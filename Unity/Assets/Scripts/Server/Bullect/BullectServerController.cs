using JKFrame;
using System.Collections;
using UnityEngine;

public class BullectServerController : MonoBehaviour, IBullectServerController
{
    private float timer;
    public BullectController mainController { get; private set; }
    private Vector2Int currentAOICoord;
    private AttackData attackData;
    private BullectConfig config => mainController.config;
    public void FirstInit()
    {
        mainController = GetComponent<BullectController>();
    }

    public void Init() { }
    public void Init(AttackData attackData, string layer)
    {
        this.timer = config.time;
        this.attackData = attackData;
        gameObject.layer = LayerMask.NameToLayer(layer);
        isSpawned = false;
    }
    public void OnNetworkSpawn()
    {
        StartCoroutine(DoOnNetworkSpawn());
    }

    private bool isSpawned;
    private IEnumerator DoOnNetworkSpawn()
    {
        yield return CoroutineTool.WaitForFrame();
        AOIManager.Instance.InitServerObject(mainController.NetworkObject, currentAOICoord);
        mainController.OnReleaseClientRpc();
        isSpawned = true;
    }
    private void Update()
    {
        if (!isSpawned) return;
        timer -= Time.deltaTime;
        transform.Translate(config.moveSpeed * Time.deltaTime * Vector3.forward, Space.Self);
        UpdateAOI();
        if (timer <= 0)
        {
            Destroy();
        }
    }

    private void UpdateAOI()
    {
        Vector2Int newCoord = AOIManager.Instance.GetCoordByWorldPostion(transform.position);
        AOIManager.Instance.UpdateServerObjectChunkCoord(mainController.NetworkObject, currentAOICoord, newCoord);
        currentAOICoord = newCoord;
    }

    private void Destroy()
    {
        NetManager.Instance.DestroyObject(mainController.NetworkObject);
    }

    public void OnNetworkDespawn()
    {
        AOIManager.Instance.RemoveServerObject(mainController.NetworkObject, currentAOICoord);
    }

    private void OnTriggerStay(Collider other)
    {
        IHitTarget target = other.GetComponentInParent<IHitTarget>();
        if (target != null)
        {
            Vector3 point = other.ClosestPoint(transform.position); // 命中点
            mainController.OnHitClientRpc(point);
            target.BeHit(attackData);
            Destroy();
        }
    }

}
