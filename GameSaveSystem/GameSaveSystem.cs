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
using System;
using System.Collections.Generic;
using System.IO;

namespace UnityToolbag
{
    /// <summary>
    /// A game save system that handles file management and save file caching.
    /// </summary>
    public static class GameSaveSystem
    {
        // If _useRollingBackups is true, this is how many save files are stored for each named file.
        private const int MaxSavesPerSaveName = 5;

        private static bool _isInitialized;
        private static string _fileSaveLocation;
        private static bool _useRollingBackups;

        /// <summary>
        /// Gets the folder where the game saves are stored;
        /// </summary>
        public static string saveLocation
        {
            get
            {
                ThrowIfNotInitialized();
                return _fileSaveLocation;
            }
        }

        /// <summary>
        /// Initializes the system with a given company and game.
        /// </summary>
        /// <param name="companyName">The name of the company. Must be safe for use as a directory name.</param>
        /// <param name="gameName">The name of the game. Must be safe for use as a directory name.</param>
        /// <param name="useRollingBackups"><c>true</c> to use the rolling backup system, false to only store one version of a save file.</param>
        public static void Initialize(string companyName, string gameName, bool useRollingBackups = true)
        {
            // We only require the game name; the company can be omitted if that's preferred for creating the save location.
            if (string.IsNullOrEmpty(gameName)) {
                throw new ArgumentException("gameName must be a non-empty string!");
            }

            if (_isInitialized) {
                return;
            }

            // Find the base path for where we want to save game saves. Unity's persistentDataPath is generally unacceptable
            // to me. In Windows it uses some AppData folder, in OS X it (quite incorrectly) puts saves into a Cache folder,
            // and I have no idea what they use on Linux but I'm assuming it's not what I want.
#if UNITY_STANDALONE_WIN
            _fileSaveLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games");
#elif UNITY_STANDALONE_OSX
            _fileSaveLocation = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library/Application Support");
#elif UNITY_STANDALONE_LINUX
            string home = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
            if (string.IsNullOrEmpty(home)) {
                home = Environment.GetEnvironmentVariable("HOME");
            }
            _fileSaveLocation = Path.Combine(home, ".local/share");
#else
            // For any non Mac/Windows/Linux platform, we'll default back to the persistentDataPath since we know it should work.
            _fileSaveLocation = Application.persistentDataPath;
#endif

            // Do final cleanup on the path if we're on a desktop platform
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            // Company name is optional so we check before appending it
            if (!string.IsNullOrEmpty(companyName)) {
                _fileSaveLocation = Path.Combine(_fileSaveLocation, companyName);
            }

            _fileSaveLocation = Path.Combine(_fileSaveLocation, gameName);
            _fileSaveLocation = Path.GetFullPath(_fileSaveLocation);
#endif

            // Ensure the directory for saves exists.
            Directory.CreateDirectory(_fileSaveLocation);

            _useRollingBackups = useRollingBackups;
            _isInitialized = true;
        }

        /// <summary>
        /// Loads a game save.
        /// </summary>
        /// <typeparam name="TGameSave">The game save class type.</typeparam>
        /// <param name="name">The name of the gamesave to load.</param>
        /// <param name="forceFromDisk">
        /// If <c>true</c> the save will always be read from disk. If <c>false</c> the system will return a cached instance if available.
        /// </param>
        /// <returns>An <see cref="IFuture<GameSaveLoadResult<TGameSave>>" /> that can be used to track and examine the load results.</returns>
        public static IFuture<GameSaveLoadResult<TGameSave>> Load<TGameSave>(string name, bool forceFromDisk = false)
            where TGameSave : class, IGameSave, new()
        {
            ThrowIfNotInitialized();

            Future<GameSaveLoadResult<TGameSave>> future = new Future<GameSaveLoadResult<TGameSave>>();

            TGameSave save = null;

            if (GameSaveCache<TGameSave>.TryGetSave(name, out save) && !forceFromDisk) {
                future.Assign(new GameSaveLoadResult<TGameSave>(save, true, false));
            }
            else {
                if (save == null) {
                    save = new TGameSave();
                    GameSaveCache<TGameSave>.Set(name, save);
                }
                else {
                    save.Reset();
                }

                future.Process(() =>
                {
                    bool usedBackup = false;

                    if (_useRollingBackups) {
                        usedBackup = DoLoadWithBackups(save, name);
                    }
                    else {
                        // For compatibility, we append 0 in case the game changes policy on using rolling backups.
                        // This will ensure the game can go from using them to not, or vice versa.
                        using (var stream = File.OpenRead(GetGameSavePath(name) + "0")) {
                            save.Load(stream);
                        }
                    }

                    return new GameSaveLoadResult<TGameSave>(save, false, usedBackup);
                });
            }

            return future;
        }

        /// <summary>
        /// Saves a game save.
        /// </summary>
        /// <typeparam name="TGameSave">The game save class type.</typeparam>
        /// <param name="name">The name of the gamesave to load.</param>
        /// <param name="save">The game save to save.</param>
        /// <returns>An <see cref="IFuture{bool}"/> that can be used to track completion of the save operation.</returns>
        public static IFuture<bool> Save<TGameSave>(string name, TGameSave save)
            where TGameSave : class, IGameSave, new()
        {
            ThrowIfNotInitialized();

            GameSaveCache<TGameSave>.Set(name, save);

            return new Future<bool>().Process(() =>
            {
                if (_useRollingBackups) {
                    DoSaveWithBackups(save, name);
                }
                else {
                    // For compatibility, we append 0 in case the game changes policy on using rolling backups.
                    // This will ensure the game can go from using them to not, or vice versa.
                    using (var stream = File.Create(GetGameSavePath(name) + "0")) {
                        save.Save(stream);
                    }
                }
                return true;
            });
        }

        private static bool DoLoadWithBackups(IGameSave save, string name)
        {
            // Get our base path just once
            var basePath = GetGameSavePath(name);

            // We go through and try loading all of the available game saves
            var foundGoodSave = false;
            var saveIndex = 0;
            for (saveIndex = 0; !foundGoodSave && saveIndex < MaxSavesPerSaveName; saveIndex++) {
                var savePath = basePath + saveIndex;

                // Try loading the file.
                try {
                    using (var stream = File.OpenRead(savePath)) {
                        save.Load(stream);
                    }
                    foundGoodSave = true;

                    // break so we don't increment the saveIndex again
                    break;
                }
                catch (Exception e) {
                    Debug.LogWarning(e.ToString());
                    save.Reset();
                }
            }

            // If used a backup file (or failed to load any file), delete the newer files and rename the others so everything's looking good
            if (foundGoodSave && saveIndex > 0) {
                // Delete all the newer saves
                for (int i = 0; i < saveIndex; i++) {
                    var path = basePath + i;
                    try {
                        if (File.Exists(path)) {
                            File.Delete(path);
                        }
                    }
                    catch (Exception e) {
                        Debug.LogError(e.ToString());
                    }
                }

                // Shuffle down the remaining saves to fill in for us
                for (int i = saveIndex; i < MaxSavesPerSaveName; i++) {
                    try {
                        var path1 = basePath + i;
                        var path2 = basePath + (i - saveIndex);

                        if (File.Exists(path1) && File.Exists(path2)) {
                            File.Delete(path1);
                            File.Copy(path2, path1);
                        }
                    }
                    catch (Exception e) {
                        Debug.LogError(e.ToString());
                    }
                }
            }

            // If we didn't find any good saves, throw an exception so the future will receive the error and
            // games can choose how to handle it.
            if (!foundGoodSave) {
                throw new FileNotFoundException("No game save found with name '" + name + "'");
            }

            // Return true if we used a backup file
            return saveIndex > 0;
        }

        private static void DoSaveWithBackups(IGameSave save, string name)
        {
            var basePath = GetGameSavePath(name);
            var tempPath = basePath + ".temp";
            var finalPath = basePath + "0";

            // Start by attempting the save to a temp file
            using (var stream = File.Create(tempPath)) {
                save.Save(stream);
            }

            // Saving succeeded so we need to move from the temp path to save0.
            // First we need to move all existing save files down a slot.
            for (int i = MaxSavesPerSaveName - 2; i >= 0; i--) {
                var path = basePath + i;
                if (File.Exists(path)) {
                    var nextPath = basePath + (i + 1);
                    if (File.Exists(nextPath)) {
                        File.Delete(nextPath);
                    }
                    File.Move(path, nextPath);
                }
            }

            // Then we can just move the new file into place
            if (File.Exists(finalPath)) {
                File.Delete(finalPath);
            }
            File.Move(tempPath, finalPath);
        }

        private static void ThrowIfNotInitialized()
        {
            if (!_isInitialized) {
                throw new InvalidOperationException("You must call Initialize first!");
            }
        }

        private static string GetGameSavePath(string name)
        {
            return Path.Combine(_fileSaveLocation, name + ".save");
        }

        // Static generic class for caching individual types of game saves.
        private static class GameSaveCache<TGameSave>
            where TGameSave : class, IGameSave, new()
        {
            private static readonly Dictionary<string, TGameSave> _cache = new Dictionary<string, TGameSave>();

            public static bool TryGetSave(string name, out TGameSave save)
            {
                return _cache.TryGetValue(name, out save);
            }

            public static void Set(string name, TGameSave save)
            {
                _cache[name] = save;
            }
        }
    }
}
