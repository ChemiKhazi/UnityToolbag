using System;
using System.IO;
using UnityEngine;

namespace UnityToolbag
{
    public static class StandardPaths
    {
        public static bool includeCompanyName { get; set; }

        public static string saveDataDirectory
        {
            get
            {
                string path;

                switch (Application.platform) {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                    {
                        path = Windows.GetPath("Saves");
                        break;
                    }
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                    {
                        path = OSX.GetApplicationSupportPath("Saves");
                        break;
                    }
                    case RuntimePlatform.LinuxPlayer:
                    {
                        path = Linux.GetSaveDirectory();
                        break;
                    }
                    default:
                    {
                        path = Application.persistentDataPath;
                        break;
                    }
                }

                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string configDirectory
        {
            get
            {
                string path;

                switch (Application.platform) {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                    {
                        path = Windows.GetPath("Config");
                        break;
                    }
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                    {
                        path = OSX.GetApplicationSupportPath("Config");
                        break;
                    }
                    case RuntimePlatform.LinuxPlayer:
                    {
                        path = Linux.GetConfigDirectory();
                        break;
                    }
                    default:
                    {
                        path = Application.persistentDataPath;
                        break;
                    }
                }

                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string logDirectory
        {
            get
            {
                string path;

                switch (Application.platform) {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                    {
                        path = Windows.GetPath("Logs");
                        break;
                    }
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                    {
                        path = OSX.GetLogsPath();
                        break;
                    }
                    case RuntimePlatform.LinuxPlayer:
                    {
                        path = Linux.GetLogDirectory();
                        break;
                    }
                    default:
                    {
                        path = Application.persistentDataPath;
                        break;
                    }
                }

                Directory.CreateDirectory(path);
                return path;
            }
        }

        static StandardPaths()
        {
            includeCompanyName = false;
        }

        private static string AppendProductPath(string path)
        {
            if (includeCompanyName) {
                path = AppendDirectory(path, Application.companyName);
            }

            return AppendDirectory(path, Application.productName);
        }

        private static string AppendDirectory(string path, string dir)
        {
            if (string.IsNullOrEmpty(dir)) {
                return path;
            }

            dir = CleanForPath(dir);
            return Path.Combine(path, dir);
        }

        private static string CleanForPath(string str)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var ch in invalidChars) {
                str = str.Replace(ch, '_');
            }

            return str;
        }

        private static class Windows
        {
            public static string GetPath(string subdirectory)
            {
                var result = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "My Games");

                result = AppendProductPath(result);
                return AppendDirectory(result, subdirectory);
            }
        }

        private static class OSX
        {
            public static string GetApplicationSupportPath(string subdirectory)
            {
                var result = Path.Combine(
                    Environment.GetEnvironmentVariable("HOME"),
                    "Library/Application Support");

                result = AppendProductPath(result);
                return AppendDirectory(result, subdirectory);
            }

            public static string GetLogsPath()
            {
                var result = Path.Combine(
                    Environment.GetEnvironmentVariable("HOME"),
                    "Library/Logs");

                return AppendProductPath(result);
            }
        }

        private static class Linux
        {
            public static string GetSaveDirectory()
            {
                string result = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                if (string.IsNullOrEmpty(result)) {
                    string home = Environment.GetEnvironmentVariable("HOME");
                    result = Path.Combine(home, ".local/share");
                }

                return AppendProductPath(result);
            }

            public static string GetConfigDirectory()
            {
                string result = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
                if (string.IsNullOrEmpty(result)) {
                    string home = Environment.GetEnvironmentVariable("HOME");
                    result = Path.Combine(home, ".config");
                }

                return AppendProductPath(result);
            }

            public static string GetLogDirectory()
            {
                return AppendProductPath("/var/log");
            }
        }
    }
}
