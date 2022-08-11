using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : UIItem
{
    [SerializeField] private Image _imageIcon;
    [SerializeField] private Text _textAmount;
    public IInventoryItem Item { get; private set; }

    public void Refresh(IInventorySlot inventorySlot)
    {
        if (inventorySlot.IsEmpty)
        {
            Cleanup();
            return;
        }

        Item = inventorySlot.Item;
        _imageIcon.sprite = Item.Info.Icon;
        _imageIcon.gameObject.SetActive(true);
        var textAmountEnabled = inventorySlot.Amount > 1;
        _textAmount.gameObject.SetActive(textAmountEnabled);
        if (textAmountEnabled)
        {
            _textAmount.text = inventorySlot.Amount.ToString();
        }
    }

    private void Cleanup()
    {
        _imageIcon.gameObject.SetActive(false);
        _textAmount.gameObject.SetActive(false);
    }
}
