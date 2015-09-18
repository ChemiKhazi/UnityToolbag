﻿using UnityEngine;
using UnityEditor;
using System;

namespace UnityToolbag
{
    public class ScrollViewBlock : IDisposable
    {
        public ScrollViewBlock(ref Vector2 scrollPosition, params GUILayoutOption[] options)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }
    }
}
