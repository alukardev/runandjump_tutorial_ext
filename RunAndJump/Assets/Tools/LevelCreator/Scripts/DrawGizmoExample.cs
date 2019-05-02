using UnityEditor;
using UnityEngine;

namespace Assets.Tools.LevelCreator.Scripts
{
    public class DrawGizmoExample : MonoBehaviour
    {
        [DrawGizmo(GizmoType.NotInSelectionHierarchy |
                   GizmoType.Selected |
                   GizmoType.Pickable)]
        private static void MyCustomOnDrawGizmos(TargetExample targetExample, GizmoType gizmoType)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(targetExample.transform.position, Vector3.one);
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy |
                   GizmoType.Active)]
        private static void MyCustomOnDrawGizmosSelected(
            TargetExample targetExample, GizmoType gizmoType)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                targetExample.transform.position, Vector3.one);
        }
    }
}