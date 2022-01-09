using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Move;
using UnityEngine;

namespace Core
{
    public class AggroZone : MonoBehaviour
    {
        private const float AttackDistance = 2; // its minimal value for attack opportunity
        
        [SerializeField][Min(1)]
        private int _startAggroDistance;
        [SerializeField][Min(2)]
        private int _endAggroDistance;

        private EnemyMover _enemyMover;
        private List<IDamageable> _damageableInRange;

        public bool IsAggro { get; private set; }
        public bool CanAttack { get; private set; }

        public DistanceProvider DistanceProvider { get; set; }
        public IEnumerable<IDamageable> DamageableInRange => _damageableInRange;


        public void Initialize(EnemyMover enemyMover)
        {
            _enemyMover = enemyMover;
            IsAggro = false;
        }

        public void DoUpdate()
        {
            var distance = DistanceProvider.GetDistanceFromEnemy(_enemyMover);

            if (distance <= _startAggroDistance && DistanceProvider.IsEnabled)
                IsAggro = true;
            
            if (distance >= _endAggroDistance || DistanceProvider.IsEnabled == false)
                IsAggro = false;

            _damageableInRange = GetDamageableInNeighbors().ToList();
            
            CanAttack = distance <= AttackDistance && _damageableInRange.Count > 0 && IsAggro;
        }
        
        public bool TryGetNearestTarget(out Vector2Int coord)
        {
            return DistanceProvider.TryGetNearestNeighborOfTarget(out coord, _enemyMover);
        }

        private IEnumerable<IDamageable> GetDamageableInNeighbors()
        {
            var result = new List<IDamageable>();
            
            var neighbors = DistanceProvider.GetNeighborsOfTarget(_enemyMover.CurrentPosition);
            neighbors.ForEach(cell =>
            {
                if(cell.Player != null)
                    result.Add(cell.Player.Health);
            });

            return result;
        }

        private void OnValidate()
        {
            if (_endAggroDistance <= _startAggroDistance)
                _endAggroDistance = _startAggroDistance + 1;
        }
    }
}