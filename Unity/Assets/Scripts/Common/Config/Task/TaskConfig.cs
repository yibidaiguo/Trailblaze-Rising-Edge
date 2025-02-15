using JKFrame;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/TaskConfig")]
public class TaskConfig : ConfigBase
{
    public Dictionary<LanguageType, string> nameDic;
    public Dictionary<LanguageType, string> descriptionDic;
    public string nextTaskId;
    public TaskInfoBase taskInfo;
    public TaskRewardBase taskReward;
    public Vector3 guidePosition;
}
