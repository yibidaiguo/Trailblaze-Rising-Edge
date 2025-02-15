using JKFrame;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/BullectConfig")]
public class BullectConfig : ConfigBase
{
    public float moveSpeed;
    public float time;
    public SkillEffect releaseEffect;
    public SkillEffect hitEffect;
}
