using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Move
{
    public class EnemyMover : MonoBehaviour
    {
        [SerializeField][Range(1, 10)]
        private float _speed = 2;

        [SerializeField] [Min(1)]
        private int _walkMinDistance = 1;
        [SerializeField] [Min(1)]
        private int _walkMaxDistance = 6;
        
        [SerializeField] [Min(.2f)]
        private float _relaxMinDuration = .5f;
        [SerializeField] [Min(.5f)]
        private float _relaxMaxDuration = 5;
        
        private MoveController _moveController;

        private int WalkDistance => Random.Range(_walkMinDistance, _walkMaxDistance + 1);
        public float RelaxDuration => Random.Range(_relaxMinDuration, _relaxMaxDuration);
        
        public Vector2Int CurrentPosition => _moveController.CurrentPosition.AsVector();
        public bool IsMoving => _moveController.IsMoving;
        public bool IsRelaxing { get; private set; }

        public void Initialize(MoveController moveController)
        {
            _moveController = moveController;
        }

        public void TryStartMove()
        {
            if (IsMoving)
                return;

            var desiredDistance = WalkDistance;
            _moveController.Move(desiredDistance, _speed, false);
        }

        public void TryStartRelax()
        {
            if (IsRelaxing)
                return;

            StartCoroutine(Relax(RelaxDuration));
        }

        public void Stop() => 
            _moveController.Stop();

        private IEnumerator Relax(float relaxDuration)
        {
            IsRelaxing = true;

            yield return new WaitForSeconds(relaxDuration);
            
            IsRelaxing = false;
        }
    }
}