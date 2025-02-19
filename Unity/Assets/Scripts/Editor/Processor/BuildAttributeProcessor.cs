#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

public class BuildAttributeProcessor : IProcessSceneWithReport
{
    public int callbackOrder => int.MaxValue;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        bool isServerBuild = BuildProcessorConfig.IsServerBuildTarget();
        BuildProcessorHelper.ProcessSceneAndPrefabComponents(isServerBuild, (component, mode) =>
        {
            BuildProcessorHelper.ApplyComponentMode(component, mode);
        });
    }
}

#endif