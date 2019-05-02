using RunAndJump;
using UnityEngine;

namespace Assets.Tools.LevelCreator.Scripts
{
    [ExecuteInEditMode]
    public class SnapToGridTest : MonoBehaviour
    {
        private void Update()
        {
            Vector3 gridCoord = Level.Instance.WorldToGridCoordinates(transform.position);
            transform.position = Level.Instance.GridToWorldCoordinates((int)gridCoord.x, (int)gridCoord.y);
        }

        private void OnDrawGizmos()
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = (Level.Instance.IsInsideGridBounds(transform.
                position)) ? Color.green : Color.red;
            Gizmos.DrawCube(transform.position, Vector3.one * Level.GridSize);
            Gizmos.color = oldColor;
        }
    }
}