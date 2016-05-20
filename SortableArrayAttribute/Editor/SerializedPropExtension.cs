using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace UnityToolbag
{
	static internal class SerializedPropExtension
	{
		#region Simple string path based extensions
		/// <summary>
		/// Returns the path to the parent of a SerializedProperty
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static string ParentPath(this SerializedProperty prop)
		{
			int lastDot = prop.propertyPath.LastIndexOf('.');
			if (lastDot == -1) // No parent property
				return "";

			return prop.propertyPath.Substring(0, lastDot);
		}

		/// <summary>
		/// Returns the parent of a SerializedProperty, as another SerializedProperty
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static SerializedProperty GetParentProp(this SerializedProperty prop)
		{
			string parentPath = prop.ParentPath();
			return prop.serializedObject.FindProperty(parentPath);
		}
		#endregion

		/// <summary>
		/// Expand the given SerializedProperty and propogate the change up the hierarchy
		/// </summary>
		/// <param name="prop"></param>
		/// <param name="expand"></param>
		public static void ExpandHierarchy(this SerializedProperty prop, bool expand = true)
		{
			prop.isExpanded = expand;
			SerializedProperty parent = GetParentProp(prop);
			if (parent != null)
				ExpandHierarchy(parent);
		}

		#region Reflection extensions
		// http://answers.unity3d.com/questions/425012/get-the-instance-the-serializedproperty-belongs-to.html

		/// <summary>
		/// Use reflection to get the actual data instance of a SerializedProperty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static T GetValue<T>(this SerializedProperty prop)
		{
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');
			foreach (var element in elements)
			{
				if (element.Contains("["))
				{
					var elementName = element.Substring(0, element.IndexOf("["));
					var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
					obj = GetValue(obj, elementName, index);
				}
				else
				{
					obj = GetValue(obj, element);
				}
			}
			return (T) obj;
		}

		/// <summary>
		/// Uses reflection to get the actual data instance of the parent of a SerializedProperty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static T GetParent<T>(this SerializedProperty prop)
		{
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');
			foreach (var element in elements.Take(elements.Length - 1))
			{
				if (element.Contains("["))
				{
					var elementName = element.Substring(0, element.IndexOf("["));
					var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
					obj = GetValue(obj, elementName, index);
				}
				else
				{
					obj = GetValue(obj, element);
				}
			}
			return (T) obj;
		}

		private static object GetValue(object source, string name)
		{
			if (source == null)
				return null;
			Type type = source.GetType();
			FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (f == null)
			{
				PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (p == null)
					return null;
				return p.GetValue(source, null);
			}
			return f.GetValue(source);
		}

		private static object GetValue(object source, string name, int index)
		{
			var enumerable = GetValue(source, name) as IEnumerable;
			var enm = enumerable.GetEnumerator();
			while (index-- >= 0)
				enm.MoveNext();
			return enm.Current;
		}

		/// <summary>
		/// Check if a SerializedProperty has a given attribute
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static bool HasAttribute<T>(this SerializedProperty prop)
		{
			object[] attributes = GetAttributes<T>(prop);
			if (attributes != null)
			{
				return attributes.Length > 0;
			}
			return false;
		}

		/// <summary>
		/// Use reflection to get the attributes on a SerializedProperty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static object[] GetAttributes<T>(this SerializedProperty prop)
		{
			object obj = GetParent<object>(prop);
			Type objType = obj.GetType();
			const BindingFlags bindingFlags = System.Reflection.BindingFlags.GetField
			                                  | System.Reflection.BindingFlags.GetProperty
			                                  | System.Reflection.BindingFlags.Instance
			                                  | System.Reflection.BindingFlags.NonPublic
			                                  | System.Reflection.BindingFlags.Public;
			FieldInfo field = objType.GetField(prop.name, bindingFlags);
			if (field != null)
				return field.GetCustomAttributes(typeof (T), true);
			return null;
		}

		#endregion
	}
}