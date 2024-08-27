using System.Buffers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuickSlotUI : MonoBehaviour, IDropHandler
{
    public GameObject slot;
    private Item currentItem;
    public int slotIndex; // 슬롯 인덱스 또는 고유 식별자

    public InputActionAsset InputActionAsset; // InputActionAsset을 Inspector에서 할당
    private InputAction slotAction;

    public Item droppedItem;

    private void Awake()
    {
        var InputActionMap = InputActionAsset.FindActionMap("QuickSlot", true);
        slotAction = InputActionMap.FindAction($"Slot{slotIndex}", true);

        slotAction.Enable();
        slotAction.performed += HandleSlotAction;
    }

    private void OnDisable()
    {
        slotAction.Disable();
        slotAction.performed -= HandleSlotAction;
    } 

    public void OnDrop(PointerEventData eventData)
    {
        // 드롭된 아이템을 가져옵니다. 예: eventData.pointerDrag.GetComponent<ItemUI>().Item;
        //Debug.Log($"isNull : {eventData.pointerDrag==null}");
        //Debug.Log($"Name : {eventData.pointerDrag.name}");
        //Debug.Log($"Data : {eventData.pointerDrag.GetComponentInChildren<TextMeshProUGUI>().text}");

        string ItemName = eventData.pointerDrag.GetComponentInChildren<TextMeshProUGUI>().text;
        droppedItem = PlayerItemContainer.Instance.inventoryItems[ItemName];

        if (droppedItem.itemType == Item.ItemType.Consumable)
        {
            UpdateUI(droppedItem.icon);
        }
    }

    // UI 업데이트 메소드
    private void UpdateUI(Sprite itemInfo)
    {
        Image slotImage = slot.GetComponent<Image>();
        //TextMeshProUGUI slotText = GetComponentInChildren<TextMeshProUGUI>();

        //Debug.Log($"Is itemInfo Null? : {itemInfo==null}");
        //Debug.Log($"Is slotImage Null? : {slotImage==null}");
        if (itemInfo != null)
        {
            slotImage.sprite = itemInfo;
            Color color = slotImage.color;
            color.a = 1f;
            slotImage.color = color;
            //slotText.text = $"X{currentItem.quantity}";
        }
        else
        {
            slotImage = null;
            //slotText.text = "";
        }
    }

    public void HandleSlotAction(InputAction.CallbackContext context)
    {
        Debug.Log($"Action performed for slot {slotIndex}");
        if (droppedItem != null)
        {
            if (droppedItem.quantity > 1)
            {
                droppedItem.Use();
            }
            else if(droppedItem.quantity == 1)
            {
                droppedItem.Use();
                Image slotImage = slot.GetComponent<Image>();
                slotImage.color = new Color(1, 1, 1, 0);
                droppedItem = null;
            }
            //else
            //{
            //    Image slotImage = slot.GetComponent<Image>();
            //    slotImage.color = new Color(1,1,1,0);
            //}
        }
    }

}
