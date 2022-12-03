using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DontMissTravel.Data
{
    public class SceneLoader
    {
        private const string InitMenu = "InitMenu";
        private const string Tutorial = "Tutorial";
        private const string Game = "Game";
        
        private static readonly Dictionary<SceneEnum, string> SceneDictionary = new Dictionary<SceneEnum, string>
        {
            {SceneEnum.InitMenu, InitMenu},
            {SceneEnum.Tutorial, Tutorial},
            {SceneEnum.Game, Game}
        };

        private static string GetSceneName(SceneEnum sceneEnum) => SceneDictionary[sceneEnum];

        public static void LoadScene(SceneEnum sceneEnum)
        {
            SceneManager.LoadSceneAsync(GetSceneName(sceneEnum));
        }
    }
}