using System;
using Core.Interfaces;

namespace Core
{
    public class Health : IDamageable
    {
        public event Action Dead;
        public event Action Changed;
        
        private bool _isDead;
        private int _maxHp;
        private int _currentHp;

        public Health(int maxHp)
        {
            _isDead = false;
            _maxHp = maxHp;
            _currentHp = _maxHp;
        }

        public int CurrentHP => _currentHp;
        public int MaxHP => _maxHp;


        public void TakeDamage(int damageValue)
        {
            if (damageValue <= 0)
                throw new InvalidOperationException("Damage value must be positive");

            _currentHp -= damageValue;
            Changed?.Invoke();

            if (_currentHp <= 0)
            {
                _currentHp = 0;
                Dead?.Invoke();
            }
        }
    }
}