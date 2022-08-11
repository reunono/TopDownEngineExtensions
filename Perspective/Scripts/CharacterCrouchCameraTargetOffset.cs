using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterCrouchCameraTargetOffset : MonoBehaviour, MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>
{
    public Vector3 Value = new Vector3(0,-.75f,0);
    private Character _character;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> movementStateChangeEvent)
    {
        if (movementStateChangeEvent.Target != gameObject) return;
        if (IsCrouchingOrCrawling(movementStateChangeEvent.NewState))
            _character.CameraTarget.transform.position += Value;
        if (IsCrouchingOrCrawling(movementStateChangeEvent.PreviousState))
            _character.CameraTarget.transform.position -= Value;

        bool IsCrouchingOrCrawling(CharacterStates.MovementStates movementState)
        {
            return movementState == CharacterStates.MovementStates.Crouching || movementState == CharacterStates.MovementStates.Crawling;
        }
    }
}
