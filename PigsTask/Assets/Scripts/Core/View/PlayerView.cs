using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.Move;
using DG.Tweening;
using UnityEngine;

namespace Core.View
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerView : MonoBehaviour
    {
        private IFourDirectionView _defaultView;
        private IFourDirectionView _currentView;
        private SpriteRenderer _spriteRenderer;

        public async Task Initialize(IAssetProvider assetProvider)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _defaultView = new FourDirectionView();
            var defaultViewPictures = GetDefaultViewPictures();
            await _defaultView.Initialize(assetProvider, _spriteRenderer, defaultViewPictures[MoveDirection.Left], 
                defaultViewPictures[MoveDirection.Right], defaultViewPictures[MoveDirection.Up], defaultViewPictures[MoveDirection.Down]);
            
            _currentView = _defaultView;
            _currentView.SetDirection(MoveDirection.Right);
        }

        public Dictionary<MoveDirection, string> GetDefaultViewPictures()
        {
            return new Dictionary<MoveDirection, string>()
            {
                {MoveDirection.Left, AssetAddress.PlayerDefaultLeft},
                {MoveDirection.Right, AssetAddress.PlayerDefaultRight},
                {MoveDirection.Up, AssetAddress.PlayerDefaultUp},
                {MoveDirection.Down, AssetAddress.PlayerDefaultDown},
            };
        }

        public void EnableDefaultView()
        {
            _spriteRenderer.color = Color.white;
        }

        public void EnableDirtyView()
        {
            _spriteRenderer.color = Color.yellow;
        }

        public void SetDirection(MoveDirection direction)
        {
            _currentView.SetDirection(direction);
        }

        public void SetOrder(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }

        public void HandleTakeDamage()
        {
            _spriteRenderer.color = Color.red;
            _spriteRenderer.DOColor(Color.white, 0.4f).SetEase(Ease.InOutQuad);
        }
        
        public void HandleDead()
        {
            _spriteRenderer.DOKill();
            _spriteRenderer.color = Color.black;
        }
    }
}