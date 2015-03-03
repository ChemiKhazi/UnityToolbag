using UnityEngine;
using UnityEditor;

namespace UnityToolbag
{
    [CustomEditor(typeof(SpriteFrameAnimation))]
    public class SpriteFrameAnimationEditor : Editor
    {
        [UnityEditor.MenuItem("Assets/Create/UnityToolbag/Sprite Frame Animation")]
        static void Create()
        {
            ScriptableObjectUtility.CreateAsset<SpriteFrameAnimation>();
        }
    }
}
