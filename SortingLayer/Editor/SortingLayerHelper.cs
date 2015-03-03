using UnityEngine;
using System;
using System.Reflection;

namespace UnityToolbag
{
    // Helpers used by the different sorting layer classes.
    public static class SortingLayerHelper
    {
        private static Type _utilityType;
        private static PropertyInfo _sortingLayerNamesProperty;
        private static MethodInfo _getSortingLayerUserIdMethod;

        static SortingLayerHelper()
        {
            _utilityType = Type.GetType("UnityEditorInternal.InternalEditorUtility, UnityEditor");
            _sortingLayerNamesProperty = _utilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            _getSortingLayerUserIdMethod = _utilityType.GetMethod("GetSortingLayerUserID", BindingFlags.Static | BindingFlags.NonPublic);
        }

        // Gets an array of sorting layer names.
        // Since this uses reflection, callers should check for 'null' which will be returned if the reflection fails.
        public static string[] sortingLayerNames
        {
            get
            {
                if (_sortingLayerNamesProperty == null) {
                    return null;
                }

                return _sortingLayerNamesProperty.GetValue(null, null) as string[];
            }
        }

        // Given the ID of a sorting layer, returns the sorting layer's name
        public static string GetSortingLayerNameFromID(int id)
        {
            string[] names = sortingLayerNames;
            if (names == null) {
                return null;
            }

            for (int i = 0; i < names.Length; i++) {
                if (GetSortingLayerIDForIndex(i) == id) {
                    return names[i];
                }
            }

            return null;
        }

        // Given the name of a sorting layer, returns the ID.
        public static int GetSortingLayerIDForName(string name)
        {
            string[] names = sortingLayerNames;
            if (names == null) {
                return 0;
            }

            return GetSortingLayerIDForIndex(Array.IndexOf(names, name));
        }

        // Helper to convert from a sorting layer INDEX to a sorting layer ID. These are not the same thing.
        // IDs are based on the order in which layers were created and do not change when reordering the layers.
        // Thankfully there is a private helper we can call to get the ID for a layer given its index.
        public static int GetSortingLayerIDForIndex(int index)
        {
            if (_getSortingLayerUserIdMethod == null) {
                return 0;
            }

            return (int)_getSortingLayerUserIdMethod.Invoke(null, new object[] { index });
        }
    }
}
