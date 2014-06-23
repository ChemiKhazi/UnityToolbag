CacheBehaviour and FastCacheBehaviour
===

There are two reasons not to use the default component properties (e.g. `rigidBody`, `particleSystem`, etc) on `MonoBehaviour`:

1. The properties do no caching, so each time you use the property you are calling `GetComponent`.
2. Unity 5 is [removing all of them](http://blogs.unity3d.com/2014/06/23/unity5-api-changes-automatic-script-updating/) aside from the `transform` property (which it will start caching).

The two scripts in this folder act as replacements as script base classes instead of using `MonoBehaviour` directly. These provide all the same properties as `MonoBehaviour` does currently, but they cache the results to optimize the properties.

The two scripts differ in one big way: how they test for the cached value. Unity does some [clever things](http://blogs.unity3d.com/2014/05/16/custom-operator-should-we-keep-it/) when you compare components since the managed objects are just wrappers around the native objects. With this in mind, `CacheBehaviour` is a "safe" cache in that it performs the implicit cast to bool which will check the components for being non-null and not destroyed. `FastCacheBehaviour` does a manually null check that is much faster, but doesn't check if the object is not destroyed.

`CacheBehaviour` is the safer of the two options since it can handle components being destroyed and re-added, however `FastCacheBehaviour` has the better performance for situations where you are not worried about components being destroyed during the lifetime of the script.
