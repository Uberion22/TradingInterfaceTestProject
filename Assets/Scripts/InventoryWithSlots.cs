using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryWithSlots : IInventory
{
    public event Action<object, IInventoryItem, int> OnInventoryItemAddedEvent;
    public event Action<object, Type, int> OnInventoryItemRemovedEvent;
    public event Action<object> OnInventoryStateChangedEvent;
    public int Capacity { get; set; }
    public bool IsTrader;
    public bool IsFull => _slots.All(s => IsFull);
    private List<IInventorySlot> _slots;
    public float GoldAmount;
    public InventoryWithSlots(int capacity, float goldAmount, bool isTrader = false)
    {
        Capacity = capacity;
        _slots = new List<IInventorySlot>();
        IsTrader = isTrader;
        GoldAmount = goldAmount;
        for (int i = 0; i < Capacity; i++)
        {
            _slots.Add(new InventorySlot());
        }
    }

    public void TransitFromSlotToSlot(object sender, IInventorySlot fromSlot, IInventorySlot toSlot)
    {
        if(fromSlot.IsEmpty || toSlot.IsFool || fromSlot == toSlot) return;

        if(!toSlot.IsEmpty && toSlot.ItemType != fromSlot.ItemType ) return;

        var slotCapacity = fromSlot.Capacity;
        var fits = fromSlot.Amount + toSlot.Amount <= slotCapacity;
        var amountToAdd = fits
            ? fromSlot.Amount
            : slotCapacity - toSlot.Amount;
        var amountLeft = fromSlot.Amount - amountToAdd;

        if (toSlot.IsEmpty)
        {
            toSlot.SetItem(fromSlot.Item);
            fromSlot.Clear();
            OnInventoryStateChangedEvent?.Invoke(sender);
        }

        toSlot.Item.State.Amount += amountToAdd;
        if (fits)
        {
            fromSlot.Clear();
        }
        else
        {
            fromSlot.Item.State.Amount = amountLeft;
        }
        OnInventoryStateChangedEvent?.Invoke(sender);
    }

    //public void TransitFromSlotToSlotInTrade(object sender, InventoryWithSlots fromInventory, InventoryWithSlots toInventory, IInventorySlot fromSlot, IInventorySlot toSlot)
    public void TransitFromSlotToSlotInTrade(object sender, IInventorySlot fromSlot, IInventorySlot toSlot)
    {
        if (fromSlot.IsEmpty || toSlot.IsFool) return;

        if (!toSlot.IsEmpty && toSlot.ItemType != fromSlot.ItemType) return;

        var slotCapacity = fromSlot.Capacity;
        var fits = fromSlot.Amount + toSlot.Amount <= slotCapacity;
        var amountToAdd = fits
            ? fromSlot.Amount
            : slotCapacity - toSlot.Amount;
        var amountLeft = fromSlot.Amount - amountToAdd;

        //toInventory.RemoveGold(fromSlot.Item);
        //fromInventory.AddGold(fromSlot.Item);
        if(!TryRemoveGold(fromSlot.Item)) return;
        (sender as InventoryWithSlots).AddGold(fromSlot.Item);
        Debug.LogWarning("Trade");
        if (toSlot.IsEmpty)
        {
            //toInventory.RemoveGold(fromSlot.Item);
            //fromInventory.AddGold(fromSlot.Item);
            toSlot.SetItem(fromSlot.Item);
            fromSlot.Clear();
            OnInventoryStateChangedEvent?.Invoke(sender);
        }

        toSlot.Item.State.Amount += amountToAdd;
        if (fits)
        {
            fromSlot.Clear();
        }
        else
        {
            fromSlot.Item.State.Amount = amountLeft;
        }
        OnInventoryStateChangedEvent?.Invoke(sender);
    }


    public IInventoryItem GetItem(Type itemType)
    {
        return _slots.FirstOrDefault(s => s.ItemType == itemType)?.Item;
    }

    public IInventoryItem[] GetAllItems()
    {
        var allItems = new List<IInventoryItem>();
        foreach (var slot in _slots)
        {
            if(slot.Item == null) continue;

            allItems.Add(slot.Item);
        }

        return allItems.ToArray();
    }

    public IInventoryItem[] GetAllItems(Type itemType)
    {
        var allItems = new List<IInventoryItem>();
        var slotsOfType = _slots.FindAll(s => !s.IsEmpty && s.ItemType == itemType);
        foreach (var slot in slotsOfType)
        {
            allItems.Add(slot.Item);
        }

        return allItems.ToArray();
    }

    public IInventoryItem[] GetEquippedItems()
    {
        var allItems = new List<IInventoryItem>();
        var slotsOfType = _slots.FindAll(s => !s.IsEmpty && s.Item.State.IsEquipped);
        foreach (var slot in slotsOfType)
        {
            allItems.Add(slot.Item);
        }

        return allItems.ToArray();
    }

    public int GetItemAmount(Type itemType)
    {
        var amount = 0;
        var slotsOfType = _slots.FindAll(s => !s.IsEmpty && s.Item.Type == itemType);
        foreach (var slot in slotsOfType)
        {
            amount += slot.Item.State.Amount;
        }

        return amount;
    }

    public bool TryToAdd(object sender, IInventoryItem item)
    {
        var slotWithSameItemButNotEmpty = _slots.FirstOrDefault(s => !s.IsEmpty && s.ItemType == item.Type && !s.IsFool);
        if (slotWithSameItemButNotEmpty != null)
        {
            return TryAddToSlot(sender, slotWithSameItemButNotEmpty, item);
        }

        var emptySlot = _slots.FirstOrDefault(s => s.IsEmpty);
        if (emptySlot != null)
        {
            return TryAddToSlot(sender, emptySlot, item);
        }

        return false;
    }

    public bool TryAddToSlot(object sender, IInventorySlot slot, IInventoryItem item)
    {
        var canAdd = slot.Amount + item.State.Amount <= item.Info.MaxItemInInventorySlot;
        var amountToAdd = canAdd
            ? item.State.Amount
            : item.Info.MaxItemInInventorySlot - slot.Amount;
        var amountLeft = item.State.Amount - amountToAdd;
        var clonedItem = item.Clone();
        clonedItem.State.Amount = amountToAdd;
        if (slot.IsEmpty)
        {
            slot.SetItem(clonedItem);
        }
        else
        {
            slot.Item.State.Amount += amountToAdd;
            Debug.LogWarning($"Item added, Type: {item.Type}, Amount {amountToAdd}");

        }
        OnInventoryStateChangedEvent?.Invoke(sender);
        OnInventoryItemAddedEvent?.Invoke(sender, item, amountToAdd);
        if (amountLeft <= 0)
        {
            return true;
        }

        item.State.Amount = amountLeft;

        return TryToAdd(sender, item);
    }

    public void Remove(object sender, Type itemType, int amount = 1)
    {
        var slotsWithItem = GetAllSlots(itemType);
        if (slotsWithItem.Length == 0)
        {
            return;
        }

        var amountToRemove = amount;
        var count = slotsWithItem.Length;
        for (int i = count -1; i >= 0; i--)
        {
            var slot = slotsWithItem[i];
            if (slot.Amount >= amountToRemove)
            {
                slot.Item.State.Amount -= amountToRemove;
                if (slot.Amount <= 0)
                {
                    slot.Clear();
                }
                Debug.LogWarning($"Item removed, Type: {itemType}, Amount {amountToRemove}");
                OnInventoryItemRemovedEvent?.Invoke(sender, itemType, amountToRemove);
                OnInventoryStateChangedEvent?.Invoke(sender);
                break;
            }

            var amountRemoved = slot.Amount;
            amountToRemove -= slot.Amount;
            Debug.LogWarning($"Item removed, Type: {itemType}, Amount {amountRemoved}");
            slot.Clear();
            OnInventoryItemRemovedEvent?.Invoke(sender, itemType, amountRemoved);
            OnInventoryStateChangedEvent?.Invoke(sender);
        }
    }

    public IInventorySlot[] GetAllSlots(Type itemType)
    {
        return _slots.FindAll(s => !s.IsEmpty && s.ItemType == itemType).ToArray();
    }

    public IInventorySlot[] GetAllSlots()
    {
        return _slots.ToArray();
    }

    public bool HasItem(Type type, out IInventoryItem item)
    {
        item = GetItem(type);
        
        return item == null;
    }

    public void AddGold(IInventoryItem item)
    {
        this.GoldAmount += IsTrader ? item.State.Amount * item.Info.Price : item.State.Amount * item.Info.Price * (100 - item.Info.MarkdownPercentage) / 100.0f;
    }

    public bool TryRemoveGold(IInventoryItem item)
    {
        var result = IsTrader ? item.State.Amount * item.Info.Price * (100 - item.Info.MarkdownPercentage) / 100.0f : item.State.Amount * item.Info.Price;

        if (result >= 0)
        {
            this.GoldAmount -= result;
            
            return true;
        }

        return false;
    }
}
