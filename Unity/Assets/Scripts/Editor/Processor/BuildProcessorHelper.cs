#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class BuildProcessorHelper
{
    public static void ProcessSceneAndPrefabComponents(bool isServerBuild, Action<MonoBehaviour, ComponentMode> componentAction = null)
    {
        // 处理场景组件
        ProcessSceneComponents(isServerBuild, componentAction);
        // 处理预制体组件
        ProcessPrefabComponents(isServerBuild, componentAction);
    }
    
    public static void ProcessSceneComponents(bool isServerBuild, Action<MonoBehaviour, ComponentMode> componentAction = null)
    {
        for (int i =0; i < SceneManager.sceneCount; i++)
        {
            foreach (var rootObj in SceneManager.GetSceneAt(i).GetRootGameObjects())
            {
                ProcessRootObject(rootObj, isServerBuild, componentAction);
            }
        }
    }

    private static void ProcessRootObject(GameObject rootObj, bool isServerBuild, Action<MonoBehaviour, ComponentMode> componentAction)
    {
        var attributeType = isServerBuild ? typeof(OnServerBuildAttribute) : typeof(OnClientBuildAttribute);
        ProcessComponents(rootObj, attributeType, componentAction);

        foreach (Transform child in rootObj.transform)
        {
            ProcessRootObject(child.gameObject, isServerBuild, componentAction);
        }
    }

    private static void ProcessComponents(GameObject obj, Type attributeType, Action<MonoBehaviour, ComponentMode> componentAction)
    {
        var components = obj.GetComponents<MonoBehaviour>();
        for (int i = components.Length - 1; i >= 0; i--)
        {
            var component = components[i];
            if (component == null) continue;

            try
            {
                var attribute = component.GetType().GetCustomAttribute(attributeType, true) as BuildTypeAttribute;
                if (attribute == null) continue;

                componentAction?.Invoke(component, attribute.Mode);
            }
            catch (Exception ex)
            {
                Debug.LogError($"处理组件 {component.GetType().Name} 时出错: {ex.Message}");
            }
        }
    }
    private static void ProcessPrefabComponents(bool isServerBuild, Action<MonoBehaviour, ComponentMode> componentAction)
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                ProcessRootObject(prefab, isServerBuild, componentAction);
            }
        }
    }

    public static void HandleScriptWrapping(Type scriptType, bool wrap)
    {
        string scriptName = scriptType.Name + ".cs";
        string[] scriptFiles = Directory.GetFiles(Application.dataPath, scriptName, SearchOption.AllDirectories);
        
        foreach (string scriptFile in scriptFiles)
        {
            try
            {
                string content = File.ReadAllText(scriptFile);
                bool isWrapped = content.StartsWith(BuildProcessorConfig.wrapperStart) && 
                                 content.EndsWith(BuildProcessorConfig.wrapperEnd);

                if (wrap && !isWrapped)
                {
                    File.WriteAllText(scriptFile, $"{BuildProcessorConfig.wrapperStart}\n{content}\n{BuildProcessorConfig.wrapperEnd}");
                    Debug.Log($"Wrapped script: {scriptFile}");
                }
                else if (!wrap && isWrapped)
                {
                    string unwrappedContent = content.Substring(
                        BuildProcessorConfig.wrapperStart.Length + 1,
                        content.Length - BuildProcessorConfig.wrapperStart.Length - BuildProcessorConfig.wrapperEnd.Length - 2
                    );
                    File.WriteAllText(scriptFile, unwrappedContent);
                    Debug.Log($"Unwrapped script: {scriptFile}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"处理脚本文件 {scriptFile} 时出错: {ex.Message}");
            }
        }
    }

    public static void ApplyComponentMode(MonoBehaviour component, ComponentMode mode)
    {
        switch (mode)
        {
            case ComponentMode.Delete:
                if (Application.isEditor)
                    Object.DestroyImmediate(component, true); // 对于预制体，需要传递 true 以确保在预制体实例上正确删除
                else
                    Object.Destroy(component);
                break;
            case ComponentMode.Hide:
                component.enabled = false;
                break;
        }
    }
}
#endif
