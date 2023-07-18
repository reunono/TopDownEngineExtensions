using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterMultiButtonActivation : CharacterButtonActivation
{
    private Collider _collider;
    private readonly List<ButtonActivated> _buttonActivated = new();
    protected override void Awake()
    {
        base.Awake();
        _collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter2D(Collider2D other) => OnEnter(other);
    private void OnTriggerExit2D(Collider2D other) => OnExit(other);
    private void OnTriggerEnter(Collider other) => OnEnter(other);
    private void OnTriggerExit(Collider other) => OnExit(other);
    private void OnEnter(Component other) => _buttonActivated.AddRange(other.GetComponents<ButtonActivated>());
    private void OnExit(Component other)
    {
        foreach (var buttonActivated in other.GetComponents<ButtonActivated>())
            _buttonActivated.Remove(buttonActivated);
    }

    protected override void HandleInput()
    {
        if (!AbilityAuthorized) return;
        if (_collider)
            for (var i=_buttonActivated.Count-1; i >= 0; i--)
            {
                var collider = _buttonActivated[i].GetComponent<Collider>();
                if (!collider || !collider.enabled || !collider.bounds.Intersects(_collider.bounds)) _buttonActivated.RemoveAt(i);
            }
        for (var i=_buttonActivated.Count-1; i >= 0; i--)
        {
            InButtonActivatedZone = true;
            ButtonActivatedZone = _buttonActivated[i];
            var buttonPressed = false;
            switch (ButtonActivatedZone.InputType)
            {
                case ButtonActivated.InputTypes.Default:
                    buttonPressed = _inputManager.InteractButton.State.CurrentState == MMInput.ButtonStates.ButtonDown;
                    break;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
					case ButtonActivated.InputTypes.Button:
					case ButtonActivated.InputTypes.Key:
						buttonPressed = ButtonActivatedZone.InputActionPerformed;
						break;
#else
                case ButtonActivated.InputTypes.Button:
                    buttonPressed = Input.GetButtonDown(_character.PlayerID + "_" + ButtonActivatedZone.InputButton);
                    break;
                case ButtonActivated.InputTypes.Key:
                    buttonPressed = Input.GetKeyDown(ButtonActivatedZone.InputKey);
                    break;
#endif
            }

            if (buttonPressed) ButtonActivation();
        }
    }
}
