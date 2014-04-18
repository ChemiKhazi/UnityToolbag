/*
 * Copyright (c) 2014, Nick Gravelyn.
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

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityToolbag
{
    [InitializeOnLoad]
    public class UnityLock : EditorWindow
    {
        private const string LockMenuItem = "GameObject/UnityLock/Lock GameObject";
        private const string LockRecursivelyMenuItem = "GameObject/UnityLock/Lock GameObject and Children %#l";
        private const string UnlockMenuItem = "GameObject/UnityLock/Unlock GameObject";
        private const string UnlockRecursivelyMenuItem = "GameObject/UnityLock/Unlock GameObject and Children %#u";

        private const string ShowIconPrefKey = "UnityLock_ShowIcon";
        private const string AddUndoRedoPrefKey = "UnityLock_EnableUndoRedo";
        private const string DisableSelectionPrefKey = "UnityLock_DisableSelection";

        static UnityLock()
        {
            if (!EditorPrefs.HasKey(ShowIconPrefKey)) {
                EditorPrefs.SetBool(ShowIconPrefKey, true);
            }

            if (!EditorPrefs.HasKey(AddUndoRedoPrefKey)) {
                EditorPrefs.SetBool(AddUndoRedoPrefKey, true);
            }

            if (!EditorPrefs.HasKey(DisableSelectionPrefKey)) {
                EditorPrefs.SetBool(DisableSelectionPrefKey, false);
            }
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            HierarchyIcon.drawingIcon = ShowPrefsBoolOption(ShowIconPrefKey, "Show lock icon in hierarchy");
            EditorGUILayout.HelpBox(
                "When enabled a small lock icon will appear in the hierarchy view for all locked objects.",
                MessageType.None);

            EditorGUILayout.Space();

            bool wasSelectionDisabled = EditorPrefs.GetBool(DisableSelectionPrefKey);
            bool isSelectionDisabled = ShowPrefsBoolOption(DisableSelectionPrefKey, "Disable selecting locked objects");
            EditorGUILayout.HelpBox(
                "When enabled locked objects will not be selectable in the scene view with a left click. Some objects can still be selected by using a selection rectangle; it doesn't appear to be possible to prevent this.\n\nObjects represented only with gizmos will not be drawn as gizmos aren't rendered when selection is disabled.",
                MessageType.None);

            if (wasSelectionDisabled != isSelectionDisabled) {
                ToggleSelectionOfLockedObjects(isSelectionDisabled);
            }

            EditorGUILayout.Space();

            ShowPrefsBoolOption(AddUndoRedoPrefKey, "Support undo/redo for lock and unlock");
            EditorGUILayout.HelpBox(
                "When enabled the lock and unlock operations will be properly inserted into the undo stack just like any other action.\n\nIf this is disabled the Undo button will never lock or unlock an object. This can cause other operations to silently fail, such as trying to undo a translation on a locked object.",
                MessageType.None);

            EditorGUILayout.EndVertical();
        }

        bool ShowPrefsBoolOption(string key, string name)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(name, GUILayout.ExpandWidth(true));
            bool oldValue = EditorPrefs.GetBool(key);
            bool newValue = EditorGUILayout.Toggle(oldValue, GUILayout.Width(20));
            if (newValue != oldValue) {
                EditorPrefs.SetBool(key, newValue);
            }

            EditorGUILayout.EndHorizontal();

            return newValue;
        }

        [MenuItem("Window/UnityLock Preferences")]
        static void ShowPreferences()
        {
            var window = GetWindow<UnityLock>();
            window.minSize = new Vector2(275, 300);
            window.Show();
        }

        private static void ToggleSelectionOfLockedObjects(bool disableSelection)
        {
            foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject))) {
                if ((go.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable) {
                    foreach (Component comp in go.GetComponents(typeof(Component))) {
                        if (!(comp is Transform)) {
                            if (disableSelection) {
                                comp.hideFlags |= HideFlags.NotEditable;
                                comp.hideFlags |= HideFlags.HideInHierarchy;
                            }
                            else {
                                comp.hideFlags &= ~HideFlags.NotEditable;
                                comp.hideFlags &= ~HideFlags.HideInHierarchy;
                            }
                        }
                    }

                    EditorUtility.SetDirty(go);
                }
            }
        }

        private static void LockObject(GameObject go)
        {
            if (EditorPrefs.GetBool(AddUndoRedoPrefKey)) {
                Undo.RecordObject(go, "Lock Object");
            }

            go.hideFlags |= HideFlags.NotEditable;

            foreach (Component comp in go.GetComponents(typeof(Component))) {
                if (!(comp is Transform)) {
                    if (EditorPrefs.GetBool(DisableSelectionPrefKey)) {
                        comp.hideFlags |= HideFlags.NotEditable;
                        comp.hideFlags |= HideFlags.HideInHierarchy;
                    }
                }
            }

            EditorUtility.SetDirty(go);
        }

        private static void UnlockObject(GameObject go)
        {
            if (EditorPrefs.GetBool(AddUndoRedoPrefKey)) {
                Undo.RecordObject(go, "Unlock Object");
            }

            go.hideFlags &= ~HideFlags.NotEditable;

            foreach (Component comp in go.GetComponents(typeof(Component))) {
                if (!(comp is Transform)) {
                    // Don't check pref key; no harm in removing flags that aren't there
                    comp.hideFlags &= ~HideFlags.NotEditable;
                    comp.hideFlags &= ~HideFlags.HideInHierarchy;
                }
            }

            EditorUtility.SetDirty(go);
        }

        [MenuItem(LockMenuItem)]
        static void Lock()
        {
            foreach (var go in Selection.gameObjects) {
                LockObject(go);
            }
        }

        [MenuItem(LockMenuItem, true)]
        static bool CanLock()
        {
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem(LockRecursivelyMenuItem)]
        static void LockRecursively()
        {
            Stack<GameObject> objectsToLock = new Stack<GameObject>(Selection.gameObjects);

            while (objectsToLock.Count > 0) {
                var go = objectsToLock.Pop();
                LockObject(go);

                foreach (Transform childTransform in go.transform) {
                    objectsToLock.Push(childTransform.gameObject);
                }
            }
        }

        [MenuItem(LockRecursivelyMenuItem, true)]
        static bool CanLockRecursively()
        {
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem(UnlockMenuItem)]
        static void Unlock()
        {
            foreach (var go in Selection.gameObjects)
            {
                UnlockObject(go);
            }
        }

        [MenuItem(UnlockMenuItem, true)]
        static bool CanUnlock()
        {
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem(UnlockRecursivelyMenuItem)]
        static void UnlockRecursively()
        {
            Stack<GameObject> objectsToUnlock = new Stack<GameObject>(Selection.gameObjects);

            while (objectsToUnlock.Count > 0) {
                var go = objectsToUnlock.Pop();
                UnlockObject(go);

                foreach (Transform childTransform in go.transform) {
                    objectsToUnlock.Push(childTransform.gameObject);
                }
            }
        }

        [MenuItem(UnlockRecursivelyMenuItem, true)]
        static bool CanUnlockRecursively()
        {
            return Selection.gameObjects.Length > 0;
        }

        [InitializeOnLoad]
        private static class HierarchyIcon
        {
            private const float _iconSize = 16f;
            private static string _iconFile;
            private static Texture2D _icon;
            private static bool _drawingIcon;

            private static string iconFile
            {
                get
                {
                    if (string.IsNullOrEmpty(_iconFile)) {
                        var path = Directory.GetFiles(Application.dataPath, "UnityLockHierarchyIcon.png", SearchOption.AllDirectories)[0];
                        _iconFile = "Assets" + path.Substring(Application.dataPath.Length).Replace('\\', '/');
                    }

                    return _iconFile;
                }
            }

            public static bool drawingIcon
            {
                get { return _drawingIcon; }
                set
                {
                    if (_drawingIcon != value) {
                        _drawingIcon = value;

                        if (_drawingIcon) {
                            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
                        }
                        else {
                            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
                        }

                        EditorApplication.RepaintHierarchyWindow();
                    }
                }
            }

            static HierarchyIcon()
            {
                drawingIcon = EditorPrefs.GetBool(UnityLock.ShowIconPrefKey);
            }

            private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
            {
                var obj = EditorUtility.InstanceIDToObject(instanceID) as UnityEngine.Object;
                if (obj && (obj.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable) {
                    if (!_icon) {
                        _icon = AssetDatabase.LoadAssetAtPath(iconFile, typeof(Texture2D)) as Texture2D;
                    }

                    GUI.Box(
                        new Rect(selectionRect.xMax - _iconSize, selectionRect.center.y - (_iconSize / 2f), _iconSize, _iconSize),
                        _icon,
                        GUIStyle.none);
                }
            }
        }
    }
}
