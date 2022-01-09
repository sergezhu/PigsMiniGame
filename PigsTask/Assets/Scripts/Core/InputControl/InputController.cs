using System;
using UnityEngine;

namespace Core.InputControl
{
    public class InputController
    {
        public event Action<GridCell> CellClick; 
        
        private RayCaster2D _rayCaster;

        public InputController(RayCaster2D rayCaster)
        {
            _rayCaster = rayCaster;
        }

        public void DoUpdate()
        {
            if (Input.GetMouseButtonDown(0) == false)
                return;
            
            _rayCaster.DoCast();

            var hitCollider = _rayCaster.Hit.collider;

            if (hitCollider is null)
                return;

            if (hitCollider.TryGetComponent(out GridCell cell))
            {
                //Debug.Log($"hit on cell {cell.Coords.AsVector()}");
                CellClick?.Invoke(cell);
            }
        }
    }
}