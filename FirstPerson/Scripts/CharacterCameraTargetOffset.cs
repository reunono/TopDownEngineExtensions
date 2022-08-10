using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterCameraTargetOffset : MonoBehaviour
{
    public Vector3 Value = new Vector3(0,2.5f,0);
    private Character _character;
    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void Start()
    {
        _character.CameraTarget.transform.localPosition = Value;
    }
}
