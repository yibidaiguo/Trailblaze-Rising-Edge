using JKFrame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/DialogConfig")]
public class DialogConfig : ConfigBase
{
    [ListDrawerSettings(ShowIndexLabels = true, ShowPaging = false)]
    public List<DialogStepConfig> stepList = new List<DialogStepConfig>();
}

public class DialogStepConfig
{
    public bool player;
    public Dictionary<LanguageType, string> contentDic;
}
