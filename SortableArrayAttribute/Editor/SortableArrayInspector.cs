﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityToolbag
{
	[CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
	[CanEditMultipleObjects]
	public class SortableArrayInspector : Editor
	{
		// Set this to true to turn every array in non custom inspectors into reorderable lists
		private const bool LIST_ALL_ARRAYS = false;

		protected static string GetGrandParentPath(SerializedProperty property)
		{
			string parent = property.propertyPath;
			int firstDot = property.propertyPath.IndexOf('.');
			if (firstDot > 0)
			{
				parent = property.propertyPath.Substring(0, firstDot);
			}
			return parent;
		}

		/// <summary>
		/// Internal class that manages ReorderableLists for each reorderable
		/// SerializedProperty in a SerializedObject's direct child
		/// </summary>
		protected class SortableListData
		{
			public string Parent		{ get; private set; }
			public Func<int, string> ElementHeaderCallback = null;

			private Dictionary<string, ReorderableList> propIndex = new Dictionary<string, ReorderableList>();

			public SortableListData(string parent)
			{
				Parent = parent;
			}

			public void AddProperty(SerializedProperty property)
			{
				// Check if this property actually belongs to the same direct child
				if (GetGrandParentPath(property).Equals(Parent) == false)
					return;

				ReorderableList propList = new ReorderableList(
					property.serializedObject, property,
					draggable: true, displayHeader: false,
					displayAddButton: true, displayRemoveButton: true)
				{
					headerHeight = 5
				};

				propList.drawElementCallback = delegate(Rect rect, int index, bool active, bool focused)
				{
					SerializedProperty targetElement = property.GetArrayElementAtIndex(index);

					bool isExpanded = targetElement.isExpanded;
					rect.height = EditorGUI.GetPropertyHeight(targetElement, GUIContent.none, isExpanded);

					if (targetElement.hasVisibleChildren)
						rect.xMin += 10;

					// Get Unity to handle drawing each element
					GUIContent propHeader = new GUIContent(targetElement.displayName);
					if (ElementHeaderCallback != null)
						propHeader.text = ElementHeaderCallback(index);
					EditorGUI.PropertyField(rect, targetElement, propHeader, isExpanded);
					// Height might have changed when dealing with serialized class
					// Call the select callback when height changes to reset the list elementHeight
					float newHeight = EditorGUI.GetPropertyHeight(targetElement, GUIContent.none, targetElement.isExpanded);
					if (rect.height != newHeight)
					{
						propList.onSelectCallback(propList);
#if UNITY_5_1 || UNITY_5_2
						propList.elementHeight = Mathf.Max(propList.elementHeight, newHeight);
#endif
					}
				};

				propList.onSelectCallback = delegate(ReorderableList list)
				{
					SerializedProperty targetElement = property.GetArrayElementAtIndex(list.index);
					list.elementHeight = EditorGUI.GetPropertyHeight(targetElement, GUIContent.none, targetElement.isExpanded);
				};

				// Unity 5.3 onwards allows reorderable lists to have variable element heights
#if UNITY_5_3_OR_NEWER
				propList.elementHeightCallback = delegate(int index)
				{
					SerializedProperty arrayElement = property.GetArrayElementAtIndex(index);
					return EditorGUI.GetPropertyHeight(arrayElement, GUIContent.none, arrayElement.isExpanded) + 3;
				};
#endif

				propIndex.Add(property.propertyPath, propList);
			}

			public bool DoLayoutProperty(SerializedProperty property)
			{
				if (propIndex.ContainsKey(property.propertyPath) == false)
					return false;

				// Draw the header
				string headerText = string.Format("{0} [{1}]", property.displayName, property.arraySize);
				EditorGUILayout.PropertyField(property, new GUIContent(headerText), false);
				
				// Save header rect for handling drag and drop
				Rect dropRect = GUILayoutUtility.GetLastRect();

				// Draw the reorderable list for the property
				if (property.isExpanded)
				{
					int newArraySize = EditorGUILayout.IntField("Size", property.arraySize);
					if (newArraySize != property.arraySize)
						property.arraySize = newArraySize;
					propIndex[property.propertyPath].DoLayoutList();
				}

				// Handle drag and drop into the header
				Event evt = Event.current;
				if (evt == null)
					return true;

				if (evt.type == EventType.dragUpdated || evt.type == EventType.dragPerform)
				{
					if (dropRect.Contains(evt.mousePosition) == false)
						return true;

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					if (evt.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						foreach (Object dragged_object in DragAndDrop.objectReferences)
						{
							if (dragged_object.GetType() != property.GetType())
								continue;

							int newIndex = property.arraySize;
							property.arraySize++;

							SerializedProperty target = property.GetArrayElementAtIndex(newIndex);
							target.objectReferenceInstanceIDValue = dragged_object.GetInstanceID();
						}
						evt.Use();
					}
				}
				return true;
			}

			public ReorderableList GetPropertyList(SerializedProperty property)
			{
				if (propIndex.ContainsKey(property.propertyPath))
					return propIndex[property.propertyPath];
				return null;
			}
		} // End SortableListData

		protected List<SortableListData> listIndex;

		protected bool isInitialized = false;
		protected bool hasSortableArrays = false;

		protected Dictionary<string, MethodInfo> contextMenus = new Dictionary<string, MethodInfo>(); 

		~SortableArrayInspector()
		{
			listIndex.Clear();
			isInitialized = false;
			hasSortableArrays = false;
		}

		#region Initialization
		private void OnEnable()
		{
			InitInspector();
		}

		protected void FindSortableArrays()
		{
			listIndex = new List<SortableListData>();

			SerializedProperty iterProp = serializedObject.GetIterator();
			// This iterator goes through all the child serialized properties, looking
			// for properties that have the SortableArray attribute
			if (iterProp.NextVisible(true))
			{
				do
				{
					if (iterProp.isArray && iterProp.propertyType != SerializedPropertyType.String)
					{
						bool canTurnToList = true;
						// If not going to list all arrays
						// Use SerializedPropExtension to check for attribute
						if (LIST_ALL_ARRAYS == false)
						{
							canTurnToList = iterProp.HasAttribute<SortableArrayAttribute>();
						}

						if (canTurnToList)
						{
							hasSortableArrays = true;
							CreateListData(serializedObject.FindProperty(iterProp.propertyPath));
						}
					}
				} while (iterProp.NextVisible(true));
			}

			isInitialized = true;
			if (hasSortableArrays == false)
			{
				listIndex = null;
			}
		}

		protected virtual void InitInspector()
		{
			if (isInitialized)
				return;

			FindSortableArrays();

			// Get context menu
			Type targetType = target.GetType();
			Type contextMenuType = typeof (ContextMenu);
			MethodInfo[] methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int index = 0; index < methods.GetLength(0); ++index)
			{
				MethodInfo methodInfo = methods[index];
				foreach (ContextMenu contextMenu in methodInfo.GetCustomAttributes(contextMenuType, false))
				{
					if (contextMenus.ContainsKey(contextMenu.menuItem))
					{
						contextMenus[contextMenu.menuItem] = methodInfo;
						continue;
					}
					contextMenus.Add(contextMenu.menuItem, methodInfo);
				}
			}
		}

		private void CreateListData(SerializedProperty property)
		{
			string parent = GetGrandParentPath(property);

			// Try to find the grand parent in SortableListData
			SortableListData data = listIndex.Find(listData => listData.Parent.Equals(parent));
			if (data == null)
			{
				data = new SortableListData(parent);
				listIndex.Add(data);
			}

			data.AddProperty(property);
			object[] attr = property.GetAttributes<SortableArrayAttribute>();
			if (attr != null && attr.Length == 1)
			{
				SortableArrayAttribute arrayAttr = (SortableArrayAttribute) attr[0];
				if (arrayAttr != null && string.IsNullOrEmpty(arrayAttr.ElementHeader) == false)
				{
					data.ElementHeaderCallback = delegate(int i)
					{
						return string.Format("{0} {1}", arrayAttr.ElementHeader, (arrayAttr.HeaderZeroIndex ? i : i + 1));
					};
				}
			}
		}

		protected ReorderableList GetSortableList(SerializedProperty property)
		{
			if (listIndex == null)
				return null;

			string parent = GetGrandParentPath(property);

			SortableListData data = listIndex.Find(listData => listData.Parent.Equals(parent));
			if (data == null)
				return null;

			return data.GetPropertyList(property);
		}
		#endregion

		public override void OnInspectorGUI()
		{
			// Not initialized, try initializing
			if (listIndex == null)
				InitInspector();

			if (hasSortableArrays && listIndex != null)
			{
				serializedObject.Update();
				EditorGUI.BeginChangeCheck();
				DrawDefaultSortable();
				if (EditorGUI.EndChangeCheck())
				{
					serializedObject.ApplyModifiedProperties();
					FindSortableArrays();
				}
				DrawContextMenuButtons();
				return;
			}

			base.OnInspectorGUI();
			DrawContextMenuButtons();
		}

		protected void IterateSerializedProp(SerializedProperty property)
		{
			if (property.NextVisible(true))
			{
				// Remember depth iteration started from
				int depth = property.Copy().depth;
				do
				{
					// If goes deeper than the iteration depth, get out
					if (property.depth != depth)
						break;

					DrawPropertySortableArray(property);
				} while (property.NextVisible(false));
			}
		}

		/// <summary>
		/// Draw a SerializedProperty as a ReorderableList if it was found during
		/// initialization, otherwise use EditorGUILayout.PropertyField
		/// </summary>
		/// <param name="property"></param>
		protected void DrawPropertySortableArray(SerializedProperty property)
		{
			// Try to get the sortable list this property belongs to
			SortableListData listData = null;
			if (listIndex != null)
				listData = listIndex.Find(data => property.propertyPath.StartsWith(data.Parent));

			if (listData != null)
			{
				// Try to show the list
				if (listData.DoLayoutProperty(property) == false)
				{
					EditorGUILayout.PropertyField(property, false);
					if (property.isExpanded)
					{
						EditorGUI.indentLevel++;
						SerializedProperty targetProp = serializedObject.FindProperty(property.propertyPath);
						IterateSerializedProp(targetProp);
						EditorGUI.indentLevel--;
					}

				}
			}
			else
			{
				SerializedProperty targetProp = serializedObject.FindProperty(property.propertyPath);
				bool restoreEnable = GUI.enabled;
				if (targetProp.propertyPath.StartsWith("m_"))
					GUI.enabled = false;
				EditorGUILayout.PropertyField(targetProp, targetProp.isExpanded);
				GUI.enabled = restoreEnable;
			}
		}

		#region Helper functions
		/// <summary>
		/// Draw the default inspector, with the sortable arrays
		/// </summary>
		public void DrawDefaultSortable()
		{
			SerializedProperty iterProp = serializedObject.GetIterator();
			IterateSerializedProp(iterProp);
		}

		/// <summary>
		/// Draw the default inspector, except for the given property names
		/// </summary>
		/// <param name="propertyNames"></param>
		public void DrawSortableExcept(params string[] propertyNames)
		{
			SerializedProperty iterProp = serializedObject.GetIterator();
			if (iterProp.NextVisible(true))
			{
				do
				{
					if (propertyNames.Contains(iterProp.name))
						continue;
					DrawPropertySortableArray(iterProp);
				} while (iterProp.NextVisible(false));
			}
		}

		/// <summary>
		/// Draw the default inspector, starting from a given property
		/// </summary>
		/// <param name="propertyStart">Property name to start from</param>
		public void DrawPropertiesFrom(string propertyStart)
		{
			bool canDraw = false;
			SerializedProperty iterProp = serializedObject.GetIterator();
			if (iterProp.NextVisible(true))
			{
				do
				{
					if (iterProp.name.Equals(propertyStart))
						canDraw = true;

					if (canDraw == false)
						continue;

					DrawPropertySortableArray(iterProp);

				} while (iterProp.NextVisible(false));
			}
		}

		/// <summary>
		/// Draw the default inspector, up to a given property
		/// </summary>
		/// <param name="propertyStop">Property name to stop at</param>
		public void DrawPropertiesUpTo(string propertyStop)
		{
			SerializedProperty iterProp = serializedObject.GetIterator();
			if (iterProp.NextVisible(true))
			{
				do
				{
					if (iterProp.name.Equals(propertyStop))
						break;
					DrawPropertySortableArray(iterProp);
				} while (iterProp.NextVisible(false));
			}
		}

		/// <summary>
		/// Draw the default inspector, starting from a given property to a stopping property
		/// </summary>
		/// <param name="propertyStart">Property name to start from</param>
		/// <param name="propertyStop">Property name to stop at</param>
		public void DrawPropertiesFromUpTo(string propertyStart, string propertyStop)
		{
			bool canDraw = false;
			SerializedProperty iterProp = serializedObject.GetIterator();
			if (iterProp.NextVisible(true))
			{
				do
				{
					if (iterProp.name.Equals(propertyStart))
						canDraw = true;

					if (canDraw == false)
						continue;

					DrawPropertySortableArray(iterProp);

					if (iterProp.name.Equals(propertyStop))
						break;
				} while (iterProp.NextVisible(false));
			}
		}

		public void DrawContextMenuButtons()
		{
			if (contextMenus.Count == 0) return;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Context Menu", EditorStyles.boldLabel);
			foreach (KeyValuePair<string, MethodInfo> kv in contextMenus)
			{
				if (GUILayout.Button(kv.Key))
				{
					kv.Value.Invoke(target, null);
				}
			}
		}
		#endregion
	}
}