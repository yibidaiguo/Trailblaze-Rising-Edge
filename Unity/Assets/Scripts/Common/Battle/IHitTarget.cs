public interface IHitTarget
{
#if UNITY_EDITOR || UNITY_SERVER
    // 返回值代表这一次被攻击是否被击杀
    public bool BeHit(AttackData attackData);
#endif
}

#if UNITY_EDITOR || UNITY_SERVER
public struct AttackData
{
    public float attackValue;
    public float repelDistance;
    public float repelTime;
    public UnityEngine.Vector3 sourcePosition;
}
#endif