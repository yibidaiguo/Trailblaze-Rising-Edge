using Unity.Netcode;
using UnityEngine;
public class BullectController : NetworkEntityBase
{
    public BullectConfig config;

    [ClientRpc]
    public void OnReleaseClientRpc()
    {
        ((IBullectClientController)sideController).PlayReleaseEffect();
    }

    [ClientRpc]
    public void OnHitClientRpc(Vector3 point)
    {
        ((IBullectClientController)sideController).PlayHitEffect(point);
    }
}
