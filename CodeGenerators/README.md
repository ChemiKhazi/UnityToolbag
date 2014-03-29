CodeGenerators
===

In this folder will live any code generators that are useful during development. In the editor all generators are accessible from the `UnityToolbag` main menu item under `Code Generators`.

CodeGenTagsAndLayers ("Tags and Layers" in the menu)
---

This generator will create a single file containing constants for all tags, sorting layers, and layers defined in the editor. When you first run the command it will prompt you for a directory in which to save `TagsAndLayers.cs`. Each time after this, it will find that file and replace it, making it very quick to re-run the process.

This is an example of what you'll see in `TagsAndLayers.cs`:

    public static class Tags {
        public const string Untagged = "Untagged";
        public const string Respawn = "Respawn";
        public const string Finish = "Finish";
        public const string EditorOnly = "EditorOnly";
        public const string MainCamera = "MainCamera";
        public const string Player = "Player";
        public const string GameController = "GameController";
        public const string Test = "Test";
        public const string AnotherTest = "Another Test";
        public const string _3Testing = "3 Testing";
    }

    public static class SortingLayers {
        public const int Default = 0;
        public const int Second = 1;
        public const int Another = 2;
        public const int NewLayer3 = 3;
    }

    public static class Layers {
        public const int Default = 0;
        public const int TransparentFX = 1;
        public const int IgnoreRaycast = 2;
        public const int Water = 4;
        public const int Layer8 = 8;
        public const int Layer12 = 12;
    }
