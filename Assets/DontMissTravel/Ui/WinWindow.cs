using UnityEngine;
using UnityEngine.UI;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel.Ui
{
    public class WinWindow : Window
    {
        [SerializeField] private Button _resumeGame;
        private Hud _hud;

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

        protected void Start()
        {
            _hud = Singleton<Hud>.Instance;
        }

        private void OnResumeClick()
        {
            GameSystemManager.NextLevel();
            _hud.ShowHideWindow(WindowName.Win, false);
        }
    }
}