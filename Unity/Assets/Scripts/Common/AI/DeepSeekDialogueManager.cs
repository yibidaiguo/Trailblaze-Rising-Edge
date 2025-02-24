using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class DeepSeekDialogueManager : MonoBehaviour
{
    [FormerlySerializedAs("aiConfig")] [Header("API Settings")] [SerializeField]
    private AIKeyConfig aiKeyConfig;

    private Dictionary<int, Message> messagesDic = new();
    private int currentMessageId;

    // 对话参数
    [Header("Dialogue Settings")] [Range(0, 2)]
    public float temperature = 0.5f; // 控制生成文本的随机性（0-2，值越高越随机）

    [Range(1, 1000)] public int maxTokens = 100; // 生成的最大令牌数（控制回复长度）

    // 角色设定
    [System.Serializable]
    public class NPCCharacter
    {
        public string name = "";
        [TextArea(3, 10)] public string personalityPrompt = ""; // 角色设定提示词
    }

    [SerializeField] public NPCCharacter npcCharacter;

    // 回调委托，用于异步处理API响应
    public delegate void DialogueCallback(string content, bool isSuccess);

    // Start is called before the first frame update
    void Start()
    {
        InitmessageDic();
    }

    public void SendMessageToDeepSeek(string message, DialogueCallback callback)
    {
        StartCoroutine(PostRequest(message, callback));
    }

    private void InitmessageDic()
    {
        messagesDic.Clear();
        messagesDic.Add(0, new Message { role = "system", content = npcCharacter.personalityPrompt });
        currentMessageId = 0;
    }

    /// <summary>
    /// 构建消息列表
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns></returns>
    private List<Message> BuildMessage(string message)
    {
        AddMessageDic("user",message);
        List<Message> messages = messagesDic.Values.ToList();
        return messages;
    }

    /// <summary>
    /// 增加消息构建列表内容
    /// </summary>
    /// <param name="message">消息</param>
    private void AddMessageDic(string role, string message)
    {
        currentMessageId++;
        messagesDic.Add(currentMessageId, 
            new Message { role = role, content = message });
    }
    
    /// <summary>
    ///  处理对话请求的协程
    /// </summary>
    /// <param name="message">玩家的输入内容<</param>
    /// <param name="callback">回调函数，用于处理API响应</param>
    /// <returns></returns>
    IEnumerator PostRequest( string message, DialogueCallback callback)
    {
        // 构建消息列表，包含系统提示和用户输入
        List<Message> messages = BuildMessage(message);

        // 构建请求体
        ChatRequest requestBody = new ChatRequest
        {
            model = aiKeyConfig.modelName, // 模型名称
            messages = messages, // 消息列表
            temperature = temperature, // 温度参数
            max_tokens = maxTokens // 最大令牌数
        };


        // 使用Newtonsoft.Json序列化
        //string jsonBody = JsonConvert.SerializeObject(requestBody);
        string jsonBody = JsonUtility.ToJson(requestBody);

        Debug.Log(jsonBody);
        //yield return null;
        // 创建UnityWebRequest
        UnityWebRequest request = CreateWebRequest(jsonBody);
        yield return request.SendWebRequest();

        if (IsRequestError(request))
        {
            if (request.responseCode == 429) // 速率限制
            {
                Debug.LogWarning("速率限制达到，延迟重试中...");
                yield return new WaitForSeconds(5); // 延迟5秒后重试
                StartCoroutine(PostRequest(message, callback));
                yield break;
            }
            else
            {
                Debug.LogError($"API Error: {request.responseCode}\n{request.downloadHandler.text}");
                callback?.Invoke($"API请求失败：{request.downloadHandler.text}", false);
                yield break;
            }
        }

        Debug.Log(request.downloadHandler.text);
        DeepSeekResponse response = ParseResponse(request.downloadHandler.text);
        
        if (response != null && response.choices.Length > 0)
        {
            Debug.Log("Reply " + request.downloadHandler.text);
            string npcReply = response.choices[0].message.content;
            Debug.Log(npcReply);
            AddMessageDic("assistant",npcReply);
            callback?.Invoke(npcReply, true);
        }
        else
        {
            callback?.Invoke(name + "（陷入沉默）", false);
        }

        request.Dispose(); // 确保释放UnityWebRequest
    }

    /// <summary>
    /// 创建UnityWebRequest对象
    /// </summary>
    /// <param name="jsonBody">请求体的JSON字符串</param>
    /// <returns>配置好的UnityWebRequest对象</returns>
    private UnityWebRequest CreateWebRequest(string jsonBody)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        UnityWebRequest request = new UnityWebRequest(aiKeyConfig.apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw); // 设置上传处理器
        request.downloadHandler = new DownloadHandlerBuffer(); // 设置下载处理器
        request.SetRequestHeader("Content-Type", "application/json"); // 设置请求头
        request.SetRequestHeader("Authorization", $"Bearer {aiKeyConfig.apiKey}"); // 设置认证头
        request.SetRequestHeader("Accept", "application/json"); // 设置接受类型
        return request;
    }

    /// <summary>
    /// 检查请求是否出错
    /// </summary>
    /// <param name="request">UnityWebRequest对象</param>
    /// <returns>如果请求出错返回true，否则返回false</returns>
    private bool IsRequestError(UnityWebRequest request)
    {
        return request.result == UnityWebRequest.Result.ConnectionError ||
               request.result == UnityWebRequest.Result.ProtocolError ||
               request.result == UnityWebRequest.Result.DataProcessingError;
    }

    /// <summary>
    /// 解析API响应
    /// </summary>
    /// <param name="jsonResponse">API响应的JSON字符串</param>
    /// <returns>解析后的DeepSeekResponse对象</returns>
    private DeepSeekResponse ParseResponse(string jsonResponse)
    {
        try
        {
            DeepSeekResponse response = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
            if (response == null || response.choices == null || response.choices.Length == 0)
            {
                Debug.LogError("API响应格式错误或未包含有效数据。");
                return null;
            }
            return response;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON解析失败: {e.Message}\n响应内容：{jsonResponse}");
            return null;
        }
    }


    // 可序列化数据结构
    [System.Serializable]
    private class ChatRequest
    {
        public string model; // 模型名称
        public List<Message> messages; // 消息列表
        public float temperature; // 温度参数
        public int max_tokens; // 最大令牌数
    }

    [System.Serializable]
    public class Message
    {
        public string role; // 角色（system/user/assistant）
        public string content; // 消息内容
    }

    [System.Serializable]
    private class Choice
    {
        public Message message; // 生成的消息
    }

    [System.Serializable]
    private class DeepSeekResponse
    {
        public Choice[] choices; // 生成的选择列表
    }
}