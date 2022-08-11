using MoreMountains.Tools;
using UnityEngine;

public class PerspectiveModel : MonoBehaviour, MMEventListener<PerspectiveChangeEvent>
{
    public GameObject[] TopDownGameObjects;
    public float DelayBeforeDeactivation = 2;
    private Perspectives _perspective;
    private float _deactivationTime;

    private void Awake()
    {
        this.MMEventStartListening();
        enabled = false;
    }

    private void OnDestroy()
    {
        this.MMEventStopListening();
    }
    
    public void OnMMEvent(PerspectiveChangeEvent perspectiveChangeEvent)
    {
        var newPerspective = perspectiveChangeEvent.NewPerspective;
        if (_perspective == newPerspective) return;
        _perspective = newPerspective;
        if (_perspective == Perspectives.FirstPerson)
        {
            _deactivationTime = Time.time + DelayBeforeDeactivation;
            enabled = true;
        }
        else
        {
            foreach (var gameObjectToDeactivate in TopDownGameObjects)
                gameObjectToDeactivate.SetActive(true);
            enabled = false;
        }
    }

    private void Update()
    {
        if (Time.time < _deactivationTime) return;
        foreach (var gameObjectToDeactivate in TopDownGameObjects)
            gameObjectToDeactivate.SetActive(false);
        enabled = false;
    }
}
