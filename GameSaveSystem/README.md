Game Save System
===

This is a game save management system I wrote for our games. This system solves a few fundamental problems when building a game save system.

1. All saving and loading occurs off the main thread using futures. This ensures games are responsive and don't lock up during file operations.
2. File saves are cached so trying to load a save that is in the cache doesn't hit the disk (unless you pass in an argument to force it to).
3. The system stores backups of game saves (4 backups + the good save) for cases where loading fails. This provides a nice fallback system in cases where game saves become corrupt.
4. Unity's `Application.persistentDataPath` is almost always not where game saves should go. This game save system uses much nicer paths for game saves.

Using the game save system is straightforward:

1. Create one or more classes that implement `IGameSave`.
2. At the start of your game, call `GameSaveSystem.Initialize(string companyName, string gameName)` to provide the system with your company and game name, which are both used to construct the game save directory (which can be accessed via the `GameSaveSystem.saveLocation` property).
3. Use `GameSaveSystem.Load` to load saves and `GameSaveSystem.Save` to save them.

All of the code files are heavily commented and should be referenced for more documentation. Additionally there is a Demo folder that has an extremely minimal sample showing the usage of the system.

This system relies on both the `IFuture<T>` interface and `Future<T>` class from the [Future](https://github.com/nickgravelyn/UnityToolbag/tree/master/Future) library, so make sure you include them in your project if you want to use this sytem.
