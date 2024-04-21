using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public static class CharacterOrientation3DExtensions
{
    private const float _defaultDuration = 1;
    private static readonly Dictionary<CharacterOrientation3D, Coroutine> Coroutines = new Dictionary<CharacterOrientation3D, Coroutine>();
    private static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    private static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
    public static void LookAt(this CharacterOrientation3D orientation, Transform target)
    {
        orientation.StopLookAt();
        orientation.StartCoroutine(LookAt());

        IEnumerator LookAt()
        {
            orientation.SetForcedOrientation(true);
            orientation.ForcedRotationDirection = (target.position - orientation.transform.position).MMSetY(0);
            yield return WaitForEndOfFrame;
            orientation.SetForcedOrientation(false);
        }
    }
    public static void FixedUpdateLookAt(this CharacterOrientation3D orientation, Transform target)
    {
        orientation.StopLookAt();
        orientation.StartCoroutine(LookAt());

        IEnumerator LookAt()
        {
            yield return WaitForFixedUpdate;
            orientation.SetForcedOrientation(true);
            orientation.ForcedRotationDirection = (target.position - orientation.transform.position).MMSetY(0);
            yield return WaitForFixedUpdate;
            orientation.SetForcedOrientation(false);
        }
    }
    public static void LookAtForSeconds(this CharacterOrientation3D orientation, Transform target, float duration = _defaultDuration)
    {
        var end = Time.time + duration;
        orientation.StartCoroutine(MakeForceLookAtWhile(target, () => Time.time < end));
    }
    public static void LookAtWhile(this CharacterOrientation3D orientation, Transform target, Func<bool> condition) => orientation.StartCoroutine(MakeForceLookAtWhile(target, condition));
    public static void LookAtUntil(this CharacterOrientation3D orientation, Transform target, Func<bool> condition) => orientation.StartCoroutine(MakeForceLookAtWhile(target, () => !condition()));
    public static void StartLookAt(this CharacterOrientation3D orientation, Transform target) => orientation.StartCoroutine(MakeForceLookAtWhile(target, () => true));
    public static void StopLookAt(this CharacterOrientation3D orientation){
        orientation.SetForcedOrientation(false);
        if (Coroutines.TryGetValue(orientation, out var coroutine) && coroutine != null) orientation.StopCoroutine(coroutine);
    }
    public static void StartCoroutine(this CharacterOrientation3D orientation, Func<CharacterOrientation3D, IEnumerator> routine)
    {
        orientation.StopLookAt();
        Coroutines[orientation] = orientation.StartCoroutine(routine(orientation));
    }
    public static Func<CharacterOrientation3D, IEnumerator> MakeForceLookAtWhile(Transform target, Func<bool> condition) => orientation => ForceLookAtWhile(orientation, target, condition);
    public static void LookAt(this Character character, Transform target) => character.FindAbility<CharacterOrientation3D>().LookAt(target);
    public static void FixedUpdateLookAt(this Character character, Transform target) => character.FindAbility<CharacterOrientation3D>().FixedUpdateLookAt(target);
    public static void LookAtForSeconds(this Character character, Transform target, float duration = _defaultDuration) => character.FindAbility<CharacterOrientation3D>().LookAtForSeconds(target, duration);
    public static void LookAtWhile(this Character character, Transform target, Func<bool> condition) => character.FindAbility<CharacterOrientation3D>().LookAtWhile(target, condition);
    public static void LookAtUntil(this Character character, Transform target, Func<bool> condition) => character.FindAbility<CharacterOrientation3D>().LookAtUntil(target, condition);
    public static void StartLookAt(this Character character, Transform target) => character.FindAbility<CharacterOrientation3D>().StartLookAt(target);
    public static void StopLookAt(this Character character) => character.FindAbility<CharacterOrientation3D>().StopLookAt();
    public static void SetForcedOrientation(this CharacterOrientation3D orientation, bool value)
    {
        orientation.ForcedRotation = value;
        orientation.ShouldRotateToFaceWeaponDirection = !value;
    }
    private static IEnumerator ForceLookAtWhile(CharacterOrientation3D orientation, Transform target, Func<bool> condition)
    {
        orientation.SetForcedOrientation(true);
        while (condition())
        {
            orientation.ForcedRotationDirection = (target.position - orientation.transform.position).MMSetY(0);
            yield return null;
        }
        orientation.SetForcedOrientation(false);
    }
}
