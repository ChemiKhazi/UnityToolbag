Editor Tools
===

These are just some little helper tools for making editor GUI layout a bit easier. Largely utilizing the IDisposable pattern to increase readability and safety when making UI calls that require a Begin/End pair. These add a lot of safety (can't forget the End call), simplify the usage a little bit, and increase readability by creating a new scope and indent level in your code, making it easier to see what UI is inside your different blocks.

**IndentBlock**

Easily create a section of indented UI.

    EditorGUILayout.LabelField("Some Indented Stuff:");
    using (new IndentBlock())
    {
        EditorGUILayout.LabelField("This is indented!");
    }

**HorizontalBlock**

Create a horizontal section of layout.

    using (new HorizontalBlock())
    {
        if (GUILayout.Button("Left Button")) { }
        if (GUILayout.Button("Right Button")) { }
    }

**VerticalBlock**

Create a vertical section of layout.

    using (new VerticalBlock())
    {
        if (GUILayout.Button("Top Button")) { }
        if (GUILayout.Button("Bottom Button")) { }
    }

**ScrollViewBlock**

Creates a scroll view section. Requires a ref parameter for the scroll location that it will update.

    // In your editor class, define a private variable
    private Vector2 _scrollViewPosition;

    // In your GUI code you can easily make scroll views
    using (new ScrollViewBlock(ref _scrollViewPosition))
    {
        for (int i = 0; i < 20; i++)
        {
            if (GUILayout.Button("Button " + i)) { }
        }
    }
