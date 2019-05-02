using UnityEngine;

namespace Assets.Tools.LevelCreator.Scripts
{
    public class TimeAttribute : PropertyAttribute
    {
        public readonly bool DisplayHours;
        public TimeAttribute(bool displayHours = false)
        {
            DisplayHours = displayHours;
        }
    }
}