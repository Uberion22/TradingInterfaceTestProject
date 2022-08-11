using UnityEngine;

public interface IInventoryItemInfo
{
    string Id { get;}
    string Title { get; }
    Sprite Icon { get; }
    int MaxItemInInventorySlot { get; }
    float Price { get; }
    float MarkdownPercentage { get; }
}
