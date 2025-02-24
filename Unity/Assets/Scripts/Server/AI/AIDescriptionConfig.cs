using JKFrame;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/AI/AIDescriptionConfig")]
public class AIDescriptionConfig : ConfigBase
{
   public string AIName;
   [Multiline(3)]
   public string AIDescription;
   // 对话参数
   [Header("Dialogue Settings")] [Range(0, 2)]
   public float temperature = 0.5f; // 控制生成文本的随机性（0-2，值越高越随机）

   [Range(1, 1000)] public int maxTokens = 100; // 生成的最大令牌数（控制回复长度）
}
