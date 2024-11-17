using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DynamicReferenceBase), true)]
public class DynamicReferencePropertyDrawer : PropertyDrawer
{
    private string[] _popupOptions = new string[] { "UseReference", "UseValue" };
    private int _popupIndex = 0;

    private const float LabelWidth = 0.35f;
    private const float PopupWidth = 0.05f;
    private const float PropertyWidth = 0.6f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var refValueField = property.FindPropertyRelative("_refValue");
        var varValueField = property.FindPropertyRelative("_varValue");

        EditorGUI.LabelField(new Rect(position.x, position.y, position.width * LabelWidth, position.height), new GUIContent(property.displayName));

        _popupIndex = EditorGUI.Popup(new Rect(position.x + position.width * LabelWidth, position.y, position.width * PopupWidth, position.height), _popupIndex, _popupOptions);

        if (_popupIndex == 0)
            EditorGUI.PropertyField(new Rect(position.x + position.width * (LabelWidth + PopupWidth), position.y, position.width * PropertyWidth, position.height), refValueField, GUIContent.none);
        else
            EditorGUI.PropertyField(new Rect(position.x + position.width * (LabelWidth + PopupWidth), position.y, position.width * PropertyWidth, position.height), varValueField, GUIContent.none);

        EditorGUI.EndProperty();

        property.FindPropertyRelative("_useReference").SetUnderlyingValue(_popupIndex == 0 ? true : false);
    }
}
