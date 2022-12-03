using System;
using DontMissTravel.Audio;
using DontMissTravel.Data;
using UnityEditor;
using UnityEngine; //Using for the "not Editor"
using AudioType = DontMissTravel.Audio.AudioType;

namespace DontMissTravel
{
    public class GameSystemManager : Singleton<GameSystemManager>
    {
        public event Action<bool> OnInstructionStatusChange;

        private bool _isInstructionOpened;
        private bool _isTutorialOpened;
        private AudioManager _audioManager;
        private KeepDataManager _keepDataManager;

        private void Start()
        {
            _keepDataManager = Singleton<KeepDataManager>.Instance;
            _audioManager = Singleton<AudioManager>.Instance;

            //we don't need to check if this one exist, because it controlled by SingletonManager.
            DontDestroyOnLoad(this);
        }

        public void InvokeStartGame()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _keepDataManager.GameMode = GameMode.Game;
            _keepDataManager.LoadScene(SceneEnum.Game);
            _keepDataManager.SwitchGameState(GameState.Play);
        }

        public void ToggleTutorial(bool toOpen)
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _keepDataManager.GameMode = toOpen ? GameMode.Tutorial : GameMode.Menu;
            _keepDataManager.LoadScene(toOpen ? SceneEnum.Tutorial : SceneEnum.InitMenu);
        }

        // todo: make a button in main menu to exit the game
        private void InvokeQuitGame()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void NextLevel()
        {
            _keepDataManager.SwitchGameState(GameState.Play);
            _keepDataManager.RestartCurrentScene();
        }

        public void MainMenu()
        {
            _keepDataManager.LoadScene(SceneEnum.InitMenu);
        }

        public void ToggleInstructions(bool toShow)
        {
            _keepDataManager.GameMode = toShow ? GameMode.Instruction : GameMode.Menu;
            OnInstructionStatusChange?.Invoke(toShow);
        }

        public void RestartGame()
        {
            _keepDataManager.RestartCurrentScene();
            _keepDataManager.SwitchGameState(GameState.Play);
        }
    }
}