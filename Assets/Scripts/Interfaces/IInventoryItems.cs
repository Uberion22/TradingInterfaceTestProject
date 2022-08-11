using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItemInfo
{
    string Id { get;}
    string Title { get; }
    string Description { get; }
    Sprite Icon { get; }
    int MaxItemInInventorySlot { get; }
    float Price { get; }
    float MarkdownPercentage { get; }
}
