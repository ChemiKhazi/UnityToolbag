# UnityToolbag

This repo is a host for any little Unity scripts I write that are simple and easy for others to leverage. Each folder has its own README to explain the usage in more depth than here. All scripts are being written with the latest Unity 5 and may or may not work in earlier versions.

## Features

- [AnimParamsAttribute](AnimParamsAttribute) - A property attribute that exposes the parameters of an `Animator` as a popup in the inspector.
- [CacheBehaviour](CacheBehaviour) - A drop-in replacement for `MonoBehaviour` as a script base class that provides caching of all standard properties.
- [Dispatcher](Dispatcher) - Provides a mechanism for invoking code on the main thread from background threads.
- [DrawTitleSafeArea](DrawTitleSafeArea) - Simple component you add to a camera to render the title safe area.
- [Future](Future) - Simple implementation of the [future](http://en.wikipedia.org/wiki/Futures_and_promises) programming concept.
- [QuickToggle](QuickToggle) - Basic tool for locking objects in the scene to minimize accidental edits while working.
- [SimpleSpriteAnimation](SimpleSpriteAnimation) - A very basic system for a simpler frame based animation for Unity's 2D system.
- [SnapToSurface](SnapToSurface) - Editor tools to assist in positioning objects.
- [SortingLayer](SortingLayer) - Tools for working with Unity's new sorting layers.
- [StandardPaths](StandardPaths) - A static class that exposes good locations for saving files.
- [UnityConstants](UnityConstants) - Tool for generating a C# script containing the names and values for tags, layers, sorting layers, and scenes.

## Usage

Simply clone the repository into the 'Assets' folder of a Unity project and you're good to go. If you're already using Git, you can use a submodule to check out into Assets without the Toolbag getting added to your repository.

Alternatively you can just cherry pick the features you want and copy only those folders into your project. Be careful, though, as some of the features may depend on others. See the individual feature README files to find out.

## Shameless Plug

If you find any code in here to be useful and feel so inclined, you can help me out by purchasing a game from my company, [Brushfire Games](http://brushfiregames.com). Absolutely not required (this code is free) but definitely appreciated. :)
