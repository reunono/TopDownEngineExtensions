using MoreMountains.TopDownEngine;
using UnityEngine;

public class UnparentAndRotateToMatchWeapon : MonoBehaviour
{
    private CharacterHandleWeapon _characterHandleWeapon;

    private void Awake()
    {
        _characterHandleWeapon = GetComponentInParent<Character>().FindAbility<CharacterHandleWeapon>();
        transform.parent = null;
    }

    private void Update()
    {
        if (_characterHandleWeapon.CurrentWeapon)
            transform.rotation = Quaternion.Euler(_characterHandleWeapon.CurrentWeapon.transform.rotation.eulerAngles.y * Vector3.up);
    }
}
