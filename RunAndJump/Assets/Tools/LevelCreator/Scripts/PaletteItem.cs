﻿using UnityEngine;

namespace Assets.Tools.LevelCreator.Scripts
{
    public class PaletteItem : MonoBehaviour
    {
#if UNITY_EDITOR
        public enum Category
        {
            Misc,
            Colectables,
            Enemies,
            Blocks,
        }
        public Category category = Category.Misc;
        public string itemName = "";
        public Object inspectedScript;
#endif
    }
}