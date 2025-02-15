using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #if UNITY_EDITOR || UNITY_SERVER
    private new Collider collider;
    private HashSet<IHitTarget> hitTargets = new HashSet<IHitTarget>();
    private Action<IHitTarget, Vector3> onHitAction;
    public void Init(string layer, Action<IHitTarget, Vector3> onHit)
    {
        this.onHitAction = onHit;
        gameObject.layer = LayerMask.NameToLayer(layer);
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }
    public void StartHit()
    {
        collider.enabled = true;
    }
    public void StopHit()
    {
        collider.enabled = false;
        hitTargets.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        IHitTarget target = other.GetComponentInParent<IHitTarget>();
        if (target != null && !hitTargets.Contains(target))
        {
            hitTargets.Add(target);
            Vector3 point = other.ClosestPoint(transform.position); // 命中点
            onHitAction?.Invoke(target, point);
        }
    }
    #endif
}
