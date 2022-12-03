using System;
using System.Collections;
using DontMissTravel.Audio;
using DontMissTravel.Data;
using UnityEngine;
using UnityEngine.UI;
using AudioType = DontMissTravel.Audio.AudioType;

namespace DontMissTravel
{
    public class InitialMenu : MonoBehaviour
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _infoButton;
        [SerializeField] private Button _tutorialButton;

        [Space] [SerializeField] private GameObject _initialMenu;
        [SerializeField] private Instructions _instructions;

        [Space] [SerializeField] private Image _mainImage;
        [SerializeField] private Sprite[] _sprites;

        private AudioManager _audioManager;
        private KeepDataManager _keepDataManager;
        private GameSystemManager _gameSystemManager;

        private void Start()
        {
            _keepDataManager = Singleton<KeepDataManager>.Instance;
            _audioManager = Singleton<AudioManager>.Instance;
            _gameSystemManager = Singleton<GameSystemManager>.Instance;

            StartCoroutine(nameof(PlayerAnimation));
            _keepDataManager.GameMode = GameMode.Menu;

#if UNITY_ANDROID || UNITY_IOS
            _keepDataManager.IsPhone = true;
#elif UNITY_STANDALONE
            _keepDataManager.IsPhone = false;
#endif
            _audioManager.PlayMusic();
            _gameSystemManager.OnInstructionStatusChange += OnInstructionStatusChange;
        }

        private void OnEnable()
        {
            _startGameButton.onClick.AddListener(OnStartGameButtonClick);
            _infoButton.onClick.AddListener(OnInfoButtonClick);
            _tutorialButton.onClick.AddListener(OnTutorialButtonClick);
        }

        private void OnDisable()
        {
            _startGameButton.onClick.RemoveListener(OnStartGameButtonClick);
            _infoButton.onClick.RemoveListener(OnInfoButtonClick);
            _tutorialButton.onClick.RemoveListener(OnTutorialButtonClick);
        }

        private void OnDestroy()
        {
            _gameSystemManager.OnInstructionStatusChange -= OnInstructionStatusChange;
        }

        private void OnInstructionStatusChange(bool toOpen)
        {
            ToggleInfoClick(toShow: toOpen);
        }

        private void OnInfoClosed()
        {
            ToggleInfoClick(toShow: false);
        }

        private void OnInfoButtonClick()
        {
            ToggleInfoClick(toShow: true);
        }

        private void ToggleInfoClick(bool toShow)
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _keepDataManager.GameMode = toShow ? GameMode.Instruction : GameMode.Menu;
            _instructions.SetActive(toShow);
            _initialMenu.SetActive(!toShow);

            if (toShow)
            {
                _instructions.OnInstructionsClosed += OnInfoClosed;
            }
            else
            {
                _instructions.OnInstructionsClosed -= OnInfoClosed;
            }
        }

        private void OnTutorialButtonClick()
        {
            _gameSystemManager.ToggleTutorial(toOpen: true);
        }

        private void OnStartGameButtonClick()
        {
            _gameSystemManager.InvokeStartGame();
        }

        private IEnumerator PlayerAnimation()
        {
            int index = 0;
            while (true)
            {
                if (index == 0)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }

                _mainImage.sprite = _sprites[index];
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}