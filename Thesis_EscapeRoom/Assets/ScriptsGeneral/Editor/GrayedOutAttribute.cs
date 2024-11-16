using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace CoreSystems.Extensions.Attributes
{
	public class GrayedOutAttribute : PropertyAttribute
	{
        public string boolPropertyName;
        public bool invert;
        public bool hide;
        public GrayedOutAttribute(string boolPropertyName, bool invert = false, bool hide = false)
        {
            this.boolPropertyName = boolPropertyName;
            this.invert = invert;
            this.hide = hide;
        }
    }

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(GrayedOutAttribute))]
	public class GrayedOutDrawer : PropertyDrawer
	{
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GrayedOutAttribute grayedOutAttribute = attribute as GrayedOutAttribute;
            SerializedProperty boolProperty = property.serializedObject.FindProperty(grayedOutAttribute.boolPropertyName);

            if (boolProperty != null && boolProperty.propertyType == SerializedPropertyType.Boolean)
            {
                bool isEnabled = boolProperty.boolValue;

                if (grayedOutAttribute.invert)
                {
                    isEnabled = !isEnabled;
                }

                if (!isEnabled)
                {
                    GUI.enabled = false;
                    if (!grayedOutAttribute.hide)
                    {
                        EditorGUI.PropertyField(position, property, label, true);
                    }
                    GUI.enabled = true;
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
                Debug.LogWarning("GrayedOut attribute: Invalid boolean property name.");
            }
        }
    }
#endif
}