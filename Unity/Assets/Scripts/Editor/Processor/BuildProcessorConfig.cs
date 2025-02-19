#if UNITY_EDITOR
public static class BuildProcessorConfig
{
    public const  string wrapperStart = "#if UNITY_EDITOR";
    public const string wrapperEnd = "#endif";
    
    
    public static bool IsServerBuildTarget()
    {
#if UNITY_SERVER
        return true;
#else
        return false;
#endif
    }
}
#endif