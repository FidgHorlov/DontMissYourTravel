using DontMissTravel.Obstacles.Environments;
using UnityEditor;
using UnityEngine;

namespace DontMissTravel.Editor
{
    [CustomEditor(typeof(Environment))]
    public class EnvironmentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!GUILayout.Button("Change Environment"))
            {
                return;
            }

            Environment environment = (Environment) target;
            environment.ChangeEnvironment();
            base.Repaint();
        }
    }
}