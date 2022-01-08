using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Core
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField]
        private bool _isObstacle;
        
        [Space]
        [SerializeField]
        private int _x;
        [SerializeField]
        private int _y;
        [SerializeField]
        private Vector3 _worldPosition;
        [SerializeField]
        private PathMarker _pathMarker;
        [SerializeField]
        private SpriteRenderer _backgroundRenderer;
        

        public int X => _x;
        public int Y => _y;

        public CellCoords Coords =>
            new CellCoords(_x, _y);
        
        public GridCell Parent { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F { get; set; }

        public bool IsObstacle => _isObstacle;
        public bool HasEnemy { get; set; }
        public bool HasPlayer { get; set; }
        public bool HasBomb { get; set; }
        public Vector3 WorldPosition => _worldPosition;

        public SpriteRenderer BackgroundRenderer => _backgroundRenderer;
        

        public void Initialize(int x, int y, Vector3 worldPosition)
        {
            _x = x;
            _y = y;
            _worldPosition = worldPosition;
        }

        public void SetAsObstacle() => _isObstacle = true;
        public void SetAsDefault() => _isObstacle = false;
        public void Enable() => _backgroundRenderer.enabled = true;
        public void Disable() => _backgroundRenderer.enabled = false;

        public void AnimateMarker() => _pathMarker.Animate();
        public void SetMarkerColor(Color color) => _pathMarker.SetColor(color);
    }
}
