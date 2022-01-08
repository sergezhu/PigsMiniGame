using Core.Move;
using UnityEngine;

namespace Core
{
    public class AggroZone : MonoBehaviour, ITickable
    {
        [SerializeField][Min(1)]
        private int _startAggroDistance;
        [SerializeField][Min(2)]
        private int _endAggroDistance;

        private EnemyMover _enemyMover;


        public bool IsAggroActive { get; private set; }

        public DistanceProvider DistanceProvider { get; set; }

        public void Initialize(EnemyMover enemyMover)
        {
            _enemyMover = enemyMover;
            IsAggroActive = false;
        }


        public void Tick()
        {
            var distance = DistanceProvider.GetDistanceFromEnemy(_enemyMover);

            if (distance <= _startAggroDistance)
                IsAggroActive = true;
            
            if (distance >= _endAggroDistance)
                IsAggroActive = false;
        }

        private void OnValidate()
        {
            if (_endAggroDistance <= _startAggroDistance)
                _endAggroDistance = _startAggroDistance + 1;
        }
    }
}