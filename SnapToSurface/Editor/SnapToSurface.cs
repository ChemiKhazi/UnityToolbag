using UnityEngine;
using UnityEditor;

namespace UnityToolbag
{
    public class SnapToSurface : EditorWindow
    {
        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X")) {
                Drop(new Vector3(1, 0, 0));
            }
            if (GUILayout.Button("Y")) {
                Drop(new Vector3(0, 1, 0));
            }
            if (GUILayout.Button("Z")) {
                Drop(new Vector3(0, 0, 1));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-X")) {
                Drop(new Vector3(-1, 0, 0));
            }
            if (GUILayout.Button("-Y")) {
                Drop(new Vector3(0, -1, 0));
            }
            if (GUILayout.Button("-Z")) {
                Drop(new Vector3(0, 0, -1));
            }
            EditorGUILayout.EndHorizontal();
        }

        [MenuItem("Window/Snap to Surface")]
        static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<SnapToSurface>();
            window.title = "Snap To Surface";
        }

        static void Drop(Vector3 dir)
        {
            foreach (GameObject go in Selection.gameObjects) {
                // If the object has a collider we can do a nice sweep test for accurate placement
                if (go.GetComponent<Collider>() != null && !(go.GetComponent<Collider>() is CharacterController)) {
                    // Figure out if we need a temp rigid body and add it if needed
                    bool addedRigidBody = false;
                    if (go.GetComponent<Rigidbody>() == null) {
                        go.AddComponent<Rigidbody>();
                        addedRigidBody = true;
                    }

                    // Sweep the rigid body downwards and, if we hit something, move the object the distance
                    RaycastHit hit;
                    if (go.GetComponent<Rigidbody>().SweepTest(dir, out hit)) {
                        go.transform.position += dir * hit.distance;
                    }

                    // If we added a rigid body for this test, remove it now
                    if (addedRigidBody) {
                        DestroyImmediate(go.GetComponent<Rigidbody>());
                    }
                }
                // Without a collider, we do a simple raycast from the transform
                else {
                    // Change the object to the "ignore raycast" layer so it doesn't get hit
                    int savedLayer = go.layer;
                    go.layer = 2;

                    // Do the raycast and move the object down if it hit something
                    RaycastHit hit;
                    if (Physics.Raycast(go.transform.position, dir, out hit)) {
                        go.transform.position = hit.point;
                    }

                    // Restore layer for the object
                    go.layer = savedLayer;
                }
            }
        }
    }
}
