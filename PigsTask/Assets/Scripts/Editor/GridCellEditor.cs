using Core;
using Core.Spawn;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class GridCellEditor : UnityEditor.Editor
    {
        private static readonly Color RedColor = new Color(0.8f, 0, 0, 0.5f);
        private static readonly Color GreenColor = new Color(0f, 0.8f, 0, 0.5f);
        private static readonly Color BlueColor = new Color(.1f, 0.1f, .9f, 0.7f);
        private static readonly Color DarkGrayColor = new Color(.2f, 0.2f, .2f, 0.7f);
        private static readonly Color DarkYellowColor = new Color(.5f, 0.5f, .2f, 0.7f);
        private static readonly Color MarkerColor = new Color(1f, 1f, 1f, 0.1f);
        
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(GridCell cell, GizmoType gizmo)
        {
            Gizmos.color = Color.red;

            var backgroundRenderer = cell.BackgroundRenderer;
            var tintColor = cell.IsObstacle ? RedColor : GreenColor;

            cell.SetMarkerColor(MarkerColor);

            if (cell.TryGetComponent(out EntitySpawner spawner))
            {
                switch (spawner.Type)
                {
                    case EntitySpawner.EntityType.Player:
                        tintColor = BlueColor;
                        break;
                    case EntitySpawner.EntityType.Farmer:
                        tintColor = DarkGrayColor;
                        break;
                    case EntitySpawner.EntityType.Dog:
                        tintColor = DarkYellowColor;
                        break;
                    case EntitySpawner.EntityType.Eat:
                        break;
                }
            }

            backgroundRenderer.color = tintColor;
        }
    }
}
