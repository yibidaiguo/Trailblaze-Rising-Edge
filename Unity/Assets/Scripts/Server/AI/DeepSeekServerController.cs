using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JKFrame;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class DeepSeekServerController : SingletonMono<DeepSeekServerController>
{
    private Dictionary<string, Message> messagesDic = new();
    private int currentMessageId;

    public void SendMessageToDeepSeek(ulong clientID, string message, string npcName, string playerName)
    {

        if (messagesDic.Count == 0) InitmessageDic(npcName, playerName);
        StartCoroutine(PostRequest(clientID, message, npcName, playerName));
    }

    private void InitmessageDic(string npcName, string playerName)
    {
        messagesDic.Clear();
        //TODO:后面优化
        if (npcName == "Crafter0")
        {
            if (DatabaseManager.Instance.GetCrafterData(playerName) == null) DatabaseManager.Instance.CreateCrafterData(new CrafterData()
            {
                playerName = playerName,
                aiMessagesDic = new Dictionary<string, Message>(),
                currentAiDailogIndex = 0
            });
            CrafterData crafterData = DatabaseManager.Instance.GetCrafterData(playerName);
            if (crafterData.aiMessagesDic.Count == 0)
            {
                crafterData.aiMessagesDic.Add("0",
                    new Message
                    {
                        role = "system", content = ServerResSystem.serverConfig.aIConfigDic[npcName].AIDescription
                    });
                crafterData.currentAiDailogIndex = 0;
            }
               
            messagesDic = crafterData.aiMessagesDic;
            currentMessageId = crafterData.currentAiDailogIndex;
            DatabaseManager.Instance.SaveCrafterData(crafterData);
        }
    }

    /// <summary>
    /// 构建消息列表
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns></returns>
    private List<Message> BuildMessage(string message)
    {
        AddMessageDic("user", message);
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
        messagesDic.Add(currentMessageId.ToString(),
            new Message { role = role, content = message });
    }

    /// <summary>
    ///  处理对话请求的协程
    /// </summary>
    /// <param name="message">玩家的输入内容<</param>
    /// <param name="callback">回调函数，用于处理API响应</param>
    /// <returns></returns>
    IEnumerator PostRequest(ulong clientID, string message, string npcName, string playerName)
    {
        if (message == null) yield break;
        // 构建消息列表，包含系统提示和用户输入
        List<Message> messages = BuildMessage(message);

        // 构建请求体
        ChatRequest requestBody = new ChatRequest
        {
            model = ServerResSystem.serverConfig.aIKeyConfig.modelName, // 模型名称
            messages = messages, // 消息列表
            temperature = ServerResSystem.serverConfig.aIConfigDic[npcName].temperature, // 温度参数
            max_tokens = ServerResSystem.serverConfig.aIConfigDic[npcName].maxTokens // 最大令牌数
        };
        
        string jsonBody = JsonUtility.ToJson(requestBody);
        
        // 创建UnityWebRequest
        UnityWebRequest request = CreateWebRequest(jsonBody);
        
        yield return request.SendWebRequest();
        
        if (IsRequestError(request))
        {
            if (request.responseCode == 429) // 速率限制
            {
                Debug.LogWarning("速率限制达到，延迟重试中...");
                yield return new WaitForSeconds(5); // 延迟5秒后重试
                StartCoroutine(PostRequest(clientID, message, npcName, playerName));
                yield break;
            }
            else
            {
                NetMessageManager.Instance.SendMessageToClient(NetMessageType.S_C_AIAnswer,
                    new S_C_AIAnswer() { message = $"API Error: {request.responseCode}\n{request.downloadHandler.text}" },
                    clientID);
                yield break;
            }
        }
        
        DeepSeekResponse response = ParseResponse(request.downloadHandler.text);

        if (response != null && response.choices.Length > 0)
        {
            string npcReply = response.choices[0].message.content;
            AddMessageDic("assistant", npcReply);
            //成功接受消息就保存到数据库
            NetMessageManager.Instance.SendMessageToClient(NetMessageType.S_C_AIAnswer,
                new S_C_AIAnswer() { message = npcReply },
                clientID);
            DatabaseManager.Instance.SaveCrafterData(new CrafterData()
                { playerName = playerName, aiMessagesDic = messagesDic, currentAiDailogIndex = currentMessageId });
        }
        else
        {
            NetMessageManager.Instance.SendMessageToClient(NetMessageType.S_C_AIAnswer,
                new S_C_AIAnswer() { message = name + "（陷入沉默）" },
                clientID);
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
        UnityWebRequest request = new UnityWebRequest(ServerResSystem.serverConfig.aIKeyConfig.apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw); // 设置上传处理器
        request.downloadHandler = new DownloadHandlerBuffer(); // 设置下载处理器
        request.SetRequestHeader("Content-Type", "application/json"); // 设置请求头
        request.SetRequestHeader("Authorization", $"Bearer {ServerResSystem.serverConfig.aIKeyConfig.apiKey}"); // 设置认证头
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