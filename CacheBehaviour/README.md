CacheBehaviour
===

The `CacheBehaviour` is a very simple subclass of `MonoBehaviour` that overrides all existing properties with cached versions. The idea is that you'd use `CacheBehaviour` as your base class instead of `MonoBehaviour`. The caching mechanism should provide some increase in performance because the standard properties always call `GetComponent()`, which means another method call and some kind of hash/dictionary lookup. `CacheBehaviour` may also be handy because, supposedly, Unity 5 is removing all of the standard properties from MonoBehaviour, so this is a nice reimplementation that should continue to work in Unity 5.
