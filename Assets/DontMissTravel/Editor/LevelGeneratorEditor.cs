using UnityEditor;
using UnityEngine;

namespace DontMissTravel.Editor
{
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelGeneratorEditor : UnityEditor.Editor
    {
        private const string OpenLevelSettings = "Open level settings";
        private const string HideLevelSettings = "Hide level settings";

        private int _levelValue;
        private bool _isCustomLevel;
        private bool _isSettingsOpen;
        private LevelGenerator _levelGenerator;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _levelGenerator = (LevelGenerator) target;
            if (GUILayout.Button(_isSettingsOpen ? HideLevelSettings : OpenLevelSettings))
            {
                _isSettingsOpen = !_isSettingsOpen;
            }

            if (!_isSettingsOpen)
            {
                return;
            }

            _isCustomLevel = GUILayout.Toggle(_isCustomLevel, "Set custom level");
            if (_isCustomLevel)
            {
                _levelValue = EditorGUILayout.IntField("Target level: ", _levelValue);
                if (!GUILayout.Button("Set custom level"))
                {
                    return;
                }

                SetLevelInt(_levelValue);
            }
            else
            {
                if (GUILayout.Button("Erase level data"))
                {
                    SetLevelInt(0);
                }
            }
        }

        private void SetLevelInt(int value)
        {
            PlayerPrefs.SetInt("Level", value);
            Debug.Log($"<b><color=red>Level updated!</color></b> New level: {value}");
            _levelGenerator.SetCustomLevel(value);
        }
    }
}