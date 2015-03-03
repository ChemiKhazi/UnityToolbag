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
