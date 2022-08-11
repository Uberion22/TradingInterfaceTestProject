using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    IInventoryItemInfo Info { get; }
    IInventoryItemState State { get; }
    Type Type { get; }

    IInventoryItem Clone();
}
