using System.Linq;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NegatableDecision), true)]
public class NegatableDecisionDrawer : PropertyDrawer
{
	private const float _lineHeight = 16f;

	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		var targetMonoBehaviour = (MonoBehaviour)property.serializedObject.targetObject;

		var brain = targetMonoBehaviour.GetComponentInParent<AIBrain>();
		var decisions = (brain ?
				brain.GetAttachedDecisions() :
				targetMonoBehaviour.transform.root.GetComponentsInChildren<AIDecision>())
			.Where(decision => decision != targetMonoBehaviour).ToArray();
		var selected = 0;
		var decisionProperty = property.FindPropertyRelative("Decision");
		var currentDecision = decisionProperty.objectReferenceValue;
		var options = new string[decisions.Length + 1];
		options[0] = "None";
		for (var i = 1; i <= decisions.Length; i++)
		{
			var decision = decisions[i-1];
			var name = decision.Label;
			if (string.IsNullOrWhiteSpace(name))
			{
				var decisionName = decision.GetType().Name[10..];
				name = new string(decisionName.SelectMany((c, j) => j != 0 && char.IsUpper(c) && !char.IsUpper(decisionName[j - 1]) ? new[] { ' ', c } : new[] { c }).ToArray());
			}
			options[i] = $"{i} - {name}";
			if (decision == currentDecision) selected = i;
		}

		EditorGUI.BeginChangeCheck();
		var position = rect;
		position.height = _lineHeight;
		var totalWidth = rect.width;
		const int negateWidth = 64;
		position.width = negateWidth;
		var negateProperty = property.FindPropertyRelative("Negate");
		var negate = EditorGUI.ToggleLeft(position, "Negate", negateProperty.boolValue);
		position.width = totalWidth - negateWidth;
		position.x += negateWidth;
		selected = EditorGUI.Popup(position, selected, options);
		if (!EditorGUI.EndChangeCheck()) return;
		decisionProperty.objectReferenceValue = selected > 0 ? decisions[selected - 1] : null;
		negateProperty.boolValue = negate;
		property.serializedObject.ApplyModifiedProperties();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => _lineHeight;
}
