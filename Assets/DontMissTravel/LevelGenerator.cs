using DontMissTravel.Ui;
using UnityEngine;
using TypeOfLevel = DontMissTravel.Data.TypeOfLevel;

namespace DontMissTravel
{
    public class LevelGenerator : Singleton<LevelGenerator>
    {
#region PrivateProperties

        private const string PlayerPrefsLevelId = "Level";

        [SerializeField] private int _currentLevel;

        private TypeOfLevel _typeOfLevel;
        private Hud _hud;

#endregion

#region Properties

        public TypeOfLevel TypeOfLevel
        {
            get => _typeOfLevel;
            set => _typeOfLevel = value;
        }

        public int CurrentLevel
        {
            get => _currentLevel;
            set => _currentLevel = value;
        }

#endregion

        protected override void Awake()
        {
            base.Awake();
            _currentLevel = PlayerPrefs.GetInt(PlayerPrefsLevelId);
            if (_currentLevel == 0)
            {
                _currentLevel = 0;
            }
        }

        private void Start()
        {
            _hud = Singleton<Hud>.Instance;
            SetTextCurrentLevel();
        }

        public void ClearProgress()
        {
            _currentLevel = 0;
            PlayerPrefs.SetInt(PlayerPrefsLevelId, 0);
        }

        public void LevelUp()
        {
            _currentLevel++;
            PlayerPrefs.SetInt(PlayerPrefsLevelId, _currentLevel);
            SetTextCurrentLevel();
        }

        private void SetTextCurrentLevel()
        {
            _hud.SetCurrentLevel(_currentLevel);
        }

        public void SetCustomLevel(int value)
        {
#if UNITY_EDITOR
            _currentLevel = value;
#endif
        }
    }
}