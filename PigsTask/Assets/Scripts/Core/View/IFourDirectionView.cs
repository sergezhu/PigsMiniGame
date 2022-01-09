using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.Move;
using UnityEngine;

namespace Core.View
{
    public interface IFourDirectionView
    {
        MoveDirection CurrentDirection { get; }
        void SetDirection(MoveDirection direction);
        Task Initialize(IAssetProvider assetProvider, SpriteRenderer spriteRenderer, string leftPictureID, string rightPictureID, string upPictureID, string downPictureID);
    }
}