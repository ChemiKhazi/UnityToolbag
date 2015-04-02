using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityToolbag
{
    [CustomPropertyDrawer(typeof(LayerProperty))]
    public class LayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer) {
				EditorGUI.HelpBox(position, string.Format("{0} is not an integer but has [LayerProperty].", property.name), MessageType.Error);
            }
            else
            {
	            List<string> layerNames = new List<string>();
	            List<int> layerIndices = new List<int>();
	            for (int i = 0; i < 32; i++)
	            {
		            string layer = UnityEditorInternal.InternalEditorUtility.GetLayerName(i);
		            if (!string.IsNullOrEmpty(layer))
		            {
						layerNames.Add(layer);
			            layerIndices.Add(i);
		            }
	            }
	            int currentSelection = layerIndices.IndexOf(property.intValue);
	            if (currentSelection < 0)
		            currentSelection = 0;

	            EditorGUI.BeginProperty(position, label, property);
	            int newSelection = EditorGUI.Popup(position, label.text, currentSelection, layerNames.ToArray());
				if (newSelection != currentSelection)
				{
					property.intValue = layerIndices[newSelection];
				}
                EditorGUI.EndProperty();
            }
        }
    }
}
