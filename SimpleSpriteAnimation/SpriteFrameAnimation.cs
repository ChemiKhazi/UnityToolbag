using UnityEngine;

namespace UnityToolbag
{
    [CreateAssetMenu(order = 1000)]
    public class SpriteFrameAnimation : ScriptableObject
    {
        public bool loop;
        public float frameDuration = 15.0f; // duration is in milliseconds
        public Sprite[] frames = new Sprite[0];
    }
}
