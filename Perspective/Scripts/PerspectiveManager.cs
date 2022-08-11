using Cinemachine;
using MoreMountains.Tools;
using UnityEngine;

public class PerspectiveManager : MonoBehaviour, MMEventListener<PerspectiveChangeEvent>
{
    public Perspectives InitialPerspective = Perspectives.TopDown;
    public CinemachineVirtualCamera TopDownVirtualCamera;
    public CinemachineVirtualCamera FirstPersonVirtualCamera;

    private void Start()
    {
        PerspectiveChangeEvent.Trigger(InitialPerspective);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && TopDownVirtualCamera != null && FirstPersonVirtualCamera != null)
            ChangePerspective(InitialPerspective);
    }

    public void OnMMEvent(PerspectiveChangeEvent perspectiveChangeEvent)
    {
        ChangePerspective(perspectiveChangeEvent.NewPerspective);
    }

    private void ChangePerspective(Perspectives newPerspective)
    {
        FirstPersonVirtualCamera.enabled = false;
        TopDownVirtualCamera.enabled = false;
        if (newPerspective == Perspectives.FirstPerson)
            FirstPersonVirtualCamera.enabled = true;
        else
            TopDownVirtualCamera.enabled = true;
    }

    public void TogglePerspective()
    {
        PerspectiveChangeEvent.Trigger(FirstPersonVirtualCamera.enabled
            ? Perspectives.TopDown
            : Perspectives.FirstPerson);
    }
    
    private void OnEnable()
    {
        this.MMEventStartListening();
    }
    
    private void OnDisable()
    {
        this.MMEventStopListening();
    }
}
