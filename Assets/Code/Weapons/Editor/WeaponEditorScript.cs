using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Weapon))]
public class WeaponEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();
        bool canIterate = property.NextVisible(true); // Start with the first visible property.

        // Store the name of the toggle property to avoid magic strings
        string togglePropertyName = "m_hitscan";
        bool showConditionalProperties = false;

        while (canIterate)
        {
            // If this is the toggle property, we need to draw it and record its value
            if (property.name == togglePropertyName)
            {
                EditorGUILayout.PropertyField(property, true);
                showConditionalProperties = property.boolValue;
            }
            // If the toggle is true, draw all properties
            else if (showConditionalProperties)
            {
                EditorGUILayout.PropertyField(property, true);
            }
            // If the toggle is false, only draw properties that are not meant to be conditional
            else if (property.name != "m_hitForce" &&
                     property.name != "m_damage" &&
                     property.name != "m_effectiveRange")
            {
                EditorGUILayout.PropertyField(property, true);
            }

            canIterate = property.NextVisible(false); // Move to the next visible property.
        }

        serializedObject.ApplyModifiedProperties();
    }
}
