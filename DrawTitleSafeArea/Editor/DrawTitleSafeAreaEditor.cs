using UnityEngine;
using UnityEditor;

namespace UnityToolbag
{
    [CustomEditor(typeof(DrawTitleSafeArea))]
    public class DrawTitleSafeAreaEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_innerColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_outerColor"));

            var sizeMode = serializedObject.FindProperty("_sizeMode");
            EditorGUILayout.PropertyField(sizeMode);

            if (sizeMode.intValue == (int)TitleSafeSizeMode.Percentage) {
                var sizeX = serializedObject.FindProperty("_sizeX");
                sizeX.intValue = EditorGUILayout.IntSlider("Size X", sizeX.intValue, 0, 25);

                var sizeY = serializedObject.FindProperty("_sizeY");
                sizeY.intValue = EditorGUILayout.IntSlider("Size Y", sizeY.intValue, 0, 25);
            }
            else {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_sizeX"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_sizeY"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
