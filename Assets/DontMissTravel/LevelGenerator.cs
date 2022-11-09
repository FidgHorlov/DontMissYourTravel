using DontMissTravel.Audio;
using DontMissTravel.Ui;
using UnityEngine;
using TypeOfLevel = DontMissTravel.Data.TypeOfLevel;

namespace DontMissTravel
{
    public class LevelGenerator : MonoBehaviour
    {
#region PrivateProperties

        [SerializeField] private int _currentLevel;
        private static LevelGenerator _instance;

        private TypeOfLevel _typeOfLevel;
        private Hud _hud;

#endregion

#region Properties

        public static LevelGenerator Instance
        {
            get => _instance;
            set => _instance = value;
        }

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

        private void Awake()
        {
            _instance = this;
            _currentLevel = PlayerPrefs.GetInt("Level");
            if (_currentLevel == 0)
            {
                _currentLevel = 0;
            }
        }

        private void Start()
        {
            _hud = Hud.Instance;
            SetTextCurrentLevel();
        }

        public void LevelUp()
        {
            _currentLevel++;
            PlayerPrefs.SetInt("Level", _currentLevel);
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