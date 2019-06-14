
public class HeldButtonZone : KeyOperatedZone
{
    [SerializeField]
    protected float NecessaryTimeToHoldButton;
    protected float _buttonHeldTime;

    public override void TriggerButtonAction()
    {
        if (!CheckNumberOfUses())
        {
            return;
        }
        if (_collidingObject == null) { return; }
        if (RequiresKey)
        {
            CharacterInventory characterInventory = _collidingObject.GetComponentNoAlloc<CharacterInventory>();
            if (characterInventory == null)
            {
                PromptError();
                return;
            }
            _keyList.Clear();
            _keyList = characterInventory.MainInventory.InventoryContains(KeyID);
            if (_keyList.Count == 0)
            {
                PromptError();
                return;
            }
            else
            {
                if (NecessaryTimeToHoldButton <= _buttonHeldTime)
                {
                    _buttonHeldTime = 0;
                }
                else
                {
                    base.TriggerButtonAction();
                    characterInventory.MainInventory.UseItem(KeyID);
                }
            }
        }
        if (NecessaryTimeToHoldButton <= _buttonHeldTime)
        {
            TriggerKeyAction();
            ActivateZone();
        }
    }

//New class added to ButtonActivatedZone and called in the CharacterAbility when a button is activated and is still  pressed afterwards
    public override void TriggerButtonHeld()
    {
        if (!CheckNumberOfUses())
        {
            return;
        }
        _buttonHeldTime += Time.deltaTime;
        _buttonPrompt.SetBackgroundFill(_buttonHeldTime / NecessaryTimeToHoldButton);
        if (NecessaryTimeToHoldButton <= _buttonHeldTime)
        {
            TriggerButtonAction();
            _buttonHeldTime = 0;
        }
    }
    /// <summary>
    /// Activates the zone
    /// </summary>
    protected override void ActivateZone()
    {
        _lastActivationTimestamp = Time.time;
        ActivationFeedback?.PlayFeedbacks(this.transform.position);
        if (NecessaryTimeToHoldButton > _buttonHeldTime)
        {
            return;
        }
        if (HidePromptAfterUse)
        {
            _promptHiddenForever = true;
            HidePrompt();
        }
        _numberOfActivationsLeft--;
    }
}
