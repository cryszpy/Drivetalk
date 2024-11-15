using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DynamicReference<>), true)]
public class DynamicReferencePropertyDrawer : PropertyDrawer
{
    private string[] _popupOptions = new string[] { "UseReference", "UseValue" };
    private int _popupIndex = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.BeginHorizontal();

        var refValueField = property.FindPropertyRelative("_refValue");
        var varValueField = property.FindPropertyRelative("_varValue");

        GUILayout.Label(new GUIContent(property.displayName));

        _popupIndex = EditorGUILayout.Popup(_popupIndex, _popupOptions, GUILayout.Width(20));
        property.FindPropertyRelative("_useReference").SetUnderlyingValue(_popupIndex == 0 ? true : false);

        if (_popupIndex == 0)
            EditorGUILayout.PropertyField(refValueField, GUIContent.none);
        else
            EditorGUILayout.PropertyField(varValueField, GUIContent.none);

        EditorGUILayout.EndHorizontal();
    }
}
