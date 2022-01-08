using System.Collections.Generic;
using Core.Infrastructure.AssetManagement;
using Core.Move;

namespace Core.View
{
    public class DogView : EnemyView
    {
        public override Dictionary<MoveDirection, string> GetDefaultViewPictures()
        {
            return new Dictionary<MoveDirection, string>()
            {
                {MoveDirection.Left, AssetAddress.DogDefaultLeft},
                {MoveDirection.Right, AssetAddress.DogDefaultRight},
                {MoveDirection.Up, AssetAddress.DogDefaultUp},
                {MoveDirection.Down, AssetAddress.DogDefaultDown},
            };
        }

        public override Dictionary<MoveDirection, string> GetDirtyViewPictures()
        {
            return new Dictionary<MoveDirection, string>()
            {
                {MoveDirection.Left, AssetAddress.DogDirtyLeft},
                {MoveDirection.Right, AssetAddress.DogDirtyRight},
                {MoveDirection.Up, AssetAddress.DogDirtyUp},
                {MoveDirection.Down, AssetAddress.DogDirtyDown},
            };
        }

        public override Dictionary<MoveDirection, string> GetAggroViewPictures()
        {
            return new Dictionary<MoveDirection, string>()
            {
                {MoveDirection.Left, AssetAddress.DogAggroLeft},
                {MoveDirection.Right, AssetAddress.DogAggroRight},
                {MoveDirection.Up, AssetAddress.DogAggroUp},
                {MoveDirection.Down, AssetAddress.DogAggroDown},
            };
        }
    }
}