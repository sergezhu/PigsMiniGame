using UnityEngine;

namespace Core.Move
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField][Range(1, 10)]
        private float _speed = 3;
        
        private MoveController _moveController;
        private GridCell _targetCell;

        public Vector2Int CurrentPosition => _moveController.CurrentPosition;
        public bool IsMoving => _moveController.IsMoving;
        public bool IsEnabled { get; private set; }

        public void Initialize(MoveController moveController)
        {
            _moveController = moveController;
            IsEnabled = true;
        }

        public void CleanUp()
        {
            _moveController.CleanUp();
            IsEnabled = false;
        }

        public void HandleCellClick(GridCell cell)
        {
            Debug.Log($"cell click {cell.Coords}");
            
            _targetCell = cell;
            TryStartMove();
        }
        
        public void TryStartMove()
        {
            if (IsMoving)
                return;

            _moveController.Move(_targetCell.Coords, _speed, true);
        }

        public void Stop() => 
            _moveController.Stop();
    }
}