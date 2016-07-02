using UnityEngine;
using UnityEditor;

namespace UnityToolbag
{
	[CustomPropertyDrawer(typeof(ArrayIndexAttribute))]
	public class ArrayIndexAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}
			
			ArrayIndexAttribute attr = (ArrayIndexAttribute)attribute;
			SerializedProperty choices = null;

			string[] subPaths = attr.Path.Split('.');
			if (subPaths.Length > 0)
			{
				SerializedProperty currProp = property.serializedObject.FindProperty(subPaths[0]);
				for (int i = 1; i < subPaths.Length; i++)
				{
					if (currProp == null)
						break;

					SerializedProperty findProp = currProp.FindPropertyRelative(subPaths[i]);
					if (findProp == null && currProp.objectReferenceValue != null)
					{
						SerializedObject tempObj = new SerializedObject(currProp.objectReferenceValue);
						findProp = tempObj.FindProperty(subPaths[i]);
					}

					currProp = findProp;
				}
				choices = currProp;
			}
			
			if (choices == null || choices.isArray == false)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			GUIContent[] choiceValues = new GUIContent[choices.arraySize+1];
			choiceValues[choices.arraySize] = new GUIContent(attr.DefaultDisplay);
			for (int i = 0; i < choices.arraySize; i++)
			{
				SerializedProperty choiceProp = choices.GetArrayElementAtIndex(i);

				// Try to get the string value if straight up string
				string name = "";
				
				// TODO: add more methods for getting names
				switch (choiceProp.propertyType)
				{
					case SerializedPropertyType.String:
						name = choiceProp.stringValue;
						if (string.IsNullOrEmpty(name))
							name = choiceProp.displayName;
						break;
					case SerializedPropertyType.Float:
						name = choiceProp.floatValue.ToString();
						break;
					case SerializedPropertyType.Integer:
						name = choiceProp.intValue.ToString();
						break;
					case SerializedPropertyType.Enum:
						name = choiceProp.enumDisplayNames[choiceProp.enumValueIndex];
						break;
					case SerializedPropertyType.ObjectReference:
						name = choiceProp.objectReferenceValue.name;
						break;
					default:
						name = choiceProp.displayName;
						// Could use reflection from SerializedPropExtension found in
						// SortableArrayAttribute but expensive and maybe unecessary
						//object tempObj = choiceProp.GetValue<object>();
						//name = tempObj.ToString();
						break;
				}
				
				choiceValues[i] = new GUIContent(string.Format("{0} [{1}]", name, i));
			}

			EditorGUI.BeginProperty(position, label, property);
			int choice = property.intValue;

			// Current value is same as default index and not within array size, go to default none
			if (choice == attr.DefaultIndex && (attr.DefaultIndex < 0 || attr.DefaultIndex >= choices.arraySize))
				choice = choices.arraySize;

			int newValue = EditorGUI.Popup(position, label, choice, choiceValues);
			if (newValue != property.intValue)
			{
				// Selected the last value, set to default
				if (newValue == choices.arraySize)
					newValue = attr.DefaultIndex;
				property.intValue = newValue;
			}
			EditorGUI.EndProperty();
		}
	}
}
