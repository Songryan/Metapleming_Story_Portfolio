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

    public GameObject tooltipPrefab; // 툴팁 프리팹 참조
    private GameObject tooltipInstance; // 생성된 툴팁 인스턴스
    public GameObject CanvasObject; // 생성된 툴팁 인스턴스

    private Vector2 originalPosition;
    //public Vector2 dragOffset = new Vector2(527, -22); // 드래그 위치 오프셋
    //public Vector2 dragOffset = new Vector2(327, 228); // 드래그 위치 오프셋

    public float mouseX = -70f;
    public float mouseY = -250f;

    public float modifyX = 327;
    public float modifyY = 228;
    private void Awake()
    {
        // 자식 컴포넌트에서 UI 컴포넌트 찾기
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        EquipStatus = transform.Find("EquipStatus").GetComponent<Image>();
        EquipStatus.color = new Color(255, 255, 255, 0);
        ItemQuantity = transform.Find("ItemQuantity").GetComponent<TextMeshProUGUI>();

        //tooltipInstance = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity, this.transform);
        //tooltipInstance.SetActive(false);
        Canvas canvas = GetComponentInParent<Canvas>();

        GameObject tooltipContainer = new GameObject("TooltipContainer");
        tooltipContainer.transform.SetParent(canvas.transform, false);
        tooltipContainer.transform.SetAsLastSibling(); // 이것을 Canvas의 최상단 레이어로 만듭니다.

        // 이제 툴팁 인스턴스를 생성할 때 tooltipContainer를 부모로 사용하세요.
        tooltipInstance = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity, tooltipContainer.transform);
        tooltipInstance.SetActive(false);
    }

    // 이 함수는 ShopUIManager 스크립트에서 새로운 아이템 슬롯을 생성할 때 호출됩니다.
    public void Setup(Item item)
    {
        ItemKey = item.itemName;
        itemBackup = item;
        itemIcon.sprite = item.icon; // 아이템 아이콘을 설정합니다.
        ItemQuantity.text = $"X{item.quantity}"; // 아이템 갯수을 설정합니다.

        // TextMeshPro에 아이템 정보 설정
        TextMeshProUGUI textComponent = itemIcon.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = item.itemName; // 예를 들어, 아이템 이름을 저장
        }

        tooltipInstance.GetComponent<ItemTooltipUI>().Setup(item); // 아이템 데이터로 UI 설정


        // 입고있는 애들만 바꿔주기.
        if(item.isEquipped)
            EquipStatus.color = new Color(1, 1, 1, 1);


        // 이벤트 트리거 만들기
        // EventTrigger 컴포넌트 추가
        EventTrigger trigger = itemIcon.gameObject.AddComponent<EventTrigger>();

        // PointerClick 이벤트 항목 생성
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;

        // PointerClick 이벤트에 대한 콜백 추가
        entry.callback.AddListener((eventData) => { OnUseClick((PointerEventData)eventData, item); });

        // 이벤트 항목을 EventTrigger에 추가
        trigger.triggers.Add(entry);

        SetupEventTriggers(); //추가
    }

    public void OnUseClick(PointerEventData eventData, Item item)
    {
        Debug.Log("오른쪽 클릭");
        //마우스 오른쪽 버튼으로 아이템을 클릭했는지 확인
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
                    Debug.Log($"{item.itemName}을 벗기!");
                    EquipStatus.color = new Color(1, 1, 1, 0);
                    item.onTakeoff();
                    //InventoryManager.Instance.InvenUpdate();
                    //tooltipInstance.SetActive(false);
                }
                else
                {
                    Debug.Log($"{item.itemName}을 입기!");
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
    //    // 마우스 포지션을 Canvas의 Local Space로 변환합니다.
    //    Vector2 localPoint;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        canvasRect,
    //        Input.mousePosition,
    //        canvas.worldCamera,
    //        out localPoint
    //    );
    //
    //    // 툴팁의 위치를 설정합니다.
    //    tooltipRect.localPosition = localPoint;
    //}

    private void UpdateTooltipPosition()
    {
        Canvas canvas = tooltipInstance.transform.parent.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform tooltipRect = tooltipInstance.GetComponent<RectTransform>();
    
        // 마우스 포지션을 Canvas의 Local Space로 변환합니다.
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.worldCamera,
            out localPoint
        );
    
        // 툴팁의 위치를 조정합니다.
        localPoint += new Vector2(tooltipRect.sizeDelta.x + mouseX, tooltipRect.sizeDelta.y + mouseY); // 툴팁이 마우스 포인터 아래에 나오도록 조정합니다.
        tooltipRect.localPosition = localPoint;
    }

    void Update()
    {
        // 마우스가 움직일 때마다 툴팁 위치를 업데이트합니다.
        if (tooltipInstance.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 시작!");
        originalPosition = transform.position; // 원래 위치를 저장합니다.
    }

    public void OnDrag(PointerEventData eventData)
    {
        //RectTransform canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        //
        //Vector2 localPointerPosition;
        //// eventData.pressEventCamera를 캔버스에 부착된 렌더 카메라로 사용합니다.
        //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        //{
        //    // localPosition이 아니라 anchoredPosition을 업데이트합니다.
        //    // UI 요소가 RectTransform 컴포넌트를 가지고 있다고 가정합니다.
        //    RectTransform rectTransform = GetComponent<RectTransform>();
        //    //rectTransform.anchoredPosition = localPointerPosition + dragOffset;
        //    rectTransform.anchoredPosition = localPointerPosition;
        //}
        
        
        // Canvas의 RectTransform을 가져옵니다.
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        
        // 드래그 위치를 캔버스 좌표로 변환합니다.
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            // 아이콘(또는 아이템 슬롯)의 위치를 캔버스의 로컬 좌표에 맞춰 업데이트합니다.
            //transform.localPosition = localPointerPosition + new Vector2(modifyX, modifyY);
            transform.localPosition = localPointerPosition;
        }
    }

    //추가
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 끝!");
        // 여기서는 원래 위치로 되돌리지만, 실제 사용 시에는 다른 로직이 들어갈 수 있습니다.
        transform.position = originalPosition;
    }

    private void SetupEventTriggers()
    {
        EventTrigger trigger = itemIcon.gameObject.GetComponent<EventTrigger>() ?? itemIcon.gameObject.AddComponent<EventTrigger>();
        //EventTrigger trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

        // BeginDrag 이벤트 추가
        AddEventTrigger(trigger, EventTriggerType.BeginDrag, (data) => OnBeginDrag((PointerEventData)data));
        // Drag 이벤트 추가
        AddEventTrigger(trigger, EventTriggerType.Drag, (data) => OnDrag((PointerEventData)data));
        // EndDrag 이벤트 추가
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
