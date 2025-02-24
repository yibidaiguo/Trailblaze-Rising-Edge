using UnityEngine;
[CreateAssetMenu(menuName = "Config/AI/AIConfig")]
public class AIKeyConfig : ScriptableObject
{
    [Password]
    public string apiKey;
    public string modelName;
    public string apiUrl;
}

public class PasswordAttribute : PropertyAttribute
{
    
}


