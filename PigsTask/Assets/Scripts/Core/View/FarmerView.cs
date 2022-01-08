using System.Collections.Generic;
using Core.Infrastructure.AssetManagement;
using Core.Move;

namespace Core.View
{
    public class FarmerView : EnemyView
    {
        public override Dictionary<MoveDirection, string> GetDefaultViewPictures()
        {
            return new Dictionary<MoveDirection, string>()
            {
                {MoveDirection.Left, AssetAddress.FarmerDefaultLeft},
                {MoveDirection.Right, AssetAddress.FarmerDefaultRight},
                {MoveDirection.Up, AssetAddress.FarmerDefaultUp},
                {MoveDirection.Down, AssetAddress.FarmerDefaultDown},
            };
        }

        public override Dictionary<MoveDirection, string> GetDirtyViewPictures()
        {
            return new Dictionary<MoveDirection, string>()
            {
                {MoveDirection.Left, AssetAddress.FarmerDirtyLeft},
                {MoveDirection.Right, AssetAddress.FarmerDirtyRight},
                {MoveDirection.Up, AssetAddress.FarmerDirtyUp},
                {MoveDirection.Down, AssetAddress.FarmerDirtyDown},
            };
        }

        public override Dictionary<MoveDirection, string> GetAggroViewPictures()
        {
            return new Dictionary<MoveDirection, string>()
            {
                {MoveDirection.Left, AssetAddress.FarmerAggroLeft},
                {MoveDirection.Right, AssetAddress.FarmerAggroRight},
                {MoveDirection.Up, AssetAddress.FarmerAggroUp},
                {MoveDirection.Down, AssetAddress.FarmerAggroDown},
            };
        }
    }
}