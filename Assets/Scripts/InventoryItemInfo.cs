using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemInfo", menuName = "GamePlay/Items/Create new ItemInfo" )]
public class InventoryItemInfo : ScriptableObject, IInventoryItemInfo
{
    [SerializeField] private string _id;
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _maxItemInInventorySlot;
    [SerializeField] private float _price;
    [SerializeField] float _markdownPercentage;

    public string Id => _id;
    public string Title => _title;
    public string Description => _description;
    public Sprite Icon => _icon;
    public int MaxItemInInventorySlot => _maxItemInInventorySlot;
    public float Price => _price;
    public float MarkdownPercentage => _markdownPercentage;
}
