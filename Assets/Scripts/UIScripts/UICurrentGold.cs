using UnityEngine;
using UnityEngine.UI;

public class UICurrentGold : MonoBehaviour
{
    [SerializeField] private Text _goldAmountText;
    [SerializeField] private UIInventory _playerUiInventory;
    [SerializeField] private UIInventory _traderUiInventory;

    void Start()
    {
        _playerUiInventory.Inventory.OnInventoryStateChangedEvent += UpdateText;
        _traderUiInventory.Inventory.OnInventoryStateChangedEvent += UpdateText;
    }

    void OnDestroy()
    {
        _playerUiInventory.Inventory.OnInventoryStateChangedEvent -= UpdateText;
        _traderUiInventory.Inventory.OnInventoryStateChangedEvent -= UpdateText;
    }
    void UpdateText(object sender)
    {
        _goldAmountText.text = $"Current Gold\n{_playerUiInventory.Inventory.GoldAmount}";
    }
}
