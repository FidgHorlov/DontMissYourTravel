using System;
using DontMissTravel.Data;
using DontMissTravel.Ui;
using UnityEngine;

namespace DontMissTravel
{
    public class InputKeyManager : MonoBehaviour
    {
        // Menu buttons
        private const KeyCode StartGame = KeyCode.Return;
        private const KeyCode StartTutorial = KeyCode.T;
        private const KeyCode StartInstruction = KeyCode.I;
        private const KeyCode QuitGame = KeyCode.Escape;

        // In Game buttons
        private const KeyCode ContinueGame = KeyCode.Return;
        private const KeyCode PauseGame = KeyCode.Escape;
        private const KeyCode RestartButton = KeyCode.R;
        private const KeyCode MainMenu = KeyCode.Backspace;

        // Instruction button
        private const KeyCode CloseInstruction = KeyCode.Escape;

        private GameSystemManager _gameSystemManager;
        private KeepDataManager _keepDataManager;
        private Hud _hud;

        private void Start()
        {
            _gameSystemManager = Singleton<GameSystemManager>.Instance;
            _keepDataManager = Singleton<KeepDataManager>.Instance;
            _hud = Singleton<Hud>.Instance;
        }

        private void LateUpdate()
        {
            if (_gameSystemManager == null)
            {
                return;
            }

            switch (_keepDataManager.GameMode)
            {
                case GameMode.Menu:
                    if (IsKeyPressed(StartInstruction))
                    {
                        _gameSystemManager.ToggleInstructions(toShow: true);
                        break;
                    }

                    if (IsKeyPressed(StartTutorial))
                    {
                        _gameSystemManager.ToggleTutorial(toOpen: true);
                        break;
                    }

                    if (IsKeyPressed(StartGame))
                    {
                        _gameSystemManager.InvokeStartGame();
                        break;
                    }

                    if (IsKeyPressed(QuitGame))
                    {
                        _gameSystemManager.MainMenu();
                        break;
                    }

                    break;
                case GameMode.Instruction:
                    if (IsKeyPressed(CloseInstruction) || IsKeyPressed(StartInstruction))
                    {
                        _gameSystemManager.ToggleInstructions(toShow: false);
                        break;
                    }

                    break;
                case GameMode.Game:
                case GameMode.Tutorial:
                    if (!IsHudExistHere())
                    {
                        break;
                    }

                    if (_keepDataManager.IsGameOver)
                    {
                        RestartAndMenuIfNeeded();

                        if (IsKeyPressed(ContinueGame))
                        {
                            _gameSystemManager.NextLevel();
                        }

                        break;
                    }

                    if (IsKeyPressed(PauseGame))
                    {
                        _hud.OnPauseClick();
                        break;
                    }

                    if (!_hud.IsPaused)
                    {
                        break;
                    }

                    if (IsKeyPressed(ContinueGame))
                    {
                        _hud.OnPauseClick();
                        break;
                    }

                    RestartAndMenuIfNeeded();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RestartAndMenuIfNeeded()
        {
            if (IsKeyPressed(RestartButton))
            {
                _gameSystemManager.RestartGame();
                return;
            }

            if (IsKeyPressed(MainMenu))
            {
                _gameSystemManager.MainMenu();
            }
        }

        private bool IsKeyPressed(KeyCode keyCode) => Input.GetKeyDown(keyCode);
        private bool IsHudExistHere() => _hud != null;
    }
}