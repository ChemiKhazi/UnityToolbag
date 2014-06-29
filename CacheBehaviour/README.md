CacheBehaviour and FastCacheBehaviour
===

There are two reasons not to use the default component properties (e.g. `rigidBody`, `particleSystem`, etc) on `MonoBehaviour`:

1. The properties do no caching, so each time you use the property you are calling `GetComponent`.
2. Unity 5 is [removing all of them](http://blogs.unity3d.com/2014/06/23/unity5-api-changes-automatic-script-updating/) aside from the `transform` property (which it will start caching).

The two scripts in this folder act as replacements as script base classes instead of using `MonoBehaviour` directly, but each behave differently.

`CacheBehaviour` is the safer of the two and is a drop-in replacement for `MonoBehaviour`. The properties will check the cache variable and call `GetComponent` anytime the cached value is invalid.

`FastCacheBehaviour` uses fields instead of properties. This makes accessing the components much faster but means you have to do some work to retrieve the values. The easiest solution is to call `ResetCache()` in the `Start()` or `OnEnable()` methods of your subclass, but you can also use the component specific methods (e.g. `ResetRigidbodyCache()`) or just manually call `GetComponent` yourself and assign the value. This adds some extra work, but then the cached values are just plain fields, with the quickest access possible.

If you're not calling into a lot of the components each frame, `CacheBehaviour` is the simpler of the two, but if you're accessing a lot of components frequently, `FastCacheBehaviour` might be the better way to go.
