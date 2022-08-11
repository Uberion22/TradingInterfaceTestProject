using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour, IDropHandler
{
    private UIInventory _uiInventory;
    public virtual void OnDrop(PointerEventData eventData)
    {
        var otherTransform = eventData.pointerDrag.transform;
        otherTransform.SetParent(transform);
        otherTransform.localPosition = Vector2.zero;
    }
}
