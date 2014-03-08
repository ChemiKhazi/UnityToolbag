/*
 * Copyright (c) 2013, Nick Gravelyn.
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 *    1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 *
 *    2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 *
 *    3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace UnityToolbag {
    [CustomEditor(typeof(SortingLayerExposed))]
    public class SortingLayerExposedEditor : Editor {
        private string[] _sortingLayerNames;
		bool changeChildren = false;
		GameObject targetObject;
		int changeIndex = 0;

        void OnEnable()  {
            var internalEditorUtilityType = Type.GetType("UnityEditorInternal.InternalEditorUtility, UnityEditor");
            var sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            _sortingLayerNames = sortingLayersProperty.GetValue(null, new object[0]) as string[];
			targetObject = (target as SortingLayerExposed).gameObject;
        }

        public override void OnInspectorGUI() {
            // Get the renderer from the target object
            var renderer = targetObject.renderer;

            // If there is no renderer, we can't do anything
            if (!renderer) {
				DrawChangeChildrenOnly();
                return;
            }

            // If we have the sorting layers array, we can make a nice dropdown. For stability's sake, if the array is null
            // we just use our old logic. This makes sure the script works in some fashion even if Unity changes the name of
            // that internal field we reflected.
            if (_sortingLayerNames != null) {
                // Expose drop down for sorting layer
                int layerIndex = Array.IndexOf(_sortingLayerNames, renderer.sortingLayerName);
                int newLayerIndex = EditorGUILayout.Popup("Sorting Layer", layerIndex, _sortingLayerNames);
                if (newLayerIndex != layerIndex) {
					string newLayerName = _sortingLayerNames[newLayerIndex];

					if (!changeChildren)
					{
						Undo.RecordObject(renderer, "Edit Sorting Layer");
						renderer.sortingLayerName = newLayerName;
					}
					else
					{
						Renderer[] childRenderers = targetObject.GetComponentsInChildren<Renderer>();
						Undo.RecordObjects(childRenderers, "Edit Sorting Layer");
						foreach (Renderer child in childRenderers)
						{
							child.sortingLayerName = newLayerName;
						}
						renderer.sortingLayerName = newLayerName;
					}
                    EditorUtility.SetDirty(renderer);
                }
            }
            else {
                // Expose the sorting layer name
                string newSortingLayerName = EditorGUILayout.TextField("Sorting Layer Name", renderer.sortingLayerName);
                if (newSortingLayerName != renderer.sortingLayerName) {
                    Undo.RecordObject(renderer, "Edit Sorting Layer Name");
                    renderer.sortingLayerName = newSortingLayerName;
                    EditorUtility.SetDirty(renderer);
                }

                // Expose the sorting layer ID
                int newSortingLayerId = EditorGUILayout.IntField("Sorting Layer ID", renderer.sortingLayerID);
                if (newSortingLayerId != renderer.sortingLayerID) {
                    Undo.RecordObject(renderer, "Edit Sorting Layer ID");
                    renderer.sortingLayerID = newSortingLayerId;
                    EditorUtility.SetDirty(renderer);
                }
            }

            // Expose the manual sorting order
            int newSortingLayerOrder = EditorGUILayout.IntField("Sorting Layer Order", renderer.sortingOrder);
            if (newSortingLayerOrder != renderer.sortingOrder) {
                Undo.RecordObject(renderer, "Edit Sorting Order");
                renderer.sortingOrder = newSortingLayerOrder;
                EditorUtility.SetDirty(renderer);
            }

			changeChildren = EditorGUILayout.Toggle("Change Children", changeChildren);
        }

		void DrawChangeChildrenOnly()
		{
			if (_sortingLayerNames != null)
			{
				// Expose drop down for sorting layer
				int layerIndex = changeIndex;
				int newLayerIndex = EditorGUILayout.Popup("Sorting Layer", layerIndex, _sortingLayerNames);
				if (newLayerIndex != layerIndex)
				{
					changeIndex = newLayerIndex;
					string newLayerName = _sortingLayerNames[newLayerIndex];

					Renderer[] childRenderers = targetObject.GetComponentsInChildren<Renderer>();
					Undo.RecordObjects(childRenderers, "Edit Sorting Layer");
					foreach (Renderer child in childRenderers)
					{
						child.sortingLayerName = newLayerName;
						EditorUtility.SetDirty(child);
					}
				}
			}
		}
    }
}
