using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIInventoryTester
{
    private int inventoryCapacity = 16;
    private float goldAmount = 400;
    private InventoryItemInfo _healingElixirInfo;
    private InventoryItemInfo _axeInfo;
    private UIInventorySlot[] _uiSlots;
    public InventoryWithSlots Inventory { get; }

    public UIInventoryTester(InventoryItemInfo healingElixirInfo, InventoryItemInfo axeInfo, UIInventorySlot[] uiSlots, bool isTrader = false)
    {
        _healingElixirInfo = healingElixirInfo;
        _axeInfo = axeInfo;
        _uiSlots = uiSlots;
        Inventory = new InventoryWithSlots(inventoryCapacity, goldAmount, isTrader);
        Inventory.OnInventoryStateChangedEvent += OnInventoryStateChanged;
    }

    private void OnInventoryStateChanged(object sender)
    {
        foreach (var uislot in _uiSlots)
        {
            uislot.Refresh();
        }
    }

    private IInventorySlot AddRandomHealingElixirsIntoRandomSlots(List<IInventorySlot> slots)
    {
        var rSlotIndex = Random.Range(0, slots.Count-1);
        var rSlot = slots[rSlotIndex];
        var rCount = Random.Range(3, 5);
        var healingElixir = new HealingElixir(_healingElixirInfo);
        healingElixir.State.Amount = rCount;
        Inventory.TryAddToSlot(this, rSlot, healingElixir);

        return rSlot;

    }

    private IInventorySlot AddRandomAxesIntoRandomSlots(List<IInventorySlot> slots)
    {
        var rSlotIndex = Random.Range(0, slots.Count);
        var rSlot = slots[rSlotIndex];
        var rCount = Random.Range(2, 4);
        var axe = new Axe(_axeInfo);
        axe.State.Amount = rCount;
        Inventory.TryAddToSlot(this, rSlot, axe);

        return rSlot;

    }

    public void FillSlots()
    {
        var allSlots = Inventory.GetAllSlots();
        var availableSlots = new List<IInventorySlot>(allSlots);
        var filledSlots = 7;
        for (int i = 0; i < filledSlots; i++)
        {
            var fieldSlot = AddRandomHealingElixirsIntoRandomSlots(availableSlots);
            availableSlots.Remove(fieldSlot);

            fieldSlot = AddRandomAxesIntoRandomSlots(availableSlots);
            availableSlots.Remove(fieldSlot);

        }
        SetupInventoryUI(Inventory);
    }

    private void SetupInventoryUI(InventoryWithSlots inventory)
    {
        var allSlots = inventory.GetAllSlots();
        var allSlotsCount = allSlots.Length;
        for (int i = 0; i < allSlotsCount; i++)
        {
            var slot = allSlots[i];
            var uiSlot = _uiSlots[i];
            uiSlot.SetSlot(slot);
            uiSlot.Refresh();
        }
    }
}
