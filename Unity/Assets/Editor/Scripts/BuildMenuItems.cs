using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
public static class BuildMenuItems
{
    public const string rootFolderPath = "Builds";
    public const string serverFolderPath = "Server";
    public const string clientFolderPath = "Client";

    [MenuItem("Project/Build/Server")]
    public static void Server()
    {
        Debug.Log("开始构建服务端");
        BuildFilterAssemblies.serverMode = true;
        // 关闭HybridCLR
        SettingsUtil.Enable = false;
        // 关闭Addressables
        AddressableAssetSettingsDefaultObject.Settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer;

        List<string> sceneList = new List<string>(EditorSceneManager.sceneCountInBuildSettings);
        for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (scenePath != null && !scenePath.Contains("Client"))
            {
                Debug.Log("添加场景:" + scenePath);
                sceneList.Add(scenePath);
            }
        }
        string projectRootPath = new DirectoryInfo(Application.dataPath).Parent.FullName;
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
        {
            scenes = sceneList.ToArray(),
            target = BuildTarget.StandaloneWindows64,
            subtarget = (int)StandaloneBuildSubtarget.Server,
            locationPathName = $"{projectRootPath}/{rootFolderPath}/{serverFolderPath}/Server.exe"
        };
        string hideFolder = "Assets/Scripts/HotUpdate";
        HideFolder(hideFolder);
        try
        {
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log("完成构建服务端");
        }
        catch (System.Exception)
        {
            throw;
        }
        finally
        {
            RecoverFolder(hideFolder);
            // 还原HybridCLR
            SettingsUtil.Enable = true;
            // 还原Addressables
            AddressableAssetSettingsDefaultObject.Settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.PreferencesValue;
        }
    }
    [MenuItem("Project/Build/NewClient")]
    public static void NewClient()
    {
        Debug.Log("开始构建客户端");
        BuildFilterAssemblies.serverMode = false;
        string hideFolder = "Assets/Scripts/Server";
        HideFolder(hideFolder);
        try
        {
            // HybridCLR构建准备
            PrebuildCommand.GenerateAll();
            // 搬运dll文本文件
            GenerateDllBytesFile();
            // 清理可能存在的Addressables缓存
            string catalogPath = $"{Application.persistentDataPath}/com.unity.addressables";
            if (Directory.Exists(catalogPath)) Directory.Delete(catalogPath, true);
            List<string> sceneList = new List<string>(EditorSceneManager.sceneCountInBuildSettings);
            for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (scenePath != null && !scenePath.Contains("Server"))
                {
                    Debug.Log("添加场景:" + scenePath);
                    sceneList.Add(scenePath);
                }
            }

            string projectRootPath = new DirectoryInfo(Application.dataPath).Parent.FullName;
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = sceneList.ToArray(),
                target = BuildTarget.StandaloneWindows64,
                subtarget = (int)StandaloneBuildSubtarget.Player,
                locationPathName = $"{projectRootPath}/{rootFolderPath}/{clientFolderPath}/Client.exe"
            };
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            // Addressables会自动构建
            Debug.Log("完成构建客户端");
        }
        catch (System.Exception)
        {
            throw;
        }
        finally
        {
            RecoverFolder(hideFolder);
        }
    }


    [MenuItem("Project/Build/UpdateClient")]
    public static void UpdateClient()
    {
        Debug.Log("开始构建客户端更新包");
        BuildFilterAssemblies.serverMode = false;
        string hideFolder = "Assets/Scripts/Server";
        HideFolder(hideFolder);
        try
        {
            CompileDllCommand.CompileDllActiveBuildTarget();
            GenerateDllBytesFile();
            string path = ContentUpdateScript.GetContentStateDataPath(false);
            AddressableAssetSettings addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
            ContentUpdateScript.BuildContentUpdate(addressableAssetSettings, path);
            Debug.Log("完成构建客户端更新包");
        }
        catch (System.Exception)
        {
            throw;
        }
        finally
        {
            RecoverFolder(hideFolder);
        }

    }

    [MenuItem("Project/Build/GenerateDllBytesFile")]
    public static void GenerateDllBytesFile()
    {
        Debug.Log("开始生成dll文本文件");
        string aotUpdateDllDirPath = System.Environment.CurrentDirectory + "\\" + SettingsUtil.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget).Replace('/', '\\');
        string hotUpdateDllDirPath = System.Environment.CurrentDirectory + "\\" + SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget).Replace('/', '\\');
        string aotDllTextDirPath = System.Environment.CurrentDirectory + "\\Assets\\Scripts\\DllBytes\\Aot";
        string hotUpdateDllTextDirPath = System.Environment.CurrentDirectory + "\\Assets\\Scripts\\DllBytes\\HotUpdate";

        foreach (var dllName in SettingsUtil.AOTAssemblyNames)
        {
            string path = $"{aotUpdateDllDirPath}\\{dllName}.dll";
            if (File.Exists(path))
            {
                File.Copy(path, $"{aotDllTextDirPath}\\{dllName}.dll.bytes", true);
            }
            else
            {
                path = $"{hotUpdateDllDirPath}\\{dllName}.dll";
                File.Copy(path, $"{aotDllTextDirPath}\\{dllName}.dll.bytes", true);
            }
        }
        foreach (var dllName in SettingsUtil.HotUpdateAssemblyNamesExcludePreserved)
        {
            File.Copy($"{hotUpdateDllDirPath}\\{dllName}.dll", $"{hotUpdateDllTextDirPath}\\{dllName}.dll.bytes", true);
        }
        AssetDatabase.Refresh();
        Debug.Log("完成生成dll文本文件");
    }

    private static void HideFolder(string folder)
    {
        if (Directory.Exists(folder))
        {
            string newPath = $"{folder}~";
            Directory.Move(folder, newPath);
            File.Delete($"{folder}.meta");
            Debug.Log($"隐藏了{folder}文件夹");
            AssetDatabase.Refresh();
        }
    }
    private static void RecoverFolder(string folder)
    {
        string newPath = $"{folder}~";
        if (Directory.Exists(newPath))
        {
            Directory.Move(newPath, folder);
            Debug.Log($"恢复了{folder}文件夹");
            AssetDatabase.Refresh();
        }
    }
}
