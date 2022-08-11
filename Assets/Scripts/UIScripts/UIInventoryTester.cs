using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIInventoryTester : MonoBehaviour
{
    [SerializeField] private GameObject _playerInventoryObj;
    [SerializeField] private GameObject _traderInventoryObj;
    [SerializeField] private InventoryItemInfo[] _itemInfo;
    [SerializeField] private float _playerGold;
    [SerializeField] private float _traderGold;

    private UIInventory _playerUiInventory;
    private UIInventory _traderUiInventory;

    private InventoryWithSlots _playerInventory;
    private InventoryWithSlots _traderInventory;

    void Start()
    {
        _playerUiInventory = _playerInventoryObj.GetComponent<UIInventory>();
        _traderUiInventory = _traderInventoryObj.GetComponent<UIInventory>();
        _playerInventory = _playerUiInventory.Inventory;
        _traderInventory = _traderUiInventory.Inventory;
        _playerInventory.SetGoldAmount(_playerGold);
        _traderInventory.SetGoldAmount(_traderGold);
        FillSlots(_playerInventory, _playerUiInventory.GetSlots());
        FillSlots(_traderInventory, _traderUiInventory.GetSlots());
        _playerInventory.OnInventoryStateChangedEvent += OnPlayerInventoryStateChanged;
        _traderInventory.OnInventoryStateChangedEvent += OnTraderInventoryStateChanged;
    }

    private void OnPlayerInventoryStateChanged(object sender)
    {
        foreach (var uislot in _playerUiInventory.GetSlots())
        {
            uislot.Refresh();
        }
    }

    private void OnTraderInventoryStateChanged(object sender)
    {
        foreach (var uislot in _traderUiInventory.GetSlots())
        {
            uislot.Refresh();
        }
    }

    private IInventorySlot AddRandomItemRandomSlots(List<IInventorySlot> slots, InventoryWithSlots inventory)
    {
        var rIndex = Random.Range(0, _itemInfo.Length);
        var itemInfo = _itemInfo[rIndex];
        var rSlotIndex = Random.Range(0, slots.Count-1);
        var rSlot = slots[rSlotIndex];
        var rCount = Random.Range(1, itemInfo.MaxItemInInventorySlot + 1);
        if (itemInfo.Title.Contains("Axe"))
        {
            inventory.TryAddToSlot(this, rSlot, new Axe(itemInfo) {State = {Amount = rCount}});
        }
        else
        {
            inventory.TryAddToSlot(this, rSlot, new HealingElixir(itemInfo) { State = { Amount = rCount } });
        }
        return rSlot;
    }

    public void FillSlots(InventoryWithSlots inventory, UIInventorySlot[] uiSlots, int filledSlots = 12)
    {
        var allSlots = inventory.GetAllSlots();
        var availableSlots = new List<IInventorySlot>(allSlots);
        for (int i = 0; i < filledSlots; i++)
        {
            var fieldSlot = AddRandomItemRandomSlots(availableSlots, inventory);
            availableSlots.Remove(fieldSlot);
        }
        SetupInventoryUI(inventory, uiSlots);
    }

    private void SetupInventoryUI(InventoryWithSlots inventory, UIInventorySlot[] uiSlots)
    {
        var allSlots = inventory.GetAllSlots();
        var allSlotsCount = allSlots.Length;
        for (int i = 0; i < allSlotsCount; i++)
        {
            var slot = allSlots[i];
            var uiSlot = uiSlots[i];
            uiSlot.SetSlot(slot);
            uiSlot.Refresh();
        }
    }
}
