using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemBuyUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI ItemQuantity;

    public GameObject tooltipPrefab; // ���� ������ ����
    private GameObject tooltipInstance; // ������ ���� �ν��Ͻ�

    public float mouseX = -150f;
    public float mouseY = -150f;

    private void Awake()
    {
        // �ڽ� ������Ʈ���� UI ������Ʈ ã��
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        itemNameText = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemPriceText = transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>();
        ItemQuantity = transform.Find("ItemQuantity").GetComponent<TextMeshProUGUI>();

        //tooltipInstance = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity, this.transform.parent.parent);
        //tooltipInstance.transform.SetAsLastSibling();
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
        itemIcon.sprite = item.icon; // ������ �������� �����մϴ�.
        itemNameText.text = item.itemName; // ������ �̸��� �����մϴ�.
        itemPriceText.text = $"�ǸŰ��� : {item.reSell_price}"; // ������ ������ �����մϴ�.
        ItemQuantity.text = $"X{item.quantity}"; // ������ ������ �����մϴ�.
        // �߰����� ������ ������ ���⿡ ������ �� �ֽ��ϴ�.
        tooltipInstance.GetComponent<ItemTooltipUI>().Setup(item); // ������ �����ͷ� UI ����

        // �̺�Ʈ Ʈ���� �����
        // EventTrigger ������Ʈ �߰�
        EventTrigger trigger = itemIcon.gameObject.AddComponent<EventTrigger>();

        // PointerClick �̺�Ʈ �׸� ����
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;

        // PointerClick �̺�Ʈ�� ���� �ݹ� �߰�
        entry.callback.AddListener((eventData) => { OnSellPointerClick((PointerEventData)eventData, item); });

        // �̺�Ʈ �׸��� EventTrigger�� �߰�
        trigger.triggers.Add(entry);
    }
    public void OnSellPointerClick(PointerEventData eventData, Item item)
    {
        Debug.Log("������ Ŭ��");
        //���콺 ������ ��ư���� �������� Ŭ���ߴ��� Ȯ��
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // �κ��丮�� �������� �߰��ϰ� UI�� ������Ʈ�ϴ� �޼ҵ� ȣ��
            //if (item == null)
            //    Debug.Log("item null");
            ShopUI.Instance.SellItem(item);
            //ShopUI.Instance.UpdateByeItem();
            tooltipInstance.SetActive(false);
        }
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
}