using MoreMountains.Feedbacks;
using Unity.Cinemachine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class PerspectiveManager : MonoBehaviour, MMEventListener<PerspectiveChangeEvent>
{
    public Perspectives InitialPerspective = Perspectives.TopDown;
    public CinemachineCamera FirstPersonVirtualCamera;
    public CinemachinePanTilt FirstPersonPanTilt;
    public float LerpSpeed = 1f;
    public bool LerpTimeScale = true;
    public float TimeScaleValue = .1f;
    public float TimeScaleDuration = 1f;
    private bool _start = true;

    private void Start()
    {
        PerspectiveChangeEvent.Trigger(InitialPerspective);
        _start = false;
    }

    public void OnMMEvent(PerspectiveChangeEvent perspectiveChangeEvent) => ChangePerspective(perspectiveChangeEvent.NewPerspective);

    private void ChangePerspective(Perspectives newPerspective)
    {
        if (newPerspective == Perspectives.FirstPerson)
        {
            FirstPersonPanTilt.PanAxis.Value = LevelManager.Instance.Players[0].CharacterModel.transform.rotation.eulerAngles.y;
            FirstPersonPanTilt.TiltAxis.Value = 0;
        }

        FirstPersonVirtualCamera.gameObject.SetActive(newPerspective == Perspectives.FirstPerson);
        if (TimeScaleDuration <= 0 || _start) return;
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeScaleValue, TimeScaleDuration, LerpTimeScale, LerpSpeed, false);
    }

    public void TogglePerspective()
    {
        PerspectiveChangeEvent.Trigger(FirstPersonVirtualCamera.isActiveAndEnabled
            ? Perspectives.TopDown
            : Perspectives.FirstPerson);
    }
    
    private void OnEnable() => this.MMEventStartListening();
    private void OnDisable() => this.MMEventStopListening();
}
