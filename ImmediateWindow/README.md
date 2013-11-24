Immediate Window
===

Leverage the power of C# right from the editor for quickly lining up objects, randomly adjusting scales, swapping materials, creating objects from code, or anything else exposed through the Unity Engine or Editor APIs.


Quick Start
---

1. Open the Immediate Window choosing Immediate from the Window menu.
2. Type code you want to execute in the text area. The code is inserted into a dummy class and method so you can type any code that is valid C# for the body of a method.
3. Click "Compile + Run" to compile the code and run it.
4. After code has been compiled once, you can continue running it over and over without recompiling. Once you change the text, you must recompile.

Any compilation errors are printed below the Compile + Run button for quick access to seeing what you need to fix.


Example Scripts
---

Here are a few sample scripts that are fun to play with.

Creating a new object from code:

    var go = new GameObject("Immediate Object");
    go.AddComponent<BoxCollider>();

Changing the scale of the test cubes:

    foreach (var t in Selection.transforms)
    {
        Vector3 scale = t.localScale;
        scale.y = Random.value * 3f + 1f;
        t.localScale = scale;

        Vector3 position = t.position;
        position.y = scale.y / 2;
        t.position = position;
    }

Creating a checkerboard layout of boxes

    for (int x = -5; x <= 5; x++)
    {
        for (int z = -5; z <= 5; z++)
        {
            if ((x % 2 == 0 && z % 2 != 0) || (x % 2 != 0 && z % 2 == 0))
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Checkerboard Cube";
                cube.transform.position = new Vector3(x, 0, z);
            }
        }
    }

Easily deleting all the checkerboard cubes the above script made

    GameObject cube = null;
    while ((cube = GameObject.Find("Checkerboard Cube")) != null)
    {
        cube.active = false;
        Object.DestroyImmediate(cube);
    }
