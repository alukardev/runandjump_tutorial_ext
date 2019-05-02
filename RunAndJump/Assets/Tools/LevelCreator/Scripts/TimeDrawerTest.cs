using UnityEngine;

namespace Assets.Tools.LevelCreator.Scripts
{
    public class TimeDrawerTest : MonoBehaviour
    {
        [Time]
        public int TimeMinutes = 3600;
        [Time(true)]
        public int TimeHours = 3600;
    }
}