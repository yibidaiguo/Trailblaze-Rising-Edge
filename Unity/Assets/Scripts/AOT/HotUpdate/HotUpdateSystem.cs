using HybridCLR;
using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HotUpdateSystem : MonoBehaviour
{
    [Serializable]
    private class HotUpdateSystemState
    {
        public bool hotUpdateSucceed;
    }

    [SerializeField] private TextAsset[] aotDllAssets;
    [SerializeField] private string[] hotUpdateDllNames;
    [SerializeField] private string versionInfoAddressableKey;
    private Action<float> onPercentageForEachFile;
    private Action<bool> onEnd;

    public void StartHotUpdate(Action<float> onPercentageForEachFile, Action<bool> onEnd)
    {
        this.onPercentageForEachFile = onPercentageForEachFile;
        this.onEnd = onEnd;

#if UNITY_EDITOR
        onEnd?.Invoke(true);
        return;
#endif
        StartCoroutine(DoUpdateAddressables());
    }

    private bool succeed;
    private IEnumerator DoUpdateAddressables()
    {
        // 确定上一次热更新的状态
        HotUpdateSystemState state = SaveSystem.LoadSetting<HotUpdateSystemState>();
        if (state == null || !state.hotUpdateSucceed)
        {
            Debug.Log("断点续传");
            string catalogPath = $"{Application.persistentDataPath}/com.unity.addressables";
            if (Directory.Exists(catalogPath)) Directory.Delete(catalogPath, true);
        }

        // 初始化
        yield return Addressables.InitializeAsync();
        succeed = true;
        // 检测目录更新
        AsyncOperationHandle<List<string>> checkForCatalogUpdatesHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkForCatalogUpdatesHandle;
        if (checkForCatalogUpdatesHandle.Status != AsyncOperationStatus.Succeeded)
        {
            succeed = false;
            JKLog.Error($"CheckForCatalogUpdates失败:{checkForCatalogUpdatesHandle.OperationException}");
            Addressables.Release(checkForCatalogUpdatesHandle);
        }
        else
        {
            List<string> catalogResult = checkForCatalogUpdatesHandle.Result;
            Addressables.Release(checkForCatalogUpdatesHandle);

            // 下载最新的目录
            if (catalogResult.Count > 0)
            {
                ShowLoadingWindow();
                AsyncOperationHandle<List<IResourceLocator>> updateCatalogsHandle = Addressables.UpdateCatalogs(catalogResult, false);
                yield return updateCatalogsHandle;
                if (updateCatalogsHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    succeed = false;
                    Addressables.Release(updateCatalogsHandle);
                    JKLog.Error($"UpdateCatalogs失败:{updateCatalogsHandle.OperationException.Message}");
                }
                else
                {
                    List<IResourceLocator> locatorList = updateCatalogsHandle.Result;
                    Addressables.Release(updateCatalogsHandle);
                    JKLog.Log("下载目录更新成功");
                    List<object> downloadKeys = new List<object>(1000);
                    foreach (IResourceLocator locator in locatorList)
                    {
                        downloadKeys.AddRange(locator.Keys);
                    }
                    SetLoadingWindow();
                    yield return DownloadAllAssets(downloadKeys);
                    CloseLoadingWindow();
                }
            }
            else JKLog.Log("无需更新");
        }

        if (state == null) state = new HotUpdateSystemState();
        state.hotUpdateSucceed = succeed;
        SaveSystem.SaveSetting(state);
        if (succeed)
        {
            LoadHotUpdateDll();
            LoadMetaForAOTAssemblies();
            // 因为Addressables在初始化目录后才加载dll，这会导致AD会认为类型为热更程序集中的类型是未知的资源 认为是System.Object
            Addressables.LoadContentCatalogAsync($"{Addressables.RuntimePath}/catalog.json");
        }
        onEnd?.Invoke(succeed);
    }


    private IEnumerator DownloadAllAssets(List<object> keys)
    {
        AsyncOperationHandle<long> sizeHandle = Addressables.GetDownloadSizeAsync((IEnumerable<object>)keys);
        yield return sizeHandle;
        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            succeed = false;
            JKLog.Error($"GetDownloadSizeAsync失败:{sizeHandle.OperationException.Message}");
        }
        else
        {
            long downloadSize = sizeHandle.Result;
            if (downloadSize > 0)
            {
                // 实际的下载
                AsyncOperationHandle downloadDependenciesHandle = Addressables.DownloadDependenciesAsync((IEnumerable<object>)keys, Addressables.MergeMode.Union, false);
                // 循环查看下载进度
                while (!downloadDependenciesHandle.IsDone)
                {
                    if (downloadDependenciesHandle.Status == AsyncOperationStatus.Failed)
                    {
                        succeed = false;
                        JKLog.Error($"downloadDependenciesHandle失败:{downloadDependenciesHandle.OperationException.Message}");
                        break;
                    }
                    // 分发下载进度
                    float percentage = downloadDependenciesHandle.GetDownloadStatus().Percent;
                    onPercentageForEachFile?.Invoke(percentage);
                    UpdateLoadingWindowProgress(downloadSize * percentage, downloadSize);

                    yield return CoroutineTool.WaitForFrame();
                }
                if (downloadDependenciesHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    JKLog.Log($"全部下载完成");
                }
                Addressables.Release(downloadDependenciesHandle);
            }
        }
        Addressables.Release(sizeHandle);
    }

    private string GetVersionInfo(LanguageType languageType)
    {
        Addressables.DownloadDependenciesAsync(versionInfoAddressableKey, true).WaitForCompletion();
        VersionInfo versionInfo = Addressables.LoadAssetAsync<VersionInfo>(versionInfoAddressableKey).WaitForCompletion();
        string info = versionInfo.GetVersionData(languageType).info;
        Addressables.Release(versionInfo);
        return info;
    }
    private void LoadHotUpdateDll()
    {
        for (int i = 0; i < hotUpdateDllNames.Length; i++)
        {
            TextAsset dllTextAsset = Addressables.LoadAssetAsync<TextAsset>(hotUpdateDllNames[i]).WaitForCompletion();
            System.Reflection.Assembly.Load(dllTextAsset.bytes);
            JKLog.Log($"加载{hotUpdateDllNames[i]}程序集");
        }
    }

    private void LoadMetaForAOTAssemblies()
    {
        for (int i = 0; i < aotDllAssets.Length; i++)
        {
            byte[] dllBytes = aotDllAssets[i].bytes;
            LoadImageErrorCode errorCode = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
            JKLog.Log($"LoadMetaForAOTAssemblies：{aotDllAssets[i].name}，{errorCode}");
        }
    }

    private UI_LoadingWindow loadingWindow;
    private void ShowLoadingWindow()
    {
        loadingWindow = UISystem.Show<UI_LoadingWindow>();
        loadingWindow.Set("Loading....");
    }

    private void SetLoadingWindow()
    {
        // 根据当前设置确定语言类型
        LanguageType languageType;
        GameBasicSetting basicSetting = SaveSystem.LoadSetting<GameBasicSetting>();
        if (basicSetting == null) languageType = Application.systemLanguage == SystemLanguage.ChineseSimplified ? LanguageType.SimplifiedChinese : LanguageType.English;
        else languageType = basicSetting.languageType;
        loadingWindow.Set(GetVersionInfo(languageType));
    }

    private void CloseLoadingWindow()
    {
        UISystem.Close<UI_LoadingWindow>();
        loadingWindow = null;
    }

    private void UpdateLoadingWindowProgress(float current, float max)
    {
        loadingWindow.UpdateDownloadProgress(current, max);
    }
}
