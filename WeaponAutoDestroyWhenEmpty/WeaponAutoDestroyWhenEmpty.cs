using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponAutoDestroyWhenEmpty : MonoBehaviour
{
    private Weapon[] _weapons;
    [SerializeField] private MMFeedbacks DestructionStartFeedbacks;
    private void Start()
    {
        _weapons = GetComponents<Weapon>();
        if (!DestructionStartFeedbacks) return;
        DestructionStartFeedbacks.Initialization();
        #if UNITY_EDITOR
        foreach (var weapon in _weapons)
            if (weapon.AutoDestroyWhenEmpty && weapon.AutoDestroyWhenEmptyDelay < DestructionStartFeedbacks.TotalDuration)
                Debug.LogWarning($"{name} {weapon.GetType().Name}'s Auto Destroy When Empty Delay is less than the Destruction Start Feedbacks total duration, {DestructionStartFeedbacks.TotalDuration:F}. Feedbacks may not play to the end", weapon);
        #endif
    }
    private void Update()
    {
        foreach (var weapon in _weapons)
            if (weapon.AutoDestroyWhenEmpty && weapon.CurrentAmmoLoaded < weapon.AmmoConsumedPerShot)
            {
                enabled = false;
                DestructionStartFeedbacks?.PlayFeedbacks(transform.position);
                StartCoroutine(weapon.WeaponDestruction());
                break;
            }
    }
}
