using JKFrame;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Skill")]
public class SkillConfig : ConfigBase
{
    public string animationName;
    public float attackValueMultiple;
    public float rotateNormalizedTime = 0.2f;
    public float endNormalizedTime = 0.9f;
    public float switchNormalizedTime = 0.5f;
    public SkillEffect releaseEffect;
    public SkillEffect startHitEffect;
    public float repelDistance;
    public float repelTime;
    public SkillEffect hitEffect;
    public SkillBullect skillBullect;
}

public class SkillEffect
{
    public AudioClip audio;
    public GameObject prefab;
    public Vector3 offset;
    public Vector3 rotation;
    public Vector3 scale;
}

public class SkillBullect
{
    public GameObject prefab;
    public Vector3 offset;
}