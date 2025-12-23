using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    private PerspectiveManager.CameraTargetSettings _settings;
    private Transform _cameraTarget;
    private MMStateMachine<CharacterStates.MovementStates> _movementStateMachine;
    private PerspectiveInput _perspectiveInput;
    private float _verticalRotation;

    private void Update()
    {
        if (Time.timeScale == 0) return;
        _cameraTarget.localPosition = _movementStateMachine.CurrentState is CharacterStates.MovementStates.Crouching or CharacterStates.MovementStates.Crawling ? _settings.CrouchCameraTargetOffset : _settings.CameraTargetOffset;
        if (_cameraTarget.rotation == Quaternion.identity) _verticalRotation = 0;
        var lookInput = _perspectiveInput.Perspective.Look.ReadValue<Vector2>();
        var mouse = Input.mousePositionDelta != Vector3.zero;
        var sensitivity = mouse ? _settings.MouseSensitivity : _settings.StickSensitivity;
        var invertHorizontal = mouse ? _settings.InvertMouseHorizontal : _settings.InvertStickHorizontal;
        var invertVertical = mouse ? _settings.InvertMouseVertical : _settings.InvertStickVertical;
        var lookX = lookInput.x * sensitivity.x * (invertHorizontal ? -1 : 1);
        var lookY = lookInput.y * sensitivity.y * (invertVertical ? -1 : 1);

        _cameraTarget.Rotate(lookX * Vector3.up);

        _verticalRotation = Mathf.Clamp(_verticalRotation - lookY, _settings.VerticalClamp.x, _settings.VerticalClamp.y);
        _cameraTarget.transform.localRotation = Quaternion.Euler(_verticalRotation, _cameraTarget.transform.eulerAngles.y, 0f);
    }

    public void Initialize(PerspectiveManager.CameraTargetSettings settings, Transform cameraTarget, MMStateMachine<CharacterStates.MovementStates> movementStateMachine, PerspectiveInput perspectiveInput)
    {
        _settings = settings;
        _cameraTarget = cameraTarget;
        _movementStateMachine = movementStateMachine;
        _perspectiveInput = perspectiveInput;
    }
}
