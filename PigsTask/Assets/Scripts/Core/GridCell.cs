using Core.Interfaces;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Core
{
    public class GridCell : MonoBehaviour
    {
        private static readonly Color RedColor = new Color(0.8f, 0, 0, 0.5f);
        private static readonly Color GreenColor = new Color(0f, 0.8f, 0, 0.5f);
        private static readonly Color BlueColor = new Color(.1f, 0.1f, .9f, 0.7f);
        private static readonly Color DarkGrayColor = new Color(.2f, 0.2f, .2f, 0.7f);
        private static readonly Color DarkYellowColor = new Color(.5f, 0.5f, .2f, 0.7f);
        
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
        public IDamageable Damageable { get; set; }
        public bool IsFree => HasEnemy == false && HasBomb == false && IsObstacle == false;
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

        public void Enable(Color color)
        {
            _backgroundRenderer.enabled = true;
            _backgroundRenderer.color = color;
        }

        public void AnimateMarker() => _pathMarker.Animate();
        public void SetMarkerColor(Color color) => _pathMarker.SetColor(color);

        public void UpdateView()
        {
            Disable();

            if (HasPlayer)
                Enable(Color.magenta);
            
            if(HasEnemy)
                Enable(Color.yellow);
            
            if(HasEnemy && HasPlayer)
                Enable(Color.blue);
        }
    }
}
