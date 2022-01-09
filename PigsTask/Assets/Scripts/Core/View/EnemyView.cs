using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.Move;
using UnityEngine;

namespace Core.View
{
    public abstract class EnemyView : MonoBehaviour
    {
        private IFourDirectionView _defaultView;
        private IFourDirectionView _aggroView;
        private IFourDirectionView _dirtyView;
        
        private IFourDirectionView _currentView;
        private SpriteRenderer _spriteRenderer;

        public IFourDirectionView CurrentView => _currentView;
        public SpriteRenderer Renderer => _spriteRenderer;

        public async Task Initialize(IAssetProvider assetProvider)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _defaultView = new FourDirectionView();
            var defaultViewPictures = GetDefaultViewPictures();
            await _defaultView.Initialize(assetProvider, _spriteRenderer, defaultViewPictures[MoveDirection.Left], 
                defaultViewPictures[MoveDirection.Right], defaultViewPictures[MoveDirection.Up], defaultViewPictures[MoveDirection.Down]);

            _aggroView = new FourDirectionView();
            var aggroViewPictures = GetAggroViewPictures();
            await _aggroView.Initialize(assetProvider, _spriteRenderer, aggroViewPictures[MoveDirection.Left], 
                aggroViewPictures[MoveDirection.Right], aggroViewPictures[MoveDirection.Up], aggroViewPictures[MoveDirection.Down]);

            _dirtyView = new FourDirectionView();
            var dirtyViewPictures = GetDirtyViewPictures();
            await _dirtyView.Initialize(assetProvider, _spriteRenderer, dirtyViewPictures[MoveDirection.Left], 
                dirtyViewPictures[MoveDirection.Right], dirtyViewPictures[MoveDirection.Up], dirtyViewPictures[MoveDirection.Down]);
            
            _currentView = _defaultView;
            CurrentView.SetDirection(MoveDirection.Right);
        }

        public void EnableAggroView()
        {
            if (_currentView == _aggroView)
                return;
            
            SwitchToView(_aggroView);
        }

        public void EnableDirtyView()
        {
            if (_currentView == _dirtyView)
                return;
            
            SwitchToView(_dirtyView);
        }

        public void EnableDefaultView()
        {
            if (_currentView == _defaultView)
                return;

            SwitchToView(_defaultView);
        }

        public void SetDirection(MoveDirection direction)
        {
            _currentView.SetDirection(direction);
        }
        
        public void SetOrder(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }

        public abstract Dictionary<MoveDirection, string> GetDefaultViewPictures();
        public abstract Dictionary<MoveDirection, string> GetDirtyViewPictures();
        public abstract Dictionary<MoveDirection, string> GetAggroViewPictures();

        private void SwitchToView(IFourDirectionView fourDirectionView)
        {
            var direction = _currentView.CurrentDirection;
            _currentView = fourDirectionView;
            _currentView.SetDirection(direction);
        }
    }
}