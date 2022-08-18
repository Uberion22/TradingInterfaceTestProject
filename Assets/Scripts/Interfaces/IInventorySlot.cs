using System;

public interface IInventorySlot
{
    bool IsFool { get; }
    bool IsEmpty { get; }
    IInventoryItem Item { get;}
    Type ItemType { get; }
    int Amount { get; }
    int Capacity { get; }

    void SetItem(IInventoryItem item);
    void Clear();
}
