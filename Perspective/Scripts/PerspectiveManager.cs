using System;
using MoreMountains.Feedbacks;
using Unity.Cinemachine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class PerspectiveManager : MonoBehaviour, MMEventListener<PerspectiveChangeEvent>
{
    public static PerspectiveManager Instance;
    public Perspectives InitialPerspective = Perspectives.TopDown;
    public CinemachineCamera FirstPersonVirtualCamera;
    public CinemachineCamera ThirdPersonVirtualCamera;
    public CinemachineThirdPersonFollow ThirdPersonFollow;
    [Serializable] public class CameraTargetSettings
    {
        public Vector3 CameraTargetOffset = new(0,2.5f,0);
        public Vector3 CrouchCameraTargetOffset = new(0,1.5f,0);
        public Vector2 MouseSensitivity = new(.3f, .3f);
        public Vector2 StickSensitivity = new(1f, 1f);
        public bool InvertMouseVertical;
        public bool InvertMouseHorizontal;
        public bool InvertStickVertical;
        public bool InvertStickHorizontal;
        [MMVector("Min", "Max")] public Vector2 VerticalClamp = new(-30, 30);
    }
    public CameraTargetSettings cameraTargetSettings;
    public float LerpSpeed = .1f;
    public bool LerpTimeScale = true;
    public float TimeScaleValue = .1f;
    public float TimeScaleDuration = 1f;
    private bool _start = true;
    [NonSerialized] public Perspectives Perspective;
    private PerspectiveInput _perspectiveInput;
    private float _targetCameraSide = 1;
    public float ShoulderSwitchSpeed = 1;

    private void Awake()
    {
        Instance = this;
        _perspectiveInput = new PerspectiveInput();
        _perspectiveInput.Enable();
        _targetCameraSide = ThirdPersonFollow.CameraSide;
    }

    private void Start()
    {
        PerspectiveChangeEvent.Trigger(InitialPerspective);
        _start = false;
    }

    private void Update()
    {
        if (Perspective == Perspectives.ThirdPerson && _perspectiveInput.Perspective.SwitchShoulder.triggered)
            _targetCameraSide = 1 - _targetCameraSide;
        ThirdPersonFollow.CameraSide = MMMaths.Lerp(ThirdPersonFollow.CameraSide, _targetCameraSide, ShoulderSwitchSpeed, Time.deltaTime);
    }

    public void OnMMEvent(PerspectiveChangeEvent perspectiveChangeEvent) => ChangePerspective(perspectiveChangeEvent.NewPerspective);

    private void ChangePerspective(Perspectives newPerspective)
    {
        Perspective = newPerspective;
        var player = LevelManager.Instance.Players[0];
        var cameraTarget = player.CameraTarget.transform;
        cameraTarget.rotation = Quaternion.identity;
        if (!cameraTarget.TryGetComponent<CameraTargetController>(out _)) player.CameraTarget.AddComponent<CameraTargetController>().Initialize(cameraTargetSettings, cameraTarget, player.MovementState, _perspectiveInput);

        FirstPersonVirtualCamera.gameObject.SetActive(Perspective == Perspectives.FirstPerson);
        ThirdPersonVirtualCamera.gameObject.SetActive(Perspective == Perspectives.ThirdPerson);
        if (TimeScaleDuration <= 0 || _start) return;
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeScaleValue, TimeScaleDuration, LerpTimeScale, LerpSpeed, false);
    }

    public void TogglePerspective()
    {
        PerspectiveChangeEvent.Trigger(Perspective switch
        {
            Perspectives.FirstPerson => Perspectives.TopDown,
            Perspectives.TopDown => Perspectives.ThirdPerson,
            _ => Perspectives.FirstPerson
        });
    }
    
    private void OnEnable() => this.MMEventStartListening();
    private void OnDisable() => this.MMEventStopListening();

    private void OnValidate()
    {
        cameraTargetSettings.MouseSensitivity.x = cameraTargetSettings.MouseSensitivity.x < .0001f ? .0001f : cameraTargetSettings.MouseSensitivity.x;
        cameraTargetSettings.MouseSensitivity.y = cameraTargetSettings.MouseSensitivity.y < .0001f ? .0001f : cameraTargetSettings.MouseSensitivity.y;
        cameraTargetSettings.StickSensitivity.x = cameraTargetSettings.StickSensitivity.x < .0001f ? .0001f : cameraTargetSettings.StickSensitivity.x;
        cameraTargetSettings.StickSensitivity.y = cameraTargetSettings.StickSensitivity.y < .0001f ? .0001f : cameraTargetSettings.StickSensitivity.y;
    }
}
public enum Perspectives { TopDown, FirstPerson, ThirdPerson }
public struct PerspectiveChangeEvent
{
    public Perspectives NewPerspective;
    static PerspectiveChangeEvent e;
    public static void Trigger(Perspectives newPerspective)
    {
        e.NewPerspective = newPerspective;
        MMEventManager.TriggerEvent(e);
    }
}
