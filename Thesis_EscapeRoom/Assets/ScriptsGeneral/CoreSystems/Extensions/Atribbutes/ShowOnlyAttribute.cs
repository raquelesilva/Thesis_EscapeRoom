using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreSystems.Extensions.Attributes
{
	public class ShowOnlyAttribute : PropertyAttribute
	{
		// ### Esta classe simplesmente cria um atributo novo, neste caso para vari�veis serializadas no editor do Unity,
		// e que por design n�o devem de ser alteradas no editor.
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
	public class ReadOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property,
												GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position,
								   SerializedProperty property,
								   GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
#endif
}