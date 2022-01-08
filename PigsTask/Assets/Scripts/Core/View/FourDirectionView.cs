using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.Move;
using UnityEngine;

namespace Core.View
{
    [Serializable]
    public class FourDirectionView : IFourDirectionView
    {
        [SerializeField]
        private Sprite _leftDirectionPicture;
        [SerializeField]
        private Sprite _rightDirectionPicture;
        [SerializeField]
        private Sprite _upDirectionPicture;
        [SerializeField]
        private Sprite _downDirectionPicture;

        private Dictionary<MoveDirection, Sprite> _directionPictures;
        private SpriteRenderer _spriteRenderer;

        public async Task Initialize(IAssetProvider assetProvider, SpriteRenderer spriteRenderer, string leftPictureID, string rightPictureID, string upPictureID, string downPictureID)
        {
            var leftPictureLoad = assetProvider.Load<Sprite>(leftPictureID);
            var rightPictureLoad = assetProvider.Load<Sprite>(rightPictureID);
            var upPictureLoad = assetProvider.Load<Sprite>(upPictureID);
            var downPictureLoad = assetProvider.Load<Sprite>(downPictureID);

            var pictures = await Task.WhenAll(new Task<Sprite>[] {leftPictureLoad, rightPictureLoad, upPictureLoad, downPictureLoad});
            
            _directionPictures = new Dictionary<MoveDirection, Sprite>()
            {
                {MoveDirection.Left, pictures[0]},
                {MoveDirection.Right, pictures[1]},
                {MoveDirection.Up, pictures[2]},
                {MoveDirection.Down, pictures[3]},
            };

            _spriteRenderer = spriteRenderer;
        }

        public MoveDirection CurrentDirection { get; private set; }

        public void SetDirection(MoveDirection direction)
        {
            CurrentDirection = direction;
            _spriteRenderer.sprite = _directionPictures[CurrentDirection];
        }
    }
}