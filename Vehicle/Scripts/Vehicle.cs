using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Events;

public class Vehicle : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Suspension[] _suspensions;
    private VehicleController _vehicleController;
    private int _initialLayer;
    private GameObject _player;
    public UnityEvent OnEnableEvent;
    public UnityEvent OnDisableEvent;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _suspensions = GetComponentsInChildren<Suspension>(true);
        _vehicleController = GetComponent<VehicleController>();
        _initialLayer = gameObject.layer;
    }

    private void OnEnable()
    {
        OnEnableEvent.Invoke();
        _player = LevelManager.Instance.Players[0].gameObject;
        _player.SetActive(false);
        _player.transform.parent = transform;
        gameObject.layer = _player.layer;
        _rigidbody.isKinematic = false;
        foreach (var suspension in _suspensions)
            suspension.gameObject.SetActive(true);
        _vehicleController.enabled = true;
    }
    
    private void OnDisable()
    {
        OnDisableEvent.Invoke();
        gameObject.layer = _initialLayer;
        _rigidbody.isKinematic = true;
        foreach (var suspension in _suspensions)
            suspension.gameObject.SetActive(false);
        _vehicleController.enabled = false;
        _player.transform.parent = null;
        _player.transform.rotation = Quaternion.identity;
        _player.SetActive(true);
    }
    
    private void Update()
    {
        if (Input.GetButtonDown("Player1_Interact")) enabled = false;
    }
}
