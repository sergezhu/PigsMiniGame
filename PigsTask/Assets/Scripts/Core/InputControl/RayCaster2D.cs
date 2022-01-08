using UnityEngine;

namespace Core.InputControl
{
    public class RayCaster2D : MonoBehaviour
    {
        private const float Distance = 20f;

        [SerializeField]
        private Camera _camera;

        private RaycastHit2D _hit;
        private LayerMask _layerMask;

        public RaycastHit2D Hit => _hit;

        private void Awake()
        {
            _layerMask = (1 << LayerMask.NameToLayer("Cells"));
        }

        public void DoCast()
        {
            var mousePosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(mousePosition);

            _hit = Physics2D.Raycast(ray.origin, ray.direction, Distance, _layerMask);
        }
    }
}