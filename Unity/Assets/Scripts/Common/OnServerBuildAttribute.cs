using System;

[AttributeUsage(AttributeTargets.Class)]
public class BuildTypeAttribute : Attribute
{
    public ComponentMode Mode { get; private set; }

    public BuildTypeAttribute(ComponentMode mode)
    {
        Mode = mode;
    }
}

public class OnServerBuildAttribute : BuildTypeAttribute
{
    public OnServerBuildAttribute(ComponentMode mode) : base(mode)
    {
    }
}

public class OnClientBuildAttribute : BuildTypeAttribute
{
    public OnClientBuildAttribute(ComponentMode mode) : base(mode)
    {
    }
}

public enum ComponentMode
{
    None,
    Delete,
    Hide
}