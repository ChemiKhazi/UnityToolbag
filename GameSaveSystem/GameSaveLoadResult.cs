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
