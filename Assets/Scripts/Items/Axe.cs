using System;

public class Axe : IInventoryItem
{
    public IInventoryItemInfo Info { get; set; }
    public IInventoryItemState State { get; set; }
    public Type Type => GetType();
    public IInventoryItem Clone()
    {
        var clonedAxe = new Axe(Info);
        clonedAxe.State.Amount = State.Amount;

        return clonedAxe;
    }

    public Axe(IInventoryItemInfo inventoryItemInfo)
    {
        Info = inventoryItemInfo;
        State = new InventoryItemState();
    }
}
