using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
