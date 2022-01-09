using System.Collections.Generic;
using Core;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private BottomBar _bottomBar;
        
        public void Initialize(IBombButtonClickHandler bombButtonClickHandler, IEnumerable<IEarnScoresProvider> spawnControllerEarnScoresProviders)
        {
            _bottomBar.Initialize(bombButtonClickHandler, spawnControllerEarnScoresProviders);
        }
    }
}