using System.Collections.Generic;
using Core.Move;

namespace Core.View
{
    public interface IDirtyView
    {
        public void EnableDirtyView();
        Dictionary<MoveDirection, string> GetDirtyViewPictures();
    }
}