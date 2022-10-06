using System.Linq;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class MultipleKeyOperatedZone : KeyOperatedZone
{
    public int NumberOfKeysRequired = 10;
    public bool RemoveKeys = true;
    
    public override void TriggerButtonAction()
    {
        if (!CheckNumberOfUses())
        {
            PromptError();
            return;
        }

        if (_collidingObject == null) { return; }

        if (RequiresKey)
        {
            CharacterInventory characterInventory = _collidingObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterInventory> ();
            if (characterInventory == null)
            {
                PromptError();
                return;
            }	

            _keyList.Clear ();
            _keyList = characterInventory.MainInventory.InventoryContains (KeyID);
            var numberOfKeysInInventory = _keyList.Sum(index => characterInventory.MainInventory.Content[index].Quantity);
            if (numberOfKeysInInventory < NumberOfKeysRequired)
            {
                PromptError();
                return;
            }
            else
            {
                base.TriggerButtonAction ();
                if (RemoveKeys) characterInventory.MainInventory.RemoveItemByID(KeyID, NumberOfKeysRequired);
            }
        }

        TriggerKeyAction ();
        ActivateZone ();
    }
}
