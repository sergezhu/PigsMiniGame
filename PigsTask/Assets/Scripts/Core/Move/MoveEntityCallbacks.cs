using System;

namespace Core.Move
{
    public class MoveEntityCallbacks
    {
        private Action<GridCell> _cellMarkAction;
        private Action<GridCell> _cellUnmarkAction;
        private Func<GridCell, bool> _checkCellMarkFunc;

        public MoveEntityCallbacks(Action<GridCell> cellMarkAction, Action<GridCell> cellUnmarkAction, Func<GridCell, bool> checkCellMarkFunc)
        {
            _cellMarkAction = cellMarkAction;
            _cellUnmarkAction = cellUnmarkAction;
            _checkCellMarkFunc = checkCellMarkFunc;
        }

        public Action<GridCell> CellMarkAction => _cellMarkAction;
        public Action<GridCell> CellUnmarkAction => _cellUnmarkAction;
        public Func<GridCell, bool> CheckCellMarkFunc => _checkCellMarkFunc;
    }
}