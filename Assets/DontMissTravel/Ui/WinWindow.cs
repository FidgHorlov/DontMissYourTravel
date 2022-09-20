using UnityEngine;
using UnityEngine.UI;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel.Ui
{
    public class WinWindow : Window
    {
        [SerializeField] private Button _resumeGame;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _resumeGame.onClick.AddListener(OnResumeClick);
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            _resumeGame.onClick.RemoveListener(OnResumeClick);
        }

        private void OnResumeClick()
        {
             GameController.Instance.NextLevel();
             Hud.Instance.ShowHideWindow(WindowName.Win, false);
        }
    }
}
