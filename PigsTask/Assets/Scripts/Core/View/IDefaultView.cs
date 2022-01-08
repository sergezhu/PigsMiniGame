using System.Collections.Generic;
using Core.Move;

namespace Core.View
{
    public interface IDefaultView
    {
        public void EnableDefaultView();
        Dictionary<MoveDirection, string> GetDefaultViewPictures();
    }
}