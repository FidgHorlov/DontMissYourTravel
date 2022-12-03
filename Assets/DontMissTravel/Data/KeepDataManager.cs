using System;
using UnityEngine;

namespace DontMissTravel.Data
{
    [DefaultExecutionOrder(Constants.ScriptOrder.KeepDataManager)]
    public class KeepDataManager : Singleton<KeepDataManager>
    {
        public event Action<GameState> OnGameStateChanged;
        private SceneEnum _currentScene;
        
        public bool IsPhone { get; set; }
        public GameMode GameMode { get; set; }
        public GameState GameState { get; private set; }
        public bool IsGameOver { get; set; }

        private void Start()
        {
            KeepDataManager dataManager = FindObjectOfType<KeepDataManager>();
            if (dataManager != this)
            {
                Destroy(dataManager);
            }
            
            DontDestroyOnLoad(this);
        }

        public void LoadScene(SceneEnum sceneEnumEnum)
        {
            SceneLoader.LoadScene(sceneEnumEnum);
            _currentScene = sceneEnumEnum;
        }

        public void RestartCurrentScene()
        {
            LoadScene(_currentScene);
        }

        public void SwitchGameState(GameState gameState)
        {
            GameState = gameState;
            OnGameStateChanged?.Invoke(gameState);
            Debug.Log($"<b>Game state changed</b> -> {gameState.ToString()}");
        }
    }
}