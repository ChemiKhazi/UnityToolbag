Sorting Layer
===

Unity 4.3 added sorting layers and manual sorting orders to all renderers, however only the sprite renderer exposes the values in the inspector. If you're making a 2D game and want to use a text mesh or a standard MeshRenderer, your only option is to adjust sorting layers in code. This folder provides a couple ways of nicely accessing sorting layers on non-sprite objects:


SortingLayerExposed
---

![Readme_SortingLayerExposed.png](https://raw.github.com/nickgravelyn/UnityToolbag/master/Sorting%20Layer/Readme_SortingLayerExposed.png)

This basic component+editor combo lets you change the sorting properties of _any_ renderer simply by putting the SortingLayerExposed component on the same object. The custom editor will provide you with the UI for choosing the sorting layer and sorting layer order for the renderer attached to the object.

SortingLayerAttribute
---

_This attribute/property drawer was adapted from [ChemiKhazi's pull request](https://github.com/nickgravelyn/UnityToolbag/pull/1)_.

![Readme_SortingLayerAttribute.png](https://raw.github.com/nickgravelyn/UnityToolbag/master/Sorting%20Layer/Readme_SortingLayerAttribute.png)

If you want to change an object's sorting layer at runtime but want to configure it in the inspector, this is the better option. Using attributes, you can have any regular integer or string property show up as a sorting layer in the Inspector. Example usage:

    public class SortLayerTest : MonoBehaviour {
        [UnityToolbag.SortingLayer]
        public int sortLayer1;

        [UnityToolbag.SortingLayer]
        public string sortLayer2;
    }

In this case both of these fields will show a dropdown of the sorting layers in the inspector, storing off their IDs. Then you can apply them easily later on:

    // Note: it's not necessary to apply changes to both sortingLayerID and sortingLayerName.
    // We're doing it here only to show the two ways to accomplish the same thing.
    renderer.sortingLayerID = sortLayer1;
    renderer.sortingLayerName = sortLayer2;

This is good, for example, to have an object that starts on one layer (say, the background) but later needs to be moved to the foreground layer.

Do note that either data type can fail later on if you're not careful. The integer value is based on the ordering of the layers so if you re-order them around, your integers may not line up with the layers you intended. Likewise if you store the name as a string, you must take care when renaming layers or else you end up with the wrong value as well. It's up to you to ensure that you pick the approach that works best for you.
