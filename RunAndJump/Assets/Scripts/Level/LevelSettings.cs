using System;
using UnityEngine;

namespace Assets.Scripts.Level
{
    [Serializable]
    public class LevelSettings : ScriptableObject
    {
        public float Gravity = -30;
        public AudioClip Bgm;
        public Sprite Background;
    }
}