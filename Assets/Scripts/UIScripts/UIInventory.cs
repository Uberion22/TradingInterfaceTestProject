using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private InventoryItemInfo _healingElixirInfo;
    [SerializeField] private InventoryItemInfo _axeInfo;
    [SerializeField] private Text _goldAmountText;
    [SerializeField] private bool _isTrader;

    private int slotCount = 16;
    public InventoryWithSlots Inventory => tester.Inventory;
    private UIInventoryTester tester;

    // Start is called before the first frame update
    void Start()
    {
        var uiSlots = GetComponentsInChildren<UIInventorySlot>();
        tester = new UIInventoryTester(_healingElixirInfo, _axeInfo, uiSlots, _isTrader);
        //Inventory = new InventoryWithSlots(slotCount);
        tester.FillSlots();
    }

    // Update is called once per frame
    void Update()
    {
        _goldAmountText.text = _isTrader ? $"TRADER GOLD\n{Inventory.GoldAmount}" : $"PLAYER GOLD\n{Inventory.GoldAmount}";
    }
}
