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
    /// A settings object used to initialize the GameSaveSystem.
    /// </summary>
    public class GameSaveSystemSettings
    {
        /// <summary>
        /// The name of the company used for constructing the save location. Can be null.
        /// </summary>
        public string companyName;

        /// <summary>
        /// The name of the game for constructing the save lcoation.
        /// </summary>
        public string gameName;

        /// <summary>
        /// Whether or not to use rolling backups for game saves.
        /// </summary>
        public bool useRollingBackups = true;

        /// <summary>
        /// Number of backups to store if useRollingBackups is <c>true</c>.
        /// </summary>
        public int backupCount = 2;

        /// <summary>
        /// Creates a copy of the settings object.
        /// </summary>
        public GameSaveSystemSettings Clone()
        {
            return new GameSaveSystemSettings
            {
                companyName = this.companyName,
                gameName = this.gameName,
                useRollingBackups = this.useRollingBackups,
                backupCount = this.backupCount,
            };
        }
    }
}
