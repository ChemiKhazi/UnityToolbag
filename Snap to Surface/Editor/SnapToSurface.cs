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

using UnityEngine;
using UnityEditor;

namespace UnityToolbag
{
    public class SnapToSurface : EditorWindow
    {
        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X"))
            {
                Drop(new Vector3(1, 0, 0));
            }
            if (GUILayout.Button("Y"))
            {
                Drop(new Vector3(0, 1, 0));
            }
            if (GUILayout.Button("Z"))
            {
                Drop(new Vector3(0, 0, 1));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-X"))
            {
                Drop(new Vector3(-1, 0, 0));
            }
            if (GUILayout.Button("-Y"))
            {
                Drop(new Vector3(0, -1, 0));
            }
            if (GUILayout.Button("-Z"))
            {
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
            foreach (GameObject go in Selection.gameObjects)
            {
                // If the object has a collider we can do a nice sweep test for accurate placement
                if (go.collider != null && !(go.collider is CharacterController))
                {
                    // Figure out if we need a temp rigid body and add it if needed
                    bool addedRigidBody = false;
                    if (go.rigidbody == null)
                    {
                        go.AddComponent<Rigidbody>();
                        addedRigidBody = true;
                    }

                    // Sweep the rigid body downwards and, if we hit something, move the object the distance
                    RaycastHit hit;
                    if (go.rigidbody.SweepTest(dir, out hit))
                    {
                        go.transform.position += dir * hit.distance;
                    }

                    // If we added a rigid body for this test, remove it now
                    if (addedRigidBody)
                    {
                        DestroyImmediate(go.rigidbody);
                    }
                }
                // Without a collider, we do a simple raycast from the transform
                else
                {
                    // Change the object to the "ignore raycast" layer so it doesn't get hit
                    int savedLayer = go.layer;
                    go.layer = 2;

                    // Do the raycast and move the object down if it hit something
                    RaycastHit hit;
                    if (Physics.Raycast(go.transform.position, dir, out hit))
                    {
                        go.transform.position = hit.point;
                    }

                    // Restore layer for the object
                    go.layer = savedLayer;
                }
            }
        }
    }
}
