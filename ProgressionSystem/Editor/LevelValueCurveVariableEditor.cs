using System.Globalization;
using ProgressionSystem.Scripts.Variables;
using UnityEditor;
using UnityEngine;

namespace ProgressionSystem.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LevelValueCurveVariable))]
    public class LevelValueCurveVariableEditor : UnityEditor.Editor
    {
        private Vector2 _scroll;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MaxHeight(600));
            var levelValueCurve = (LevelValueCurveVariable)target;
            for (var i = levelValueCurve.MinLevel; i <= levelValueCurve.MaxLevel; i++)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField("Level " + i);
                EditorGUILayout.LabelField((levelValueCurve.DisplayIntValues ? levelValueCurve.EvaluateInt(i).ToString() : levelValueCurve.Evaluate(i).ToString("F", CultureInfo.InvariantCulture)) + " " + levelValueCurve.ValueRepresents);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
    }
}
