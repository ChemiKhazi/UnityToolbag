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

using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

namespace UnityToolbag
{
    /// <summary>
    /// Provides a simple interface for dynamically compiling C# code.
    /// </summary>
    public static class CodeCompiler
    {
        /// <summary>
        /// Compiles a method body of C# script, wrapped in a basic void-returning method.
        /// </summary>
        /// <param name="methodText">The text of the script to place inside a method.</param>
        /// <param name="errors">The compiler errors and warnings from compilation.</param>
        /// <param name="methodIfSucceeded">The compiled method if compilation succeeded.</param>
        /// <returns>True if compilation was a success, false otherwise.</returns>
        public static bool CompileCSharpImmediateSnippet(string methodText, out CompilerErrorCollection errors, out MethodInfo methodIfSucceeded)
        {
            // Wrapper text so we can compile a full type when given just the body of a method
            string methodScriptWrapper = @"
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
public static class CodeSnippetWrapper
{{
    public static void PerformAction()
    {{
        {0};
    }}
}}";

            // Default method to null
            methodIfSucceeded = null;

            // Compile the full script
            Assembly assembly;
            if (CompileCSharpScript(string.Format(methodScriptWrapper, methodText), out errors, out assembly)) {
                // If compilation succeeded, we can use reflection to get the method and pass that back to the user
                methodIfSucceeded = assembly.GetType("CodeSnippetWrapper").GetMethod("PerformAction", BindingFlags.Static | BindingFlags.Public);
                return true;
            }

            // Compilation failed, caller has the errors, return false
            return false;
        }

        /// <summary>
        /// Compiles a C# script as if it were a file in your project.
        /// </summary>
        /// <param name="scriptText">The text of the script.</param>
        /// <param name="errors">The compiler errors and warnings from compilation.</param>
        /// <param name="assemblyIfSucceeded">The compiled assembly if compilation succeeded.</param>
        /// <returns>True if compilation was a success, false otherwise.</returns>
        public static bool CompileCSharpScript(string scriptText, out CompilerErrorCollection errors, out Assembly assemblyIfSucceeded)
        {
            var codeProvider = new CSharpCodeProvider();
            var compilerOptions = new CompilerParameters();

            // We want a DLL and we want it in memory
            compilerOptions.GenerateExecutable = false;
            compilerOptions.GenerateInMemory = true;

            // Add references for UnityEngine and UnityEditor DLLs
            compilerOptions.ReferencedAssemblies.Add(typeof(Vector2).Assembly.Location);
            compilerOptions.ReferencedAssemblies.Add(typeof(EditorApplication).Assembly.Location);

            // Default to null output parameters
            errors = null;
            assemblyIfSucceeded = null;

            // Compile the assembly from the source script text
            CompilerResults result = codeProvider.CompileAssemblyFromSource(compilerOptions, scriptText);

            // Store the errors for the caller. even on successful compilation, we may have warnings.
            errors = result.Errors;

            // See if any errors are actually errors. if so return false
            foreach (CompilerError e in errors) {
                if (!e.IsWarning) {
                    return false;
                }
            }

            // Otherwise we pass back the compiled assembly and return true
            assemblyIfSucceeded = result.CompiledAssembly;
            return true;
        }
    }
}
