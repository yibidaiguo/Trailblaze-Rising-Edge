#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
public class MessageHandlerGenerator
{
    private const string GeneratedCodeFolder =  "/Scripts/Common/NetCodeGenerated/";

    [MenuItem("Project/生成网络消息代码")]
    public static void Generate()
    {
        // 生成 MessageType 枚举
        GenerateMessageType();

        // 重新加载程序集以获取新生成的枚举类型
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Type netMessageType = null;
        foreach (Assembly assembly in assemblies)
        {
            netMessageType = assembly.GetType("NetMessageType");
            if (netMessageType != null)
            {
                break;
            }
        }

        if (netMessageType == null)
        {
            Debug.LogError("未能找到 NetMessageType 枚举类型。");
            return;
        }

        // 生成消息处理代码
        GenerateMessageHandlers(netMessageType);
    }

    private static void GenerateMessageHandlers(Type messageTypeEnum)
    {
        StringBuilder codeBuilder = new StringBuilder();

        // 添加命名空间和引用
        codeBuilder.AppendLine("using Unity.Netcode;");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine("public partial class NetMessageManager");
        codeBuilder.AppendLine("{");
        codeBuilder.AppendLine("    //这个函数会在Init的时候调用");
        codeBuilder.AppendLine("    partial void OnInit()");
        codeBuilder.AppendLine("    {");
        codeBuilder.AppendLine("         messagingManager.OnUnnamedMessage += ReceiveMessage;");
        codeBuilder.AppendLine("    }");
        codeBuilder.AppendLine("    partial void ReceiveMessage(ulong clientId, FastBufferReader reader)");
        codeBuilder.AppendLine("    {");
        codeBuilder.AppendLine($"        reader.ReadValueSafe(out {messageTypeEnum.FullName} messageType);");
        codeBuilder.AppendLine("        switch (messageType)");
        codeBuilder.AppendLine("        {");

        var messageStructCache = GetAllMessageStructs();
        foreach (var value in Enum.GetValues(messageTypeEnum))
        {
            string enumName = Enum.GetName(messageTypeEnum, value);
            if (enumName == "None") continue;

            // 从缓存中查找结构体类型
            Type structType = messageStructCache.FirstOrDefault(t => t.Name == enumName);
            if (structType != null)
            {
                codeBuilder.AppendLine($"            case {messageTypeEnum.FullName}.{enumName}:");
                codeBuilder.AppendLine($"                reader.ReadValueSafe(out {structType.FullName} {enumName.ToLower()});");
                codeBuilder.AppendLine($"                TriggerMessageCallback({messageTypeEnum.FullName}.{enumName}, clientId, {enumName.ToLower()});");
                codeBuilder.AppendLine("                break;");
            }
        }

        codeBuilder.AppendLine("        }");
        codeBuilder.AppendLine("    }");
        codeBuilder.AppendLine("}");

        SaveGeneratedCode(codeBuilder.ToString(), "GeneratedMessageHandlers.cs", "Message handlers 生成成功!", "生成 MessageHandlers 失败: ");
    }

    private static void GenerateMessageType()
    {
        // 扫描当前程序集中标记了 NetCodeMessageType 的结构体
        var messageStructs = GetAllMessageStructs();

        // 生成 NetMessageType 枚举代码
        var messageTypeCode = GenerateMessageTypeEnum(messageStructs);
        Debug.Log("生成 NetMessageType 枚举代码:");
        Debug.Log(messageTypeCode);

        SaveGeneratedCode(messageTypeCode, "GeneratedNetMessageType.cs", "MessageType enum 生成成功!", "生成 NetMessageType 枚举失败: ");
    }

    private static string GenerateMessageTypeEnum(List<Type> messageStructs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("public enum NetMessageType : byte");
        sb.AppendLine("{");
        sb.AppendLine("    None,");
        
        
        if (!GetINetworkSerializableType(out Type networkSerializable))
        {
            Debug.LogError($"无法获取 INetworkSerializable 类型。");
            return null;
        }

        foreach (var structType in messageStructs)
        {
            // 确保结构体继承自 INetworkSerializable
            if (!networkSerializable.IsAssignableFrom(structType))
            {
                Debug.LogError($"该结构体没有继承自 '{structType.Name}' ，不是合法的消息结构体");
                continue;
            }
            sb.AppendLine($"    {structType.Name},");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static List<Type> GetAllMessageStructs()
    {
        var messageStructs = new List<Type>();
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 过滤掉系统程序集，只保留项目相关的程序集
        var relevantAssemblies = allAssemblies.Where(assembly =>
            !assembly.FullName.StartsWith("System.") &&
            !assembly.FullName.StartsWith("Microsoft.") &&
            !assembly.FullName.StartsWith("Unity.")
        ).ToList();

        foreach (var assembly in relevantAssemblies)
        {
            try
            {
                var types = assembly.GetTypes();

                // 查找标记了 NetCodeMessageType 特性的结构体
                var structsWithAttribute = types.Where(t =>
                    t.IsValueType &&
                    t.GetCustomAttributes(typeof(NetCodeMessageType), false).Length > 0
                );

                messageStructs.AddRange(structsWithAttribute);
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Debug.LogError($"从程序集中获取类型失败 {assembly.FullName}: {loaderException.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"处理程序集 {assembly.FullName} 时发生错误: {ex.Message}");
            }
        }

        return messageStructs;
    }

    private static void SaveGeneratedCode(string code, string fileName, string successMessage, string errorPrefix)
    {
        try
        {
            // 保存生成的文件
            string path = Application.dataPath +GeneratedCodeFolder + fileName;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, code);
            AssetDatabase.Refresh();

            Debug.Log(successMessage);
        }
        catch (Exception e)
        {
            Debug.LogError($"{errorPrefix} {e.Message}");
        }
    }

    private static bool GetINetworkSerializableType(out Type netMessageType)
    {
        // 遍历所有已加载的程序集查找 INetworkSerializable 类型
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            netMessageType = assembly.GetType("Unity.Netcode.INetworkSerializable");
            if (netMessageType != null)
            {
                return true;
            }
        }

        Debug.LogError("未找到 INetworkSerializable 类型，请确保已正确安装 Netcode For Gameobject 包。");
        netMessageType = null;
        return false;
    }
}
#endif