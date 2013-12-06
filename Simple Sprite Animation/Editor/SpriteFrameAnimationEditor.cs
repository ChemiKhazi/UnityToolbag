using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteFrameAnimation))]
public class SpriteFrameAnimationEditor : Editor
{
    [UnityEditor.MenuItem("Assets/Create/Sprite Frame Animation")]
    static void Create()
    {
        ScriptableObjectUtility.CreateAsset<SpriteFrameAnimation>();
    }
}
