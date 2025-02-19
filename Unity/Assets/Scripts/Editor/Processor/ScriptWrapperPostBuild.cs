#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class ScriptWrapperPostBuild : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        bool isServerBuild = BuildProcessorConfig.IsServerBuildTarget();
        BuildProcessorHelper.ProcessSceneComponents(isServerBuild, (component, mode) =>
        {
            if (mode == ComponentMode.Delete)
                BuildProcessorHelper.HandleScriptWrapping(component.GetType(), false);
        });
    }
}
#endif