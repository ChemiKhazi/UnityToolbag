using UnityEngine;
using UnityEditor;

using System;
using System.Reflection;
using System.Collections;

[CustomPropertyDrawer(typeof(SortingLayer))]
public class SortingLayerDrawer : PropertyDrawer {
	
	private static string[] _sortingLayerNames;

	void SetupSortingLayerNames()
	{
		if (_sortingLayerNames == null)
		{
			var internalEditorUtilityType = Type.GetType("UnityEditorInternal.InternalEditorUtility, UnityEditor");
			var sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			_sortingLayerNames = sortingLayersProperty.GetValue(null, new object[0]) as string[];
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		SetupSortingLayerNames();
		if (_sortingLayerNames != null)
		{
			EditorGUI.BeginProperty(position, label, property);
			SerializedProperty layerIndexProp = property.FindPropertyRelative("sortLayer");
			SerializedProperty layerNameProp = property.FindPropertyRelative("layerName");

			int layerIndex = Array.IndexOf(_sortingLayerNames, layerNameProp.stringValue);
			if (layerIndex < 0)
				layerIndex = layerIndexProp.intValue;
			if (layerIndex < 0 || layerIndex >= _sortingLayerNames.Length)
				layerIndex = 0;

			int newLayerIndex = EditorGUI.Popup(position, label.text, layerIndex, _sortingLayerNames);
			if (newLayerIndex != layerIndex)
			{
				layerIndexProp.intValue = newLayerIndex;
				layerNameProp.stringValue = _sortingLayerNames[newLayerIndex];
			}
			EditorGUI.EndProperty();
		}
		else
		{
			base.OnGUI(position, property, label);
		}
	}
}
