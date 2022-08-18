using System;

public class InventorySlot : IInventorySlot
{
    public bool IsFool => ! IsEmpty && Amount == Capacity;

    public bool IsEmpty => Item == null;

    public IInventoryItem Item { get; private set; }
    public Type ItemType => Item?.Type;

    public int Amount => IsEmpty ? 0 : Item.State.Amount;
    public int Capacity { get; private set; }

    public void SetItem(IInventoryItem item)
    {
        if(!IsEmpty) return;

        Item = item;
        Capacity = item.Info.MaxItemInInventorySlot;
    }

    public void Clear()
    {
        if (IsEmpty) return;

        Item.State.Amount = 0;
        Item = null;
    }
}
