using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.Events;

public class ItemInvenUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item itemBackup { get; set; }
    public Image itemIcon;
    public Image EquipStatus;
    public TextMeshProUGUI ItemQuantity;

    public string ItemKey { get; set; }

    public GameObject tooltipPrefab; // ���� ������ ����
    private GameObject tooltipInstance; // ������ ���� �ν��Ͻ�
    public GameObject CanvasObject; // ������ ���� �ν��Ͻ�

    private Vector2 originalPosition;
    //public Vector2 dragOffset = new Vector2(527, -22); // �巡�� ��ġ ������
    //public Vector2 dragOffset = new Vector2(327, 228); // �巡�� ��ġ ������

    public float mouseX = -70f;
    public float mouseY = -250f;

    public float modifyX = 327;
    public float modifyY = 228;
    private void Awake()
    {
        // �ڽ� ������Ʈ���� UI ������Ʈ ã��
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        EquipStatus = transform.Find("EquipStatus").GetComponent<Image>();
        EquipStatus.color = new Color(255, 255, 255, 0);
        ItemQuantity = transform.Find("ItemQuantity").GetComponent<TextMeshProUGUI>();

        //tooltipInstance = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity, this.transform);
        //tooltipInstance.SetActive(false);
        Canvas canvas = GetComponentInParent<Canvas>();

        GameObject tooltipContainer = new GameObject("TooltipContainer");
        tooltipContainer.transform.SetParent(canvas.transform, false);
        tooltipContainer.transform.SetAsLastSibling(); // �̰��� Canvas�� �ֻ�� ���̾�� ����ϴ�.

        // ���� ���� �ν��Ͻ��� ������ �� tooltipContainer�� �θ�� ����ϼ���.
        tooltipInstance = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity, tooltipContainer.transform);
        tooltipInstance.SetActive(false);
    }

    // �� �Լ��� ShopUIManager ��ũ��Ʈ���� ���ο� ������ ������ ������ �� ȣ��˴ϴ�.
    public void Setup(Item item)
    {
        ItemKey = item.itemName;
        itemBackup = item;
        itemIcon.sprite = item.icon; // ������ �������� �����մϴ�.
        ItemQuantity.text = $"X{item.quantity}"; // ������ ������ �����մϴ�.

        // TextMeshPro�� ������ ���� ����
        TextMeshProUGUI textComponent = itemIcon.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = item.itemName; // ���� ���, ������ �̸��� ����
        }

        tooltipInstance.GetComponent<ItemTooltipUI>().Setup(item); // ������ �����ͷ� UI ����


        // �԰��ִ� �ֵ鸸 �ٲ��ֱ�.
        if(item.isEquipped)
            EquipStatus.color = new Color(1, 1, 1, 1);


        // �̺�Ʈ Ʈ���� �����
        // EventTrigger ������Ʈ �߰�
        EventTrigger trigger = itemIcon.gameObject.AddComponent<EventTrigger>();

        // PointerClick �̺�Ʈ �׸� ����
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;

        // PointerClick �̺�Ʈ�� ���� �ݹ� �߰�
        entry.callback.AddListener((eventData) => { OnUseClick((PointerEventData)eventData, item); });

        // �̺�Ʈ �׸��� EventTrigger�� �߰�
        trigger.triggers.Add(entry);

        SetupEventTriggers(); //�߰�
    }

    public void OnUseClick(PointerEventData eventData, Item item)
    {
        Debug.Log("������ Ŭ��");
        //���콺 ������ ��ư���� �������� Ŭ���ߴ��� Ȯ��
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item.itemType == Item.ItemType.Consumable)
            {
                item.Use();
                //tooltipInstance.SetActive(false);
                //InventoryManager.Instance.InvenUpdate();
            }
            else if (item.itemType == Item.ItemType.Equipment)
            {
                if (item.isEquipped)
                {
                    Debug.Log($"{item.itemName}�� ����!");
                    EquipStatus.color = new Color(1, 1, 1, 0);
                    item.onTakeoff();
                    //InventoryManager.Instance.InvenUpdate();
                    //tooltipInstance.SetActive(false);
                }
                else
                {
                    Debug.Log($"{item.itemName}�� �Ա�!");
                    EquipStatus.color = new Color(1, 1, 1, 1);
                    item.onPuton();
                    //InventoryManager.Instance.InvenUpdate();
                    //tooltipInstance.SetActive(false);
                }
            }
            else { }
        }
        InventoryManager.Instance.InvenUpdate();
        tooltipInstance.SetActive(false);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipInstance.SetActive(true);
        UpdateTooltipPosition();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipInstance.SetActive(false);
    }

    //private void UpdateTooltipPosition()
    //{
    //    Canvas canvas = tooltipInstance.transform.parent.GetComponentInParent<Canvas>();
    //    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    //    RectTransform tooltipRect = tooltipInstance.GetComponent<RectTransform>();
    //
    //    // ���콺 �������� Canvas�� Local Space�� ��ȯ�մϴ�.
    //    Vector2 localPoint;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        canvasRect,
    //        Input.mousePosition,
    //        canvas.worldCamera,
    //        out localPoint
    //    );
    //
    //    // ������ ��ġ�� �����մϴ�.
    //    tooltipRect.localPosition = localPoint;
    //}

    private void UpdateTooltipPosition()
    {
        Canvas canvas = tooltipInstance.transform.parent.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform tooltipRect = tooltipInstance.GetComponent<RectTransform>();
    
        // ���콺 �������� Canvas�� Local Space�� ��ȯ�մϴ�.
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.worldCamera,
            out localPoint
        );
    
        // ������ ��ġ�� �����մϴ�.
        localPoint += new Vector2(tooltipRect.sizeDelta.x + mouseX, tooltipRect.sizeDelta.y + mouseY); // ������ ���콺 ������ �Ʒ��� �������� �����մϴ�.
        tooltipRect.localPosition = localPoint;
    }

    void Update()
    {
        // ���콺�� ������ ������ ���� ��ġ�� ������Ʈ�մϴ�.
        if (tooltipInstance.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("�巡�� ����!");
        originalPosition = transform.position; // ���� ��ġ�� �����մϴ�.
    }

    public void OnDrag(PointerEventData eventData)
    {
        //RectTransform canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        //
        //Vector2 localPointerPosition;
        //// eventData.pressEventCamera�� ĵ������ ������ ���� ī�޶�� ����մϴ�.
        //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        //{
        //    // localPosition�� �ƴ϶� anchoredPosition�� ������Ʈ�մϴ�.
        //    // UI ��Ұ� RectTransform ������Ʈ�� ������ �ִٰ� �����մϴ�.
        //    RectTransform rectTransform = GetComponent<RectTransform>();
        //    //rectTransform.anchoredPosition = localPointerPosition + dragOffset;
        //    rectTransform.anchoredPosition = localPointerPosition;
        //}
        
        
        // Canvas�� RectTransform�� �����ɴϴ�.
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        
        // �巡�� ��ġ�� ĵ���� ��ǥ�� ��ȯ�մϴ�.
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            // ������(�Ǵ� ������ ����)�� ��ġ�� ĵ������ ���� ��ǥ�� ���� ������Ʈ�մϴ�.
            //transform.localPosition = localPointerPosition + new Vector2(modifyX, modifyY);
            transform.localPosition = localPointerPosition;
        }
    }

    //�߰�
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("�巡�� ��!");
        // ���⼭�� ���� ��ġ�� �ǵ�������, ���� ��� �ÿ��� �ٸ� ������ �� �� �ֽ��ϴ�.
        transform.position = originalPosition;
    }

    private void SetupEventTriggers()
    {
        EventTrigger trigger = itemIcon.gameObject.GetComponent<EventTrigger>() ?? itemIcon.gameObject.AddComponent<EventTrigger>();
        //EventTrigger trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

        // BeginDrag �̺�Ʈ �߰�
        AddEventTrigger(trigger, EventTriggerType.BeginDrag, (data) => OnBeginDrag((PointerEventData)data));
        // Drag �̺�Ʈ �߰�
        AddEventTrigger(trigger, EventTriggerType.Drag, (data) => OnDrag((PointerEventData)data));
        // EndDrag �̺�Ʈ �߰�
        AddEventTrigger(trigger, EventTriggerType.EndDrag, (data) => OnEndDrag((PointerEventData)data));
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }
}
