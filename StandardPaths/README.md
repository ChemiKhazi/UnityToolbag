# StandardPaths

On desktop platforms, Unity's `persistentDataPath` doesn't always pick the best place for you to store files. `StandardPaths` is here to help.

`StandardPaths` is a static class containing paths for Unity games to use for writing out files. It currently includes these paths:

- `saveDataDirectory` is a directory path to use for user-specific save data.
- `configDirectory` is a directory path to use for user-specific settings/configuration.
- `logDirectory` is a directory path to use for custom log output.

These are split apart for a couple reasons:

1. Having logical names makes usage clearer, and allows for the underlying implementation to change without messing things up.
2. Different platforms have different standards around where files are stored that can't be followed with just a single path.

On all non-desktop platforms, these properties return `Application.persistentDataPath` as that's the most reliable solution. Other platforms are detailed below.

In the examples below the `{ProductPath}` means `{ProductName}` by default, but can also mean `{CompanyName}/{ProductName}` if you have set `StandardPaths.includeCompanyName` to true.

The company and product names are read from `Application.companyName` and `Application.productName`, respectively, and are processed such that all characters that are invalid for paths are converted to underscores.

On Linux, the `{ProductPath}` is converted to all lower-case to follow platform convention.

## Windows

`saveDataDirectory` maps to `C:\Users\{User}\Documents\My Games\{ProductPath}\Saves` as [recommended by RockPaperShotgun](http://www.rockpapershotgun.com/2012/01/24/start-it-the-place-to-put-save-games/).

`configDirectory` maps to `C:\Users\{User}\Documents\My Games\{ProductPath}\Config` because Windows doesn't have a separate location for configuration and this way users can easily find configuration files next to their save files.

`logDirectory` maps to `C:\Users\{User}\Documents\My Games\{ProductPath}\Logs` for the same logic as the config directory.

### Mac OS X

`saveDataDirectory` maps to `$HOME/Library/Application Support/{ProductPath}/Saves`, following [Apple's documentation](https://developer.apple.com/library/ios/documentation/FileManagement/Conceptual/FileSystemProgrammingGuide/MacOSXDirectories/MacOSXDirectories.html).

_Technically the Apple documentation recommends using the bundle identifier for your directory such as `~/Library/Application Support/com.example.MyApp` but in reality nobody does that, not even Apple who put data files from the Mac App Store at `~/Library/Application Support/App Store`._

`configDirectory` maps to `$HOME/Library/Application Support/{ProductPath}/Config`. Some would argue it should be in `$HOME/Library/Preferences` but per Apple's documentation:

> [Application Support] Contains all app-specific data and support files. These are the files that your app creates and manages on behalf of the user and can include files that contain user data.

> [Preferences] Contains the user’s preferences. You should never create files in this directory yourself. To get or set preference values, you should always use the NSUserDefaults class or an equivalent system-provided interface.

As such it would be wrong to map this path to the Preferences directory and save custom files but it is accepted to store all data and support files in Application Support.

`logDirectory` maps to `$HOME/Library/Logs/{ProductPath}`.

### Linux

In following the [Free Desktop Specifications](http://standards.freedesktop.org/basedir-spec/basedir-spec-latest.html#variables):

`saveDataDirectory` maps to `$XDG_DATA_HOME/{ProductPath}` when `XDG_DATA_HOME` exists, falling back to `$HOME/.local/share/{ProductPath}` if necessary.

`configDirectory` maps to `$XDG_CONFIG_HOME/{ProductPath}` when `XDG_CONFIG_HOME` exists, falling back to `$HOME/.config/{ProductPath}`

`logDirectory` maps to `/var/log/{ProductPath}`.
