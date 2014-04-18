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

using System.IO;

namespace UnityToolbag
{
    /// <summary>
    /// Defines the interface of a game save object that can be used with <see cref="GameSaveSystem"/>.
    /// </summary>
    public interface IGameSave
    {
        /// <summary>
        /// Resets the game save to a blank state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Saves the game state into the given stream.
        /// </summary>
        /// <param name="stream">The stream into which the game save should be saved.</param>
        void Save(Stream stream);

        /// <summary>
        /// Loads the game state from the given stream.
        /// </summary>
        /// <param name="stream">The stream from which the game save should be loaded.</param>
        void Load(Stream stream);
    }
}
