using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IDropHandler
{

    [SerializeField] private UIInventoryItem _uiInventoryItem;
    public IInventorySlot Slot { get; private set; }

    private UIInventory _uiInventory;

    public void SetSlot(IInventorySlot slot)
    {
        Slot = slot;
    }

    private void Awake()
    {
        _uiInventory = GetComponentInParent<UIInventory>();
    }

    public  void OnDrop(PointerEventData eventData)
    {
        var otherItemUi = eventData.pointerDrag.GetComponent<UIInventoryItem>();
        otherItemUi.ResetParent();
        var otherInventory = otherItemUi.GetComponentInParent<UIInventory>();
        var otherSlotUi = otherItemUi.GetComponentInParent<UIInventorySlot>();
        var otherSlot = otherSlotUi.Slot;
        var inventory = _uiInventory.Inventory;
        Debug.Log($"slot : {Slot.Item?.Info.Id}, OtherSlot : {otherSlot.Item?.Info.Id}");
        if (otherInventory.Inventory.IsTraderInventory != _uiInventory.Inventory.IsTraderInventory)
        {
            inventory.TransitFromSlotToSlotInTrade(otherInventory.Inventory, otherSlot, Slot);
        }
        else if(otherSlot != this.Slot)
        {
            inventory.TransitFromSlotToSlot(this, otherSlot, Slot);
        }
        

        Refresh();
        otherSlotUi.Refresh();
    }

    public void Refresh()
    {
        if (Slot != null)
        {
            _uiInventoryItem.Refresh(Slot);
        }
    }
}
