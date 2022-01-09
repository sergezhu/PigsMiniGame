using Core.Move;
using UnityEngine;

namespace Core
{
    public class AggroZone : MonoBehaviour
    {
        [SerializeField][Min(1)]
        private int _startAggroDistance;
        [SerializeField][Min(2)]
        private int _endAggroDistance;

        private EnemyMover _enemyMover;

        public bool IsAggro { get; private set; }

        public DistanceProvider DistanceProvider { get; set; }

        public void Initialize(EnemyMover enemyMover)
        {
            _enemyMover = enemyMover;
            IsAggro = false;
        }

        public void DoUpdate()
        {
            var distance = DistanceProvider.GetDistanceFromEnemy(_enemyMover);

            if (distance <= _startAggroDistance)
                IsAggro = true;
            
            if (distance >= _endAggroDistance)
                IsAggro = false;
        }
        
        public bool TryGetNearestTarget(out Vector2Int coord)
        {
            return DistanceProvider.TryGetNearestTarget(out coord, _enemyMover);
        }

        private void OnValidate()
        {
            if (_endAggroDistance <= _startAggroDistance)
                _endAggroDistance = _startAggroDistance + 1;
        }
    }
}