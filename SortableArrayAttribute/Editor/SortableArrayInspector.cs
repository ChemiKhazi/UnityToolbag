using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityToolbag
{
	[CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
	[CanEditMultipleObjects]
	public class SortableArrayInspector : Editor
	{
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

		protected class SortableListData
		{
			public string Parent		{ get; private set; }

			private Dictionary<string, ReorderableList> propIndex = new Dictionary<string, ReorderableList>();

			public SortableListData(string parent)
			{
				Parent = parent;
			}

			public void AddProperty(SerializedProperty property)
			{
				if (GetGrandParentPath(property).Equals(Parent) == false)
					return;

				ReorderableList list = new ReorderableList(property.serializedObject, property,
														draggable:true, displayHeader:false,
														displayAddButton:true, displayRemoveButton:true);
				
				list.drawElementCallback = delegate(Rect rect, int index, bool active, bool focused)
				{
					SerializedProperty targetElement = property.GetArrayElementAtIndex(index);
					rect.height = EditorGUI.GetPropertyHeight(targetElement);
					EditorGUI.PropertyField(rect, targetElement);

#if UNITY_5_1 || UNITY_5_2
					list.elementHeight = Mathf.Max(rect.height, EditorGUIUtility.singleLineHeight);
#endif
				};

#if UNITY_5_3 || UNITY_5_4
				list.elementHeightCallback = delegate(int index)
				{
					SerializedProperty arrayElement = property.GetArrayElementAtIndex(index);
					return EditorGUI.GetPropertyHeight(arrayElement);
				};
#endif


				propIndex.Add(property.propertyPath, list);
			}

			public bool DoLayoutProperty(SerializedProperty property)
			{
				if (propIndex.ContainsKey(property.propertyPath) == false)
					return false;

				string headerText = string.Format("{0} [{1}]", property.displayName, property.arraySize);
				EditorGUILayout.PropertyField(property, new GUIContent(headerText), false);
				
				Rect dropRect = GUILayoutUtility.GetLastRect();

				if (property.isExpanded)
				{
					propIndex[property.propertyPath].DoLayoutList();
				}

				// Handle drag and drop
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
							property.GetArrayElementAtIndex(newIndex).objectReferenceInstanceIDValue = dragged_object.GetInstanceID();
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
		}

		protected List<SortableListData> listIndex;

		protected bool isInitialized = false;
		protected bool hasSortableArrays = false;

		private void OnEnable()
		{
			InitInspector();
		}

		~SortableArrayInspector()
		{
			listIndex.Clear();
			isInitialized = false;
			hasSortableArrays = false;
		}

		protected virtual void InitInspector()
		{
			if (isInitialized)
				return;

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
						if (iterProp.HasAttribute<SortableArrayAttribute>())
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

		private void CreateListData(SerializedProperty property)
		{
			string parent = GetGrandParentPath(property);

			SortableListData data = listIndex.Find(listData => listData.Parent.Equals(parent));
			if (data == null)
			{
				data = new SortableListData(parent);
				listIndex.Add(data);
			}

			data.AddProperty(property);
		}

		public override void OnInspectorGUI()
		{
			if (listIndex == null)
				InitInspector();

			if (hasSortableArrays && listIndex != null)
			{
				DrawDefaultSortable();
				serializedObject.ApplyModifiedProperties();
				return;
			}

			base.OnInspectorGUI();
		}

		protected void IterateSerializedProp(SerializedProperty property)
		{
			if (property.NextVisible(true))
			{
				// Remember this iteration is at
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

		protected void DrawPropertySortableArray(SerializedProperty property)
		{
			// Try to get the sortable list this property belongs to
			SortableListData listData = listIndex.Find(data => property.propertyPath.StartsWith(data.Parent));
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
				EditorGUILayout.PropertyField(targetProp, targetProp.isExpanded);
			}
		}

		#region Helper functions
		protected void DrawDefaultSortable()
		{
			SerializedProperty iterProp = serializedObject.GetIterator();
			IterateSerializedProp(iterProp);
		}

		protected void DrawPropertiesFrom(string propertyStart)
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

		protected void DrawPropertiesUpTo(string propertyStop)
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

		protected void DrawPropertiesFromUpTo(string propertyStart, string propertyStop)
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
		#endregion
	}
}