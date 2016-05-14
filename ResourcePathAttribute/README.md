Resource Path Attribute
===

Turns a string property into an Object Field in the inspector which only accepts objects from Resource paths. Helps keep config file loading from getting bloated when referencing large files.

Usage
---

```C#
public class ExampleConfig : ScriptableObject
{
  // An object field that accepts generic objects
  [ResourcePath]
  public string prefabPath;

  // An object field that only accepts AudioClip objects
  [ResourcePath(typeof(AudioClip))]
  public string audioPath;
}
```
