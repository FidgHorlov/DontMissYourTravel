using System;
using UnityEditor;
using UnityEngine;

namespace DontMissTravel.Tutorial.Editor
{
    [CustomEditor(typeof(TutorialController))]
    public class TutorialControllerEditor : UnityEditor.Editor 
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10f);
            if (GUILayout.Button("Skip first tutorial part"))
            {
                TutorialController tutorialController = (TutorialController) target;
                tutorialController.OnFirstPartCompleteEditor();
            }
        }
    }
}
