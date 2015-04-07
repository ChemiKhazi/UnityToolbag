using UnityEditor;
using System;

namespace UnityToolbag
{
    public class IndentBlock : IDisposable
    {
        public IndentBlock()
        {
            EditorGUI.indentLevel++;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel--;
        }
    }
}
