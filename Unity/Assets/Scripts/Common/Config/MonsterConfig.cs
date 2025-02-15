using JKFrame;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/MonsterConfig")]
public class MonsterConfig : ConfigBase
{
    public Dictionary<LanguageType, string> nameDic;
    public float maxHP;
    public float attackValue;
    public float maxIdleTime;
    public float maxPatrolTime;
    public float searchPlayerRange;
    public float pursuitTime;
    public int audioGroupIndex;
    public float attackRange;
    public float attackCD;

}
