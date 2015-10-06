using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityToolbag
{
	[CustomPropertyDrawer(typeof(AnimParamsConfig))]
	public class AnimParamsCfgInspector : PropertyDrawer
	{
		void OnEnable()
		{
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Rect rectItem = new Rect(position);
			rectItem.height = EditorGUIUtility.singleLineHeight;

			property.isExpanded = EditorGUI.Foldout(rectItem, property.isExpanded, label);

			if (!property.isExpanded)
				return;

			EditorGUI.indentLevel++;

			rectItem.yMin = rectItem.yMax;
			rectItem.yMax = rectItem.yMin + EditorGUIUtility.singleLineHeight;
			SerializedProperty pathProp = property.FindPropertyRelative("instanceId");

			AnimatorController prevCtrl = null;
			int ctrlId;
			if (int.TryParse(pathProp.stringValue, out ctrlId))
			{
				prevCtrl = EditorUtility.InstanceIDToObject(ctrlId) as AnimatorController;
			}

			AnimatorController newCtrl = EditorGUI.ObjectField(rectItem,
										new GUIContent("Animator Controller"), prevCtrl,
										typeof(AnimatorController), false) as AnimatorController;

			if (prevCtrl != newCtrl)
			{
				string newId = (newCtrl != null) ? newCtrl.GetInstanceID().ToString() : "";
				pathProp.stringValue = newId;
				property.serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.indentLevel--;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label);
		}
	}
}