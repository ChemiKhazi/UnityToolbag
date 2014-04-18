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

using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityToolbag
{
    /// <summary>
    /// Provides an editor window for quickly compiling and running snippets of code.
    /// </summary>
    public class ImmediateWindow : EditorWindow
    {
        private const string EditorPrefsKey = "UnityToolbag.ImmediateWindow.LastText";

        // Positions for the two scroll views
        private Vector2 _scrollPos;
        private Vector2 _errorScrollPos;

        // The script text string
        private string _scriptText = string.Empty;

        // Stored away compiler errors (if any) and the compiled method
        private CompilerErrorCollection _compilerErrors = null;
        private MethodInfo _compiledMethod = null;

        void OnEnable()
        {
            if (EditorPrefs.HasKey(EditorPrefsKey)) {
                _scriptText = EditorPrefs.GetString(EditorPrefsKey);
            }
        }

        void OnGUI()
        {
            // Make a scroll view for the text area
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // Place a text area in the scroll view
            string newScriptText = EditorGUILayout.TextArea(_scriptText, GUILayout.ExpandHeight(true));

            // If the script updated save the script and remove the compiled method as it's no longer valid
            if (_scriptText != newScriptText) {
                _scriptText = newScriptText;
                EditorPrefs.SetString(EditorPrefsKey, _scriptText);
                _compiledMethod = null;
            }

            EditorGUILayout.EndScrollView();

            // Setup the compile/run button
            if (GUILayout.Button(_compiledMethod == null ? "Compile + Run" : "Run")) {
                // If the method is already compiled or if we successfully compile the script text, invoke the method
                if (_compiledMethod != null || CodeCompiler.CompileCSharpImmediateSnippet(_scriptText, out _compilerErrors, out _compiledMethod)) {
                    _compiledMethod.Invoke(null, null);
                }
            }

            // If we have any errors, we display them in their own scroll view
            if (_compilerErrors != null && _compilerErrors.Count > 0) {
                // Build up one string for errors and one for warnings
                StringBuilder errorString = new StringBuilder();
                StringBuilder warningString = new StringBuilder();

                foreach (CompilerError e in _compilerErrors) {
                    if (e.IsWarning) {
                        warningString.AppendFormat("Warning on line {0}: {1}\n", e.Line, e.ErrorText);
                    }
                    else {
                        errorString.AppendFormat("Error on line {0}: {1}\n", e.Line, e.ErrorText);
                    }
                }

                // Remove trailing new lines from both strings
                if (errorString.Length > 0) {
                    errorString.Length -= 2;
                }

                if (warningString.Length > 0) {
                    warningString.Length -= 2;
                }

                // Make a simple UI layout with a scroll view and some labels
                GUILayout.Label("Errors and warnings:");
                _errorScrollPos = EditorGUILayout.BeginScrollView(_errorScrollPos, GUILayout.MaxHeight(100));

                if (errorString.Length > 0) {
                    GUILayout.Label(errorString.ToString());
                }

                if (warningString.Length > 0) {
                    GUILayout.Label(warningString.ToString());
                }

                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// Fired when the user chooses the menu item
        /// </summary>
        [MenuItem("Window/Immediate %#I")]
        static void Init()
        {
            // Get the window, show it, and give it focus
            var window = EditorWindow.GetWindow<ImmediateWindow>("Immediate");
            window.Show();
            window.Focus();
        }
    }
}
