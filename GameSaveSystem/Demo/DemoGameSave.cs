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
using System.IO;
using System.Threading;

namespace UnityToolbag
{
    // Just a basic implementation of IGameSave
    public class DemoGameSave : IGameSave
    {
        // Helper variables to test failing when saving and loading
        public static bool FailNextSave = false;
        public static bool FailNextLoad = false;

        public string text = "Hello, world!";

        public void Reset()
        {
            text = string.Empty;
        }

        public void Save(Stream stream)
        {
            if (FailNextSave) {
                FailNextSave = false;
                throw new InvalidOperationException("Intentionally failing the save.");
            }

            using (var writer = new StreamWriter(stream)) {
                // Write the date into the file just to prove the backup system is working as intended.
                writer.WriteLine(DateTime.Now.ToString());
                writer.WriteLine(text);
            }

            // Pretend this is taking a long time ;)
            Thread.Sleep(1000);
        }

        public void Load(Stream stream)
        {
            if (FailNextLoad) {
                FailNextLoad = false;
                throw new InvalidOperationException("Intentionally failing the load.");
            }

            using (var reader = new StreamReader(stream)) {
                // Read the line containing the date; we don't really need it, though.
                reader.ReadLine();
                text = reader.ReadLine();
            }

            // Pretend this is taking a long time ;)
            Thread.Sleep(1000);
        }
    }
}
