using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEditor;
using UnityEngine;

public class SaveIKHandlesPositionInPlayMode : MonoBehaviour
{
	[HideInInspector] public string Prefab;
	[HideInInspector] public string Name;
#if UNITY_EDITOR
	private void Reset() => StorePath();
	private void OnValidate() => StorePath();
	private Transform _leftHandHandle;
	private Transform _rightHandHandle;
	private void Awake() => GetHandles(transform, ref _leftHandHandle, ref _rightHandHandle);
	private void StorePath()
	{
		if (!PrefabUtility.IsPartOfPrefabAsset(gameObject) || string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(gameObject))) return;
		Prefab = AssetDatabase.GetAssetPath(gameObject);
		Name = name;
		EditorUtility.SetDirty(gameObject);
		PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
	}
	private void OnDisable()
	{
		var prefab = AssetDatabase.LoadAssetAtPath<Transform>(Prefab);
		Component weapon = prefab.GetComponentsInChildren<WeaponModel>().FirstOrDefault(w => w.name == Name);
		if (!weapon) weapon = prefab.GetComponentsInChildren<Weapon>().FirstOrDefault(w => w.name == Name);
		Transform left = null, right = null;
		GetHandles(weapon.transform, ref left, ref right);
		if (left)
		{
			_leftHandHandle.GetLocalPositionAndRotation(out var localPosition, out var localRotation);
			left.SetLocalPositionAndRotation(localPosition, localRotation);
		}
		if (right)
		{
			_rightHandHandle.GetLocalPositionAndRotation(out var localPosition, out var localRotation);
			right.SetLocalPositionAndRotation(localPosition, localRotation);
		}
		if (!prefab) return;
		EditorUtility.SetDirty(prefab);
		PrefabUtility.RecordPrefabInstancePropertyModifications(prefab);
	}
#endif
	private void GetHandles(Transform target, ref Transform leftHandle, ref Transform rightHandle)
	{
		var model = target.GetComponent<WeaponModel>();
		if (model)
		{
			leftHandle = model.LeftHandHandle;
			rightHandle = model.RightHandHandle;
			return;
		}
		var weapon = target.GetComponent<Weapon>();
		if (!weapon) return;
		leftHandle = weapon.LeftHandHandle;
		rightHandle = weapon.RightHandHandle;
	}
}
