using UnityEngine;

namespace Assets.Tools.LevelCreator.Scripts
{
    public class GizmoExample : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawIcon(transform.position, "icon.png");
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                transform.position, Vector3.one);
        }
    }
}