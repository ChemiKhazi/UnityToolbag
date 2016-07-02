﻿using System;
﻿using System.Collections.Generic;
﻿using System.Reflection;
﻿using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityToolbag
{
    [InitializeOnLoad]
    public class QuickToggle
    {
        private const string PrefKeyShowToggle = "UnityToolbag.QuickToggle.Visible";

		private static readonly Type HierarchyWindowType;
		private static GUIStyle styleLock, styleLockUnselected, styleVisOn, styleVisOff;

		#region Menu stuff
	    [MenuItem("Window/Hierarchy Quick Toggle")]
        static void QuickToggleMenu()
        {
            bool toggle = EditorPrefs.GetBool(PrefKeyShowToggle);
            ShowQuickToggle(!toggle);
        }
		#endregion

	    static QuickToggle()
	    {
		    if (EditorPrefs.HasKey(PrefKeyShowToggle) == false) {
			    EditorPrefs.SetBool(PrefKeyShowToggle, false);
		    }

		    Assembly editorAssembly = typeof(EditorWindow).Assembly;
		    HierarchyWindowType = editorAssembly.GetType("UnityEditor.SceneHierarchyWindow");

			ResetVars();
		    ShowQuickToggle(EditorPrefs.GetBool(PrefKeyShowToggle));
	    }

	    private static void ShowQuickToggle(bool show)
        {
            EditorPrefs.SetBool(PrefKeyShowToggle, show);

            if (show)
            {
				ResetVars();
				EditorApplication.update += HandleEditorUpdate;
                EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyItem;
            }
            else
			{
				EditorApplication.update -= HandleEditorUpdate;
                EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyItem;
            }
            EditorApplication.RepaintHierarchyWindow();
        }

	    private struct ObjectState
	    {
		    public bool visible;
		    public bool locked;

		    public ObjectState(bool visible, bool locked)
		    {
			    this.visible = visible;
			    this.locked = locked;
		    }
	    }

	    private static ObjectState	propogateState;

		// Because we can't hook into OnGUI of HierarchyWindow, doing a hack
		// button that involves the editor update loop and the hierarchy item draw event
		private static bool	isFrameFresh;
	    private static bool	isMousePressed;

	    private static void ResetVars()
	    {
		    isFrameFresh = false;
		    isMousePressed = false;
	    }

	    private static void HandleEditorUpdate()
	    {   
		    EditorWindow window = EditorWindow.mouseOverWindow;
		    if (window == null)
		    {
				ResetVars();
			    return;
		    }

		    if (window.GetType() == HierarchyWindowType)
		    {
			    if (window.wantsMouseMove == false)
				    window.wantsMouseMove = true;

			    isFrameFresh = true;
		    }
	    }

	    private static void DrawHierarchyItem(int instanceId, Rect selectionRect)
        {
            BuildStyles();

            GameObject target = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (target == null)
                return;

            // Reserve the draw rects
            Rect visRect = new Rect(selectionRect)
            {
                xMin = selectionRect.xMax - (selectionRect.height * 2.1f),
                xMax = selectionRect.xMax - selectionRect.height
            };
            Rect lockRect = new Rect(selectionRect)
            {
                xMin = selectionRect.xMax - selectionRect.height
            };

			// Get states
			bool isActive = target.activeSelf;
			bool isLocked = (target.hideFlags & HideFlags.NotEditable) > 0;
			
			// Draw the visibility toggle
		    GUIStyle visStyle = (isActive) ? styleVisOn : styleVisOff;
			GUI.Label(visRect, GUIContent.none, visStyle);

			// Draw lock toggle
			GUIStyle lockStyle = (isLocked) ? styleLock : styleLockUnselected;
			GUI.Label(lockRect, GUIContent.none, lockStyle);

		    if (Event.current == null)
			    return;

			Event evt = Event.current;

			bool toggleActive = visRect.Contains(evt.mousePosition);
			bool toggleLock = lockRect.Contains(evt.mousePosition);
			bool stateChanged = (toggleActive || toggleLock);
				
			bool doMouse = false;
			switch (evt.type)
			{
				case EventType.MouseDown:
					// Checking is frame fresh so mouse state is only tested once per frame
					// instead of every time a hierarchy item is drawn
					bool isMouseDown = false;
					if (isFrameFresh && stateChanged)
					{
						isMouseDown = !isMousePressed;
						isMousePressed = true;
						isFrameFresh = false;
					}

					if (stateChanged && isMouseDown)
					{
						doMouse = true;
						if (toggleActive) isActive = !isActive;
						if (toggleLock) isLocked = !isLocked;

						propogateState = new ObjectState(isActive, isLocked);
						evt.Use();
					}
					break;
				case EventType.MouseDrag:
					doMouse = isMousePressed;
					break;
				case EventType.MouseUp:
					ResetVars();
					break;
			}
				
			if (doMouse && stateChanged)
			{
				SetVisible(target, propogateState.visible);
				SetLockObject(target, propogateState.locked);
				EditorApplication.RepaintHierarchyWindow();
			}
        }

        private static Object[] GatherObjects(GameObject root)
        {
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            Stack<GameObject> recurseStack = new Stack<GameObject>(new GameObject[] { root });

            while (recurseStack.Count > 0)
            {
                GameObject obj = recurseStack.Pop();
                objects.Add(obj);

                foreach (Transform childT in obj.transform)
                    recurseStack.Push(childT.gameObject);
            }
            return objects.ToArray();
        }

        private static void SetLockObject(GameObject target, bool isLocked)
        {
	        bool objectLockState = (target.hideFlags & HideFlags.NotEditable) > 0;
            if (objectLockState == isLocked)
		        return;

	        Object[] objects = GatherObjects(target);
            string undoString = string.Format("{0} {1}", isLocked ? "Lock" : "Unlock", target.name);
            Undo.RecordObjects(objects, undoString);

            foreach (Object obj in objects)
            {
                GameObject go = (GameObject)obj;

                // Set state according to isLocked
                if (isLocked)
                {
                    go.hideFlags |= HideFlags.NotEditable;
                }
                else
                {
                    go.hideFlags &= ~HideFlags.NotEditable;
                }

                // Set hideflags of components
                foreach (Component comp in go.GetComponents<Component>())
                {
                    if (comp is Transform)
                        continue;

                    if (isLocked)
                    {
                        comp.hideFlags |= HideFlags.NotEditable;
                        comp.hideFlags |= HideFlags.HideInHierarchy;
                    }
                    else
                    {
                        comp.hideFlags &= ~HideFlags.NotEditable;
                        comp.hideFlags &= ~HideFlags.HideInHierarchy;
                    }
                    EditorUtility.SetDirty(comp);
                }
                EditorUtility.SetDirty(obj);
            }
        }

        private static void SetVisible(GameObject target, bool isActive)
        {
			if (target.activeSelf == isActive) return;
	        
            string undoString = string.Format("{0} {1}",
                                        isActive ? "Show" : "Hide",
                                        target.name);
            Undo.RecordObject(target, undoString);

            target.SetActive(isActive);
            EditorUtility.SetDirty(target);
        }

        private static void BuildStyles()
        {
            // All of the styles have been built, don't do anything
            if (styleLock != null &&
                styleLockUnselected != null &&
                styleVisOn != null &&
				styleVisOff != null)
            {
                return;
            }

            // First, get the textures for the GUIStyles
            Texture2D icnLockOn = null, icnLockOnActive = null;
            bool normalPassed = false;
            bool activePassed = false;

            // Resource name of icon images
            const string resLockActive = "IN LockButton on";
            const string resLockOn = "IN LockButton on act";

            // Loop through all of the icons inside Resources
            // which contains editor UI textures
            Texture2D[] resTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D resTexture in resTextures)
            {
                // Regular icon
                if (resTexture.name.Equals(resLockOn))
                {
                    // if not using pro skin, use the first 'IN LockButton on'
                    // that is passed when iterating
                    if (!EditorGUIUtility.isProSkin && !normalPassed)
                        icnLockOn = resTexture;
                    else
                        icnLockOn = resTexture;
                    normalPassed = true;
                }

                // active icon
                if (resTexture.name.Equals(resLockActive))
                {
                    if (!EditorGUIUtility.isProSkin && !activePassed)
                        icnLockOnActive = resTexture;
                    else
                        icnLockOnActive = resTexture;
                    activePassed = true;
                }
            }

            // Now build the GUI styles
            // Using icons different from regular lock button so that
            // it would look darker
            styleLock = new GUIStyle(GUI.skin.FindStyle("IN LockButton"))
            {
                onNormal = new GUIStyleState() { background = icnLockOn },
                onHover = new GUIStyleState() { background = icnLockOn },
                onFocused = new GUIStyleState() { background = icnLockOn },
                onActive = new GUIStyleState() { background = icnLockOnActive },
            };

            // Unselected just makes the normal states have no lock images
            var tempStyle = GUI.skin.FindStyle("OL Toggle");
            styleLockUnselected = new GUIStyle(styleLock)
            {
                normal = tempStyle.normal,
                active = tempStyle.active,
                hover = tempStyle.hover,
                focused = tempStyle.focused
            };

	        tempStyle = GUI.skin.FindStyle("VisibilityToggle");
			styleVisOff = new GUIStyle(tempStyle);
			
            styleVisOn = new GUIStyle(tempStyle)
            {
				normal = new GUIStyleState() { background = tempStyle.onNormal.background }
            };

        }
    }
}