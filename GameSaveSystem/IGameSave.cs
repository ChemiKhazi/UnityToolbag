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
