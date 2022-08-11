using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Text _goldAmountText;
    [SerializeField] private bool _isTrader;
    private UIInventorySlot[] _uiSlots;
    public InventoryWithSlots Inventory;

    // Start is called before the first frame update
    void OnEnable()
    {
        _uiSlots = GetComponentsInChildren<UIInventorySlot>();
        Inventory = new InventoryWithSlots(_uiSlots.Length, _isTrader);
    }

    // Update is called once per frame
    void Update()
    {
        _goldAmountText.text = _isTrader ? $"TRADER GOLD\n{Inventory.GoldAmount}" : $"PLAYER GOLD\n{Inventory.GoldAmount}";
    }

    public UIInventorySlot[] GetSlots()
    {
        return _uiSlots;
    }
}
