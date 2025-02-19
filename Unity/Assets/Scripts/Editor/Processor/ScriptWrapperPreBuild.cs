#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class ScriptWrapperPreBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        bool isServerBuild = BuildProcessorConfig.IsServerBuildTarget();
        BuildProcessorHelper.ProcessSceneComponents(isServerBuild, (component, mode) =>
        {
            if (mode == ComponentMode.Delete)
                BuildProcessorHelper.HandleScriptWrapping(component.GetType(), true);
        });
    }
}
#endif