using System;

namespace Core.Move
{
    public class MoveEntityCallbacks
    {
        private Action<GridCell> _cellMarkAction;
        private Action<GridCell> _cellUnmarkAction;
        private Func<GridCell, bool> _checkCellIfStopFunc;

        public MoveEntityCallbacks(Action<GridCell> cellMarkAction, Action<GridCell> cellUnmarkAction, Func<GridCell, bool> checkCellIfStopFunc)
        {
            _cellMarkAction = cellMarkAction;
            _cellUnmarkAction = cellUnmarkAction;
            _checkCellIfStopFunc = checkCellIfStopFunc;
        }

        public Action<GridCell> CellMarkAction => _cellMarkAction;
        public Action<GridCell> CellUnmarkAction => _cellUnmarkAction;
        public Func<GridCell, bool> CheckCellIfStopFunc => _checkCellIfStopFunc;
    }
}