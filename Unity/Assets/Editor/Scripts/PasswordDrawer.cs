using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PasswordAttribute))]
public class PasswordDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            // 用 PasswordField 绘制字符串字段
            property.stringValue = EditorGUI.PasswordField(position, label, property.stringValue);
        }
        else
        {
            EditorGUI.HelpBox(position, "[Password] 仅适用于 string 类型", MessageType.Error);
        }
    }
}
