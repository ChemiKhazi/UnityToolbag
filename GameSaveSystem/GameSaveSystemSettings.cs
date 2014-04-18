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
