using UnityEngine;
using System.Collections;

public class SpriteFrameAnimation : ScriptableObject
{
    public bool loop;

    // Duration is in milliseconds
    public float frameDuration = 15.0f;
    public Sprite[] frames = new Sprite[0];

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/Sprite Frame Animation")]
    static void Create()
    {
        ScriptableObjectUtility.CreateAsset<SpriteFrameAnimation>();
    }
#endif
}
