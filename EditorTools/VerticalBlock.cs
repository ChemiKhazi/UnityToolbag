using UnityEngine;
using UnityEditor;
using System;

namespace UnityToolbag
{
    public class VerticalBlock : IDisposable
    {
        public VerticalBlock(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
        }

        public VerticalBlock(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(style, options);
        }

        public void Dispose()
        {
            GUILayout.EndVertical();
        }
    }
}
