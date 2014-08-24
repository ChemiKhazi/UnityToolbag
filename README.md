UnityToolbag
===

This repo is a host for any little Unity scripts I write that are simple and easy for others to leverage. Each folder has its own README to explain the usage in more depth than here. All scripts are being written with the latest Unity 4.3 and may or may not work in earlier versions.


Features
---

- [CacheBehaviour](CacheBehaviour) - A drop-in replacement for `MonoBehaviour` as a script base class that provides caching of all standard properties.
- [Dispatcher](Dispatcher) - Provides a mechanism for invoking code on the main thread from background threads.
- [DrawTitleSafeArea](DrawTitleSafeArea) - Simple component you add to a camera to render the title safe area.
- [EditorTools](EditorTools) - Misc tools for making it easier to build editor UI.
- [ExclusiveChildren](ExclusiveChildren) - Helper script for managing objects in a hierarchy that represent mutually exclusive options (like a set of menu screens)
- [Future](Future) - Simple implementation of the [future](http://en.wikipedia.org/wiki/Futures_and_promises) concept.
- [GameSaveSystem](GameSaveSystem) - A helper system for game saves to provide automatic backups and background thread processing along with better game save file paths.
- [ImmediateWindow](ImmediateWindow) - An editor window that allows executing manual C# snippets.
- [ScriptableObjectUtility](ScriptableObjectUtility) - An editor class to help with creating `ScriptableObject` subclasses.
- [SimpleSpriteAnimation](SimpleSpriteAnimation) - A very basic system for a simpler frame based animation for Unity's 2D system.
- [SnapToSurface](SnapToSurface) - Editor tools to assist in positioning objects.
- [SortingLayer](SortingLayer) - Tools for working with Unity's new sorting layers.
- [TimeScaleIndependentUpdate](TimeScaleIndependentUpdate) - Components to make it easier to continue animations when `Time.timeScale` is set to 0 (i.e. paused).
- [UnityConstants](UnityConstants) - Tool for generating a C# script containing the names and values for tags, layers, sorting layers, and scenes.
- [UnityLock](UnityLock) - Basic tool for locking objects in the scene to minimize accidental edits while working.


Usage
---

Simply clone the repository into the 'Assets' folder of a Unity project and you're good to go. If you're already using Git, you can use a submodule to check out into Assets without the Toolbag getting added to your repository.

Alternatively you can just cherry pick the features you want and copy only those folders into your project. Be careful, though, as some of the features may depend on others. See the individual feature README files to find out.

Any component types are exposed through the component menu under UnityToolbag:

![ComponentMenu.png](https://raw.github.com/nickgravelyn/UnityToolbag/master/ComponentMenu.png)


Shameless Plug
---

If you find any code in here to be useful and feel so inclined, you can help me out by picking up a copy of my company's first game [Shipwreck](http://brushfiregames.com/shipwreck). Absolutely not required (this code is free) but definitely appreciated. :)
