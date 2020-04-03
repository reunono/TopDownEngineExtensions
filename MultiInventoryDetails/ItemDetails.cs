using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MoreMountains.Tools;

namespace MoreMountains.InventoryEngine
{
    /// <summary>
    /// A class used to display an item's details in GUI
    /// </summary>
    public class ItemDetails : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        /// the reference inventory from which we'll display item details
		[MMInformation("Specify here the name of the inventory whose content's details you want to display in this Details panel. This field will be used only if Multi Inventory option is set to <false>", MMInformationAttribute.InformationType.Info, false)]
        public string TargetInventoryName;
        
		[MMInformation("If <true> then this display will handle every active inventory you have on scene.", MMInformationAttribute.InformationType.Info, false)]
        public bool multiInventory = true;

        [MMInformation("If MultiInventory is <true> those inventories won't be handled. Put here exact Inventory Name you want to disable. Case sensitive", MMInformationAttribute.InformationType.Info, false)]
        public string[] disabledInventories;
        /// whether the details are currently hidden or not 
        public bool Hidden { get; protected set; }

        [Header("Default")]
        [MMInformation("By checking HideOnEmptySlot, the Details panel won't be displayed if you select an empty slot.", MMInformationAttribute.InformationType.Info, false)]
        /// whether or not the details panel should be hidden when the currently selected slot is empty
        public bool HideOnEmptySlot = true;
        [MMInformation("Here you can set default values for all fields of the details panel. These values will be displayed when no item is selected (and if you've chosen not to hide the panel in that case).", MMInformationAttribute.InformationType.Info, false)]
        /// the title to display when none is provided
        public string DefaultTitle;
        /// the short description to display when none is provided
        public string DefaultShortDescription;
        /// the description to display when none is provided
        public string DefaultDescription;
        /// the quantity to display when none is provided
        public string DefaultQuantity;
        /// the icon to display when none is provided
        public Sprite DefaultIcon;

        [Header("Behaviour")]
        [MMInformation("Here you can decide whether or not to hide the details panel on start.", MMInformationAttribute.InformationType.Info, false)]
        public bool HideOnStart = true;

        [Header("Components")]
        [MMInformation("Here you need to bind the panel components.", MMInformationAttribute.InformationType.Info, false)]
        /// the icon container object
        public Image Icon;
        /// the title container object
        public Text Title;
        /// the short description container object
        public Text ShortDescription;
        /// the description container object
        public Text Description;
        /// the quantity container object
        public Text Quantity;

        protected float _fadeDelay = 0.2f;
        protected CanvasGroup _canvasGroup;

        /// <summary>
        /// On Start, we grab and store the canvas group and determine our current Hidden status
        /// </summary>
        protected virtual void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (HideOnStart)
            {
                _canvasGroup.alpha = 0;
            }

            if (_canvasGroup.alpha == 0)
            {
                Hidden = true;
            }
            else
            {
                Hidden = false;
            }
        }

        /// <summary>
        /// Starts the display coroutine or the panel's fade depending on whether or not the current slot is empty
        /// </summary>
        /// <param name="item">Item.</param>
        public virtual void DisplayDetails(InventoryItem item)
        {
            if (InventoryItem.IsNull(item))
            {
                if (HideOnEmptySlot && !Hidden)
                {
                    StartCoroutine(MMFade.FadeCanvasGroup(_canvasGroup, _fadeDelay, 0f));
                    Hidden = true;
                }
                if (!HideOnEmptySlot)
                {
                    StartCoroutine(FillDetailFieldsWithDefaults(0));
                }
            }
            else
            {
                StartCoroutine(FillDetailFields(item, 0f));

                if (HideOnEmptySlot && Hidden)
                {
                    StartCoroutine(MMFade.FadeCanvasGroup(_canvasGroup, _fadeDelay, 1f));
                    Hidden = false;
                }
            }
        }

        /// <summary>
        /// Fills the various detail fields with the item's metadata
        /// </summary>
        /// <returns>The detail fields.</returns>
        /// <param name="item">Item.</param>
        /// <param name="initialDelay">Initial delay.</param>
        protected virtual IEnumerator FillDetailFields(InventoryItem item, float initialDelay)
        {
            yield return new WaitForSeconds(initialDelay);
            if (Title != null) { Title.text = item.ItemName; }
            if (ShortDescription != null) { ShortDescription.text = item.ShortDescription; }
            if (Description != null) { Description.text = item.Description; }
            if (Quantity != null) { Quantity.text = item.Quantity.ToString(); }
            if (Icon != null) { Icon.sprite = item.Icon; }
        }

        /// <summary>
        /// Fills the detail fields with default values.
        /// </summary>
        /// <returns>The detail fields with defaults.</returns>
        /// <param name="initialDelay">Initial delay.</param>
        protected virtual IEnumerator FillDetailFieldsWithDefaults(float initialDelay)
        {
            yield return new WaitForSeconds(initialDelay);
            if (Title != null) { Title.text = DefaultTitle; }
            if (ShortDescription != null) { ShortDescription.text = DefaultShortDescription; }
            if (Description != null) { Description.text = DefaultDescription; }
            if (Quantity != null) { Quantity.text = DefaultQuantity; }
            if (Icon != null) { Icon.sprite = DefaultIcon; }
        }

        /// <summary>
        /// Catches MMInventoryEvents and displays details if needed
        /// </summary>
        /// <param name="inventoryEvent">Inventory event.</param>
        public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
        {
            if(multiInventory == false){
                // if this event doesn't concern our inventory display, we do nothing and exit
                if (inventoryEvent.TargetInventoryName != this.TargetInventoryName)
                {
                    return;
                }
            }
            else
            {
                foreach(string disabledInv in disabledInventories)
                {
                    if (inventoryEvent.TargetInventoryName == disabledInv)
                    {
                        return;
                    }
                }
                
            }
            
            switch (inventoryEvent.InventoryEventType)
            {
                case MMInventoryEventType.Select:
                    DisplayDetails(inventoryEvent.EventItem);
                    break;
                case MMInventoryEventType.InventoryOpens:
                    DisplayDetails(inventoryEvent.EventItem);
                    break;
            }
        }

        /// <summary>
        /// On Enable, we start listening for MMInventoryEvents
        /// </summary>
        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();
        }

        /// <summary>
        /// On Disable, we stop listening for MMInventoryEvents
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();
        }
    }
}