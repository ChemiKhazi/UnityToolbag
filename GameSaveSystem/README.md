Game Save System
===

This is a game save management system I wrote for our games. This system implements a number of features that make dealing with game saves easier:

1. All saving and loading occurs off the main thread using futures. This ensures games are responsive and don't lock up during file operations.
2. File saves are cached so trying to load a save that is in the cache doesn't hit the disk (unless you pass in an argument to force it to).
3. The system can automatically handle rolling backups of game saves for cases where loading fails. This provides a nice fallback system in cases where game saves become corrupt. Both disabling the backups and setting the number of backup files is configurable with the `GameSaveSystemSettings` passed to `GameSaveSystem.Initialize`.
4. Unity's `Application.persistentDataPath` is almost always not where game saves should go. This game save system uses much nicer paths for game saves.

This system relies on both the `IFuture<T>` interface and `Future<T>` class from the [Future](https://github.com/nickgravelyn/UnityToolbag/tree/master/Future) library, so make sure you include them in your project if you want to use this sytem.

Usage
---

1. Create one or more classes that implement `IGameSave`.
2. At the start of your game, call `GameSaveSystem.Initialize(GameSaveSystemSetting settings)` to configure the system to your liking.
3. Use `GameSaveSystem.Load` to load saves and `GameSaveSystem.Save` to save them.

All of the code files are heavily commented and should be referenced for more documentation. Additionally there is a Demo folder that has an extremely minimal sample showing the usage of the system.

Save Locations
---

The game save system uses custom paths on desktop platforms to form more correct paths than Unity, whose `Application.persistentDataPath` is at best not ideal and at worst a terrible path for game saves.

If you'd prefer to not include your company name in the file paths below, simply keep the `companyName` field of the `GameSaveSystemSettings` object you pass to `GameSaveSystem.Initialize` as null. If the field is null, the `{Company}` parts of the paths below will be omitted.

### Windows

On Windows, the persistent data path will stick your game saves in `C:\Users\{User}\AppData\LocalLow\{Company}\{Game}`. Of all the paths this is the least bothersome, but the AppData directory itself is a hidden folder which can make it hard for gamers to back up game saves manually (most automatic backup software will pick this directory up). Instead this game system uses `C:\Users\{User}\Documents\My Games\{Company}\{Game}` which is pretty much what RockPaperShotgun [recommended](http://www.rockpapershotgun.com/2012/01/24/start-it-the-place-to-put-save-games/) for game saves.

### Mac OS X

On OS X Unity picks a terrible folder for the persistent data path, using `~/Library/Caches/{Company}/{Game}`. From Apple's [documentation](https://developer.apple.com/library/ios/documentation/FileManagement/Conceptual/FileSystemProgrammingGuide/MacOSXDirectories/MacOSXDirectories.html):

> Contains cached data that can be regenerated as needed. Apps should never rely on the existence of cache files.

Game saves definitely don't fit that definition.

Some games use `~/Library/Preferences` but that also isn't correct by Apple's guidelines:

> Contains the user’s preferences. You should never create files in this directory yourself. To get or set preference values, you should always use the NSUserDefaults class or an equivalent system-provided interface.

As such this system puts game saves where they should go, at `~/Library/Application Support/{Company}/{Game}`.

_Technically the Apple documentation recommends using the bundle identifier for your directory such as `~/Library/Application Support/com.example.MyApp/` but in reality nobody does that, not even Apple who put data files from the Mac App Store at `~/Library/Application Support/App Store`._

### Linux

_Caveat: I'm not a Linux user so I may not be quite right here, but from those I've spoken to, the path I'm using in this system is an acceptable location for game saves._

Linux is tricky since there doesn't appear to be one great standard. However Unity chose to put all of the game saves created in `~/.config/unity3d/{Company}/{Game}` which I think is a poor decision because now your users must know that your game uses Unity in order to find the game saves. To be more appropriate this system uses `($XDG_DATA_HOME|$HOME)/.local/share/{Company}/{Game}`. To find the root the system first resolves the environment variable `XDG_DATA_HOME`, falling back to regular `HOME` if that's not available.
