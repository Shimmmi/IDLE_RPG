using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmithSlot : MonoBehaviour, IPointerClickHandler
{
    public Item thisItem;
    public Image itemImage;

    public void SetItem(Item item)
    {
        thisItem = item;
        if (itemImage != null && item != null)
            itemImage.sprite = item.icon;
    }

    public Item GetItem()
    {
        return thisItem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (SmithPanelController.instance != null)
            {
                SmithPanelController.instance.OnSmithSlotClicked(this);
            }
        }
    }
}
