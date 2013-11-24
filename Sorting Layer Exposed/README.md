Sorting Layer Exposed
===

Unity 4.3 added sorting layers and manual sorting orders to all renderers, however only the sprite renderer exposes the values in the inspector. If you're making a 2D game and want to use a text mesh or a standard MeshRenderer, your only option is to adjust sorting layers in code.

This basic component+editor combo lets you change the sorting properties of _any_ renderer simply by putting the SortingLayerExposed component on the same object. I haven't found a way to make a nice dropdown like the sprite renderer, but this works.
