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
using System.IO;

namespace UnityToolbag
{
    public static class UnityConstantsGenerator
    {
        [MenuItem("Edit/Generate UnityConstants.cs")]
        public static void Generate()
        {
            // Try to find an existing file in the project called "UnityConstants.cs"
            string filePath = string.Empty;
            foreach (var file in Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories)) {
                if (Path.GetFileNameWithoutExtension(file) == "UnityConstants") {
                    filePath = file;
                    break;
                }
            }

            // If no such file exists already, use the save panel to get a folder in which the file will be placed.
            if (string.IsNullOrEmpty(filePath)) {
                string directory = EditorUtility.OpenFolderPanel("Choose location for UnityConstants.cs", Application.dataPath, "");

                // Canceled choose? Do nothing.
                if (string.IsNullOrEmpty(directory)) {
                    return;
                }

                filePath = Path.Combine(directory, "UnityConstants.cs");
            }

            // Write out our file
            using (var writer = new StreamWriter(filePath)) {
                writer.WriteLine("// This file is auto-generated. Modifications are not saved.");
                writer.WriteLine();
                writer.WriteLine("namespace UnityConstants");
                writer.WriteLine("{");

                // Write out the tags
                writer.WriteLine("    public static class Tags");
                writer.WriteLine("    {");
                foreach (var tag in UnityEditorInternal.InternalEditorUtility.tags) {
                    writer.WriteLine("        public const string {0} = \"{1}\";", MakeSafeForCode(tag), tag);
                }
                writer.WriteLine("    }");
                writer.WriteLine();

                // Write out sorting layers
                var sortingLayerNames = SortingLayerHelper.sortingLayerNames;
                if (sortingLayerNames != null) {
                    writer.WriteLine("    public static class SortingLayers");
                    writer.WriteLine("    {");
                    for (int i = 0; i < sortingLayerNames.Length; i++) {
                        var name = sortingLayerNames[i];
                        int id = SortingLayerHelper.GetSortingLayerIDForName(name);
                        writer.WriteLine("        public const int {0} = {1};", MakeSafeForCode(name), id);
                    }
                    writer.WriteLine("    }");
                    writer.WriteLine();
                }

                // Write out layers
                writer.WriteLine("    public static class Layers");
                writer.WriteLine("    {");
                writer.WriteLine("        // Regular layer indices for assigning layers dynamically in code");
                for (int i = 0; i < 32; i++) {
                    string layer = UnityEditorInternal.InternalEditorUtility.GetLayerName(i);
                    if (!string.IsNullOrEmpty(layer)) {
                        writer.WriteLine("        public const int {0} = {1};", MakeSafeForCode(layer), i);
                    }
                }
                writer.WriteLine();
                writer.WriteLine("        // Pre-configured layer masks for use with raycasts or other such queries");
                for (int i = 0; i < 32; i++) {
                    string layer = UnityEditorInternal.InternalEditorUtility.GetLayerName(i);
                    if (!string.IsNullOrEmpty(layer)) {
                        writer.WriteLine("        public const int {0}Mask = 1 << {0};", MakeSafeForCode(layer));
                    }
                }
                writer.WriteLine("    }");
                writer.WriteLine();

                // Write out scenes
                writer.WriteLine("    public static class Scenes");
                writer.WriteLine("    {");
                for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
                    writer.WriteLine(
                        "        public const int {0} = {1};",
                        MakeSafeForCode(Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path)),
                        i);
                }
                writer.WriteLine("    }");
                writer.WriteLine("}");
                writer.WriteLine();
            }

            // Refresh
            AssetDatabase.Refresh();
        }

        // Takes in a string and makes it safe for use a variable name in C#. This just means stripping out spaces and prefixing with a "_" character
        // if the string starts with a number. It's not the most robust, but should handle most cases just fine.
        private static string MakeSafeForCode(string str)
        {
            str = str.Replace(" ", "");
            if (char.IsDigit(str[0])) {
                str = "_" + str;
            }
            return str;
        }
    }
}
