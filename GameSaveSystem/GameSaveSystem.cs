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
        private static GameSaveSystemSettings _settings;
        private static string _fileSaveLocation;

        /// <summary>
        /// Gets a value indicating whether or not the GameSaveSystem has been initialized.
        /// </summary>
        public static bool isInitialized { get; private set; }

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
        /// Initializes the system with the provided settings.
        /// </summary>
        /// <param name="settings">The settings to configure the system with.</param>
        public static void Initialize(GameSaveSystemSettings settings)
        {
            if (isInitialized) {
                throw new InvalidOperationException("GameSaveSystem is already initialized. Cannot initialize again!");
            }

            // Validate our input
            if (settings == null) {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(settings.gameName)) {
                throw new ArgumentException("The gameName in the settings must be a non-empty string");
            }
            if (settings.gameName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1) {
                throw new ArgumentException("The gameName in the settings contains illegal characters");
            }
            if (settings.useRollingBackups && settings.backupCount <= 0) {
                throw new ArgumentException("useRollingBackups is true but backupCount isn't a positive value");
            }

            // Copy the settings locally so we can retain a new instance (so it can't be changed out from under us)
            _settings = settings.Clone();

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
            if (!string.IsNullOrEmpty(_settings.companyName)) {
                _fileSaveLocation = Path.Combine(_fileSaveLocation, _settings.companyName);
            }

            _fileSaveLocation = Path.Combine(_fileSaveLocation, _settings.gameName);
            _fileSaveLocation = Path.GetFullPath(_fileSaveLocation);
#endif

            // Ensure the directory for saves exists.
            Directory.CreateDirectory(_fileSaveLocation);

            isInitialized = true;
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

                    if (_settings.useRollingBackups) {
                        usedBackup = DoLoadWithBackups(save, name);
                    }
                    else {
                        using (var stream = File.OpenRead(GetGameSavePath(name))) {
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
                if (_settings.useRollingBackups) {
                    DoSaveWithBackups(save, name);
                }
                else {
                    using (var stream = File.Create(GetGameSavePath(name))) {
                        save.Save(stream);
                    }
                }
                return true;
            });
        }

        private static bool DoLoadWithBackups(IGameSave save, string name)
        {
            string mainSavePath = GetGameSavePath(name);

            // Try loading the regular save. Most times this should work fine.
            try {
                using (var stream = File.OpenRead(mainSavePath)) {
                    save.Load(stream);
                }

                // If Load didn't throw, we're good to go and can return that we
                // didn't need to load a backup save.
                return false;
            }
            catch {
                save.Reset();
            }

            // We go through and try loading all of the available backups
            var foundGoodSave = false;
            var backupIndex = 0;
            for (backupIndex = 0; !foundGoodSave && backupIndex < _settings.backupCount; backupIndex++) {
                // Try loading the file.
                try {
                    var path = GetBackupSavePath(name, backupIndex);
                    using (var stream = File.OpenRead(path)) {
                        save.Load(stream);
                    }

                    foundGoodSave = true;

                    // break so we don't increment the backupIndex again
                    break;
                }
                catch {
                    save.Reset();
                }
            }

            // At this point we either know that A) we loaded a backup successfully or B) all the saves are bad.
            // So we need to clean up our saves to get rid of the bad ones.

            // We know the main save failed so we can delete that
            if (File.Exists(mainSavePath)) {
                File.Delete(mainSavePath);
            }

            // Delete all backups newer than the one we were able to load. This might delete all of them if no saves were good.
            for (int i = backupIndex - 1; i >= 0; i--) {
                var path = GetBackupSavePath(name, i);
                if (File.Exists(path)) {
                    File.Delete(path);
                }
            }

            if (foundGoodSave) {
                // If we did find a save, make that save our main save
                Debug.Log("Moving backup " + GetBackupSavePath(name, backupIndex) + " to " + mainSavePath);
                MoveFile(GetBackupSavePath(name, backupIndex), mainSavePath);

                // Move up the remaining backups
                for (int i = backupIndex; i <= _settings.backupCount; i++) {
                    var path1 = GetBackupSavePath(name, i);
                    if (File.Exists(path1)) {
                        File.Delete(path1);
                    }

                    if (i < _settings.backupCount) {
                        var path2 = GetBackupSavePath(name, i + 1);
                        if (File.Exists(path2)) {
                            MoveFile(path2, path1);
                        }
                    }
                }
            }

            // If we didn't find any good saves, throw an exception so the future will receive the error and
            // games can choose how to handle it.
            if (!foundGoodSave) {
                throw new FileNotFoundException("No game save found with name '" + name + "'");
            }

            // Return true because if we got here we know we used a backup file
            return true;
        }

        private static void MoveFile(string src, string dst)
        {
#if UNITY_WEBPLAYER
            File.Copy(src, dst);
            File.Delete(src);
#else
            File.Move(src, dst);
#endif
        }

        private static void DoSaveWithBackups(IGameSave save, string name)
        {
            var mainSavePath = GetGameSavePath(name);
            var tempPath = mainSavePath + ".temp";

            // Start by attempting the save to a temp file
            try {
                using (var stream = File.Create(tempPath)) {
                    save.Save(stream);
                }
            }
            catch {
                // Remove the temp file before leaving scope if saving threw an exception
                try {
                    if (File.Exists(tempPath)) {
                        File.Delete(tempPath);
                    }
                }
                catch { }

                // Let the exception continue up
                throw;
            }

            // Saving succeeded so we need to move from the temp path to the main path.
            // First up we shift down all the backup files
            for (int i = _settings.backupCount - 2; i >= 0; i--) {
                var path = GetBackupSavePath(name, i);
                if (File.Exists(path)) {
                    var nextPath = GetBackupSavePath(name, i + 1);
                    if (File.Exists(nextPath)) {
                        File.Delete(nextPath);
                    }
                    MoveFile(path, nextPath);
                }
            }

            // Then move the current main save into the first backup slot
            if (File.Exists(mainSavePath)) {
                MoveFile(mainSavePath, GetBackupSavePath(name, 0));
            }

            // Then we can just move the new file into place
            MoveFile(tempPath, mainSavePath);
        }

        private static void ThrowIfNotInitialized()
        {
            if (!isInitialized) {
                throw new InvalidOperationException("You must call Initialize first!");
            }
        }

        private static string GetGameSavePath(string name)
        {
            return Path.Combine(_fileSaveLocation, name + ".save");
        }

        private static string GetBackupSavePath(string name, int index)
        {
            return GetGameSavePath(name) + ".backup" + (index + 1);
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
