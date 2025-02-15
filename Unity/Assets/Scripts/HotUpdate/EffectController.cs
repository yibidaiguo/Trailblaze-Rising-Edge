using JKFrame;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        this.GameObjectPushPool();
    }
}
