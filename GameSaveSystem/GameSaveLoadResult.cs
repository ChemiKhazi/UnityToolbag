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

namespace UnityToolbag
{
    /// <summary>
    /// The actual object returned by the <see cref="GameSaveSystem" /> when loading a game save
    /// in order to provide additional details about the result.
    /// </summary>
    /// <typeparam name="TGameSave">The game save class type.</typeparam>
    public class GameSaveLoadResult<TGameSave>
        where TGameSave : class, IGameSave, new()
    {
        /// <summary>
        /// Gets the actual game save object.
        /// </summary>
        public TGameSave save { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the game save was returned from a cache.
        /// </summary>
        public bool wasCached { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not a backup file was loaded.
        /// </summary>
        /// <remarks>
        /// While game logic shouldn't do much with this information, it can be nice to notify the
        /// player that the game couldn't load their main game save and instead loaded a backup.
        /// </remarks>
        public bool usedBackupFile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the  <see cref="GameSaveLoadResult{TGameSave}" /> class.
        /// </summary>
        /// <param name="save">The game save object.</param>
        /// <param name="wasCached">Whether or not the game save was returned from a cache.</param>
        /// <param name="usedBackupFile">Whether or not a backup file was loaded for the save.</param>
        public GameSaveLoadResult(TGameSave save, bool wasCached, bool usedBackupFile)
        {
            this.save = save;
            this.wasCached = wasCached;
            this.usedBackupFile = usedBackupFile;
        }
    }
}
