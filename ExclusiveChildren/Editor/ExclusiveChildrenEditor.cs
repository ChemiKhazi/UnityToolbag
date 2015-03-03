using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace UnityToolbag
{
    [CustomEditor(typeof(ExclusiveChildren))]
    public class ExclusiveChildrenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Get the transform of the object and use that to get all children in alphabetical order
            var transform = (target as ExclusiveChildren).transform;
            var children = (transform as IEnumerable).Cast<Transform>().OrderBy(t => t.gameObject.name).ToArray();

            // Make a button for each child. Pressing the button enables that child and disables all others
            for (int i = 0; i < children.Length; i++) {
                var child = children[i];
                if (GUILayout.Button(child.gameObject.name)) {
                    foreach (var other in children) {
                        other.gameObject.SetActive(child == other);
                    }
                    break;
                }
            }
        }
    }
}
