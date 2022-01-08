using Core;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private BottomBar _bottomBar;
        
        public void Initialize(IBombButtonClickHandler bombButtonClickHandler)
        {
            _bottomBar.Initialize(bombButtonClickHandler);
        }
    }
}