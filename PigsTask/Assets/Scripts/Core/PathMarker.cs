using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class PathMarker : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _renderer;
        [SerializeField] [Range(0.1f, 1f)]
        private float _duration = 0.3f;
        

        private readonly Color _defaultMarkerColor = new Color(1f,1f,1f, 0.1f);
        private readonly Color _flashMarkerColor = new Color(1f,1f,1f, 1f);
        
        public SpriteRenderer Renderer => _renderer;
        
        public void Animate()
        {
            _renderer.transform.localScale = Vector2.one * 0.6f;
            _renderer.color = _flashMarkerColor;

            _renderer.transform
                .DOScale(0, _duration)
                .SetEase(Ease.OutCubic)
                .SetLoops(3, LoopType.Restart);
            
            _renderer
                .DOFade(0, _duration)
                .SetEase(Ease.OutCubic).SetLoops(5, LoopType.Restart)
                .OnComplete(() =>
                {
                    _renderer.transform.localScale = Vector2.one * 0.6f;
                    _renderer.color = _defaultMarkerColor;
                });
        }

        public void SetColor(Color color)
        {
            _renderer.color = color;
        }
    }
}