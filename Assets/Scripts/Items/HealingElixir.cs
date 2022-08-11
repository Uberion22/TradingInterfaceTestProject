using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingElixir : IInventoryItem
{
    public IInventoryItemInfo Info { get; set; }
    public IInventoryItemState State { get; set; }
    public Type Type => GetType();
    public IInventoryItem Clone()
    {
        var clonedhealingElixir = new HealingElixir(Info);
        clonedhealingElixir.State.Amount = State.Amount;

        return clonedhealingElixir;
    }

    public HealingElixir(IInventoryItemInfo inventoryItemInfo)
    {
        Info = inventoryItemInfo;
        State = new InventoryItemState();
    }
}
