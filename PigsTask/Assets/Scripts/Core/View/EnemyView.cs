using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.Move;
using UnityEngine;

namespace Core.View
{
    public abstract class EnemyView : MonoBehaviour, IDefaultView, IAggroView, IDirtyView
    {
        [SerializeField]
        private FourDirectionView _defaultView;
        [SerializeField]
        private FourDirectionView _aggroView;
        [SerializeField]
        private FourDirectionView _dirtyView;
        
        private FourDirectionView _currentView;
        private SpriteRenderer _spriteRenderer;

        public FourDirectionView CurrentView => _currentView;
        public SpriteRenderer Renderer => _spriteRenderer;

        public async Task Initialize(IAssetProvider assetProvider)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            var defaultViewPictures = GetDefaultViewPictures();
            await _defaultView.Initialize(assetProvider, _spriteRenderer, defaultViewPictures[MoveDirection.Left], 
                defaultViewPictures[MoveDirection.Right], defaultViewPictures[MoveDirection.Up], defaultViewPictures[MoveDirection.Down]);
            
            var aggroViewPictures = GetAggroViewPictures();
            await _aggroView.Initialize(assetProvider, _spriteRenderer, aggroViewPictures[MoveDirection.Left], 
                aggroViewPictures[MoveDirection.Right], aggroViewPictures[MoveDirection.Up], aggroViewPictures[MoveDirection.Down]);
            
            var dirtyViewPictures = GetDirtyViewPictures();
            await _dirtyView.Initialize(assetProvider, _spriteRenderer, dirtyViewPictures[MoveDirection.Left], 
                dirtyViewPictures[MoveDirection.Right], dirtyViewPictures[MoveDirection.Up], dirtyViewPictures[MoveDirection.Down]);
            
            _currentView = _defaultView;
            CurrentView.SetDirection(MoveDirection.Right);
        }

        public void EnableAggroView()
        {
            SwitchToView(_aggroView);
        }

        public void EnableDirtyView()
        {
            SwitchToView(_dirtyView);
        }

        public void EnableDefaultView()
        {
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

        private void SwitchToView(FourDirectionView fourDirectionView)
        {
            var direction = _currentView.CurrentDirection;
            _currentView = fourDirectionView;
            _currentView.SetDirection(direction);
        }
    }
}