using UnityEngine;
using System.Collections;

public class SpriteFrameAnimation : ScriptableObject
{
    public bool loop;
    public float frameDuration = 15.0f; // duration is in milliseconds
    public Sprite[] frames = new Sprite[0];
}
