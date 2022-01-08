using System.Collections.Generic;
using Core.Move;

namespace Core.View
{
    public interface IAggroView
    {
        public void EnableAggroView();
        Dictionary<MoveDirection, string> GetAggroViewPictures();
    }
}