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
        private const KeyCode RestartButton = KeyCode.Return;

        private GameSystemManager _gameSystemManager;
        private GameController _gameController;
        private Hud _hud;

        private void Awake()
        {
            _gameSystemManager = GameSystemManager.Instance;
            _gameController = GameController.Instance;
            _hud = Hud.Instance;
        }

        private void LateUpdate()
        {
            switch (_gameSystemManager.CurrentGameStatus)
            {
                case GameSystemManager.GameStatus.Menu:
                    if (IsKeyPressed(StartInstruction))
                    {
                        _gameSystemManager.InvokeInfoClick();
                        break;
                    }

                    if (IsKeyPressed(StartTutorial))
                    {
                        _gameSystemManager.InvokeTutorialClick();
                        break;
                    }

                    if (IsKeyPressed(StartGame))
                    {
                        _gameSystemManager.InvokeStartGame();
                    }
                    
                    if (IsKeyPressed(QuitGame))
                    {
                        _gameSystemManager.InvokeQuitGame();
                    }

                    break;
                case GameSystemManager.GameStatus.Instruction:
                    break;
                case GameSystemManager.GameStatus.Game:
                    if (IsKeyPressed(PauseGame))
                    {
                        _gameController.SwitchGameState(GameState.Pause);
                        break;
                    }

                    break;
                case GameSystemManager.GameStatus.Tutorial:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsKeyPressed(KeyCode keyCode)
        {
            return Input.GetKeyDown(keyCode);
        }
    }
}