using System.Collections;
using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;

namespace Core
{
    public class DamageDealer : MonoBehaviour
    {
        [SerializeField][Range(1, 5)]
        private int _amount = 1;
        [SerializeField][Range(.2f, 2)]
        private float _duration = .75f;

        private List<IDamageable> _damageables;
        private Coroutine _damageCoroutine;
        private WaitForSeconds _waiter;

        public void Initialize(List<IDamageable> damageables)
        {
            _damageables = damageables;

            _waiter = new WaitForSeconds(_duration);
        }

        public void StartDamage()
        {
            _damageCoroutine = StartCoroutine(Damage());
        }

        public void StopDamage()
        {
            if (_damageCoroutine != null)
            {
                StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }
        }

        private IEnumerator Damage()
        {
            while (true)
            {
                _damageables.ForEach(d => d.TakeDamage(_amount));
                yield return _waiter;
            }
        }
    }
}