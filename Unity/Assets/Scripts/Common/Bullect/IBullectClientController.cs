
using UnityEngine;

public interface IBullectClientController : INetworkController
{
    public void PlayHitEffect(Vector3 point);
    public void PlayReleaseEffect();
}
