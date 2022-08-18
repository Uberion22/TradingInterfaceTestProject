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
    public bool IsTraderInventory;
    public bool IsFull => _slots.All(s => s.IsFool);
    private List<IInventorySlot> _slots;
    public float GoldAmount { get; private set; }

    public InventoryWithSlots(int capacity, bool isTrader = false)
    {
        Capacity = capacity;
        _slots = new List<IInventorySlot>();
        IsTraderInventory = isTrader;

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

    public void TransitFromSlotToSlotInTrade(InventoryWithSlots senderInventory, IInventorySlot fromSlot, IInventorySlot toSlot)
    {
        if (fromSlot.IsEmpty || toSlot == null || IsFull) return;

        if (toSlot.ItemType != null && toSlot.ItemType != fromSlot.ItemType)
        {
            toSlot = GetFirstSuitableSlot(fromSlot.Item);
        }
        var slotCapacity = fromSlot.Capacity;
        var fits = fromSlot.Amount + toSlot.Amount <= slotCapacity;
        var amountToAdd = fits
            ? fromSlot.Amount
            : slotCapacity - toSlot.Amount;
        var amountLeft = fromSlot.Amount - amountToAdd;

        if(!TryRemoveGold(fromSlot.Item, amountToAdd)) return;

        senderInventory.AddGold(fromSlot.Item, amountToAdd);

        if (toSlot.IsEmpty)
        {
            toSlot.SetItem(fromSlot.Item);
            fromSlot.Clear();
            OnInventoryStateChangedEvent?.Invoke(senderInventory);
        }

        toSlot.Item.State.Amount += amountToAdd;
        if (fits)
        {
            fromSlot.Clear();
        }
        else
        {
            fromSlot.Item.State.Amount = amountLeft;
            var nextSlot = GetFirstSuitableSlot(fromSlot.Item);
            TransitFromSlotToSlotInTrade(senderInventory, fromSlot, nextSlot);
        }
        OnInventoryStateChangedEvent?.Invoke(senderInventory);
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
            if (slot.Item == null) continue;

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

    private void AddGold(IInventoryItem item, int itemCount)
    {
        this.GoldAmount += IsTraderInventory ? itemCount * item.Info.Price : itemCount * item.Info.Price * (100 - item.Info.MarkdownPercentage) / 100.0f;
    }

    private bool TryRemoveGold(IInventoryItem item, int itemCount)
    {
        var result = IsTraderInventory ? itemCount * item.Info.Price * (100 - item.Info.MarkdownPercentage) / 100.0f : itemCount * item.Info.Price;
        var goldAfterTrade = this.GoldAmount - result;
        if (goldAfterTrade >= 0)
        {
            this.GoldAmount = goldAfterTrade;
            
            return true;
        }

        return false;
    }

    private IInventorySlot GetFirstSuitableSlot(IInventoryItem item)
    {
        var emptySlot = _slots.FirstOrDefault(s => s.ItemType == item.Type && s.Amount < s.Capacity) 
                        ?? _slots.FirstOrDefault(s => s.IsEmpty);

        return emptySlot;
    }

    public void SetGoldAmount(float newGoldAmount)
    {
        GoldAmount = newGoldAmount;
    }
}
