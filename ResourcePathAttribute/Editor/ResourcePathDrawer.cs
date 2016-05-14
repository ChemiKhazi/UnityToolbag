using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using Object = UnityEngine.Object;

namespace UnityToolbag
{
	[CustomPropertyDrawer(typeof(ResourcePathAttribute))]
	public class ResourcePathDrawer : PropertyDrawer
	{
		private const double flashTime = 0.25f;
		private static readonly Color flashColor = new Color(0.9f, 0f, 0f, 1f);

		private double flashStarted = -1d;
		private float flashAlpha = 0f;

		private SerializedProperty targetProp = null;

		public static bool TryGetResourcePath(string assetPath, out string resourcePath)
		{
			resourcePath = "";

			string[] pathSections = assetPath.Split(new string[] { "Resources/" }, StringSplitOptions.None);
			if (pathSections.Length != 2)
				return false;

			resourcePath = pathSections[1];
			resourcePath = resourcePath.Substring(0, resourcePath.LastIndexOf('.'));

			return true;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Reject drawing this custom attribute if property wasn't a string type
			if (property.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			ResourcePathAttribute pathAttribute = (ResourcePathAttribute) attribute;

			// Can't get attribute for some reason
			if (pathAttribute == null)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			if (flashAlpha > 0 && SerializedProperty.EqualContents(targetProp, property))
			{
				EditorGUI.DrawRect(position, new Color(flashColor.r, flashColor.g, flashColor.b, flashAlpha));
			}

			Object resourceObject = Resources.Load(property.stringValue);
			Object newObject = EditorGUI.ObjectField(position, label, resourceObject, pathAttribute.allowedType, false);

			if (resourceObject == newObject)
				return;

			if (pathAttribute.allowedType != null && newObject != null)
			{
				bool isCorrectType = newObject.GetType() == pathAttribute.allowedType;
				if (isCorrectType == false)
					return;
			}

			// Object is null, empty the string value
			if (newObject == null)
			{
				property.stringValue = "";
				property.serializedObject.ApplyModifiedProperties();
			}
			else
			{
				string fullPath = AssetDatabase.GetAssetPath(newObject);
				string resourcePath;
				if (TryGetResourcePath(fullPath, out resourcePath))
				{
					property.stringValue = resourcePath;
					property.serializedObject.ApplyModifiedProperties();
				}
				else // Not a resource object, flash the inspector
				{
					flashStarted = EditorApplication.timeSinceStartup;
					targetProp = property;

					EditorApplication.update += FlashInspector;
				}
			}
		}

		private void StartInspectorFlash()
		{
			
		}

		private void FlashInspector()
		{
			double passedTime = EditorApplication.timeSinceStartup - flashStarted;
			double t = passedTime/flashTime;

			if (t > 1f)
			{
				EditorApplication.update -= FlashInspector;
				flashAlpha = 0f;
				flashStarted = -1d;
				targetProp = null;
			}
			else
			{
				flashAlpha = 1 - Convert.ToSingle(t);
				EditorUtility.SetDirty(targetProp.serializedObject.targetObject);
			}
		}
	}
}
