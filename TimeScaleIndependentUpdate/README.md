Time Scale Independent Update
===

This is my take on the code shared by [Asteroid Base](http://www.asteroidbase.com/) in their blog post [Pausing Without Pausing.](http://www.asteroidbase.com/devlog/9-pausing-without-pausing/). Here's an exerpt to give some context, but read their whole post for a full explanation:

> The simplest approach to pausing your game in Unity is to set `Time.timeScale = 0`. While the time scale is 0, `Update` methods in your scripts will still called, but `Time.deltaTime` will always return 0. This works well if you want to pause all on-screen action, but it is severely limiting if you need animated menus or overlays, since `Time.timeScale = 0` also pauses animations and particle systems.
>
> We first encountered this limitation when we were trying to implement a world map in Lovers. When the player enters the ship’s map station, we display a overlay of the current level. Since the map obstructs the ship and, as such, inhibits gameplay, we needed to pause the game while the display is visible. However, a completely static map screen would make it difficult to convey information (and also look pretty dull). In order to achieve our goal we needed a separate way to track how much time has elapsed since the last update loop.
>
> It turns out that `Time.realtimeSinceStartup` is the ideal mechanism for this. As its name implies, `Time.realtimeSinceStartup` uses the system clock to track how much time has elapsed since the game was started, independent of any time scale manipulation you may be doing. By tracking the previous update’s `Time.realtimeSinceStartup`, we can calculate a good approximation of the delta time since the last frame.

Instead of using `Time.realtimeSinceStartup`, I instead use the `Stopwatch.GetTimestamp()` and `Stopwatch.Frequency` to calculate the elapsed time. This approach uses longs for the main time tracking, converting to float only for the delta, in order to avoid issues with floating point values representing long running time. From [http://www.altdevblogaday.com/2012/02/05/dont-store-that-in-a-float/](http://www.altdevblogaday.com/2012/02/05/dont-store-that-in-a-float/):

> Therefore, if our game timer starts at zero and we store time in a float then after a minute the best precision we can get from our timer is 3.8 microseconds. After our game has been running for an hour our best precision drops to 0.24 milliseconds. After our game has been running for a day our precision drops to 7.8 milliseconds, and after a week our precision drops to 62.5 milliseconds.

While a game running for a day is farfetched, the fact is that using a float for that time means that by the end of the first day your accuracy has dropped to 1/120 of a second. For games that may have to pass long running soak tests (such as for consoles), this could be problematic. Since Unity exposes `Time.realtimeSinceStartup` as a float, we can't get any better resolution from them so we have to use other functionality to achieve this, in this case the `Stopwatch` class from .NET.

Additionally the following smaller changes were made:

- Use of cached components in the animation and particle system components for improved performance.
- Additional callback signature for the animation component.
- Made animation component public variables private with [SerializeField] so they're exposed to the Inspector but not available in scripting because options for an animation to play on startup aren't particularly useful in script and do nothing once the script has started.
