using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AssemblyChecker
{
    [MenuItem("Tools/Check Netcode Assembly")]
    public static void CheckNetcodeAssembly()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            Debug.Log($"Assembly Full Name: {assembly.FullName}");
        }
    }
}
