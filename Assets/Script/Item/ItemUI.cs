using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;

    public GameObject tooltipPrefab; // 툴팁 프리팹 참조
    private GameObject tooltipInstance; // 생성된 툴팁 인스턴스

    //public Item Item;

    public float mouseX = -150;
    public float mouseY = -150f;

    private void Awake()
    {
        // 자식 컴포넌트에서 UI 컴포넌트 찾기
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        itemNameText = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemPriceText = transform.Find("itemPrice").GetComponent<TextMeshProUGUI>();

        // 툴팁 설정
        //tooltipInstance = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity, this.transform);
        //tooltipInstance.SetActive(false);

        Canvas canvas = GetComponentInParent<Canvas>();

        GameObject tooltipContainer = new GameObject("TooltipContainer");
        tooltipContainer.transform.SetParent(canvas.transform, false);
        tooltipContainer.transform.SetAsLastSibling(); // 이것을 Canvas의 최상단 레이어로 만듭니다.

        // 이제 툴팁 인스턴스를 생성할 때 tooltipContainer를 부모로 사용하세요.
        tooltipInstance = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity, tooltipContainer.transform);
        Debug.Log($"isNull 1 : {tooltipInstance == null}");
        tooltipInstance.SetActive(false);
    }

    // 이 함수는 ShopUIManager 스크립트에서 새로운 아이템 슬롯을 생성할 때 호출됩니다.
    public void Setup(Item item)
    {
        // 아이템 정보 넣기.
        //this.Item = item;

        itemIcon.sprite = item.icon; // 아이템 아이콘을 설정합니다.
        itemNameText.text = item.itemName; // 아이템 이름을 설정합니다.
        itemPriceText.text = $"가격 : {item.price}"; // 아이템 가격을 설정합니다.

        // 추가적인 아이템 정보를 여기에 설정할 수 있습니다.
        Debug.Log($"isNull 2 : {tooltipInstance==null}");
        tooltipInstance.GetComponent<ItemTooltipUI>().Setup(item); // 아이템 데이터로 UI 설정

        // 이벤트 트리거 만들기
        // EventTrigger 컴포넌트 추가
        EventTrigger trigger = itemIcon.gameObject.AddComponent<EventTrigger>();

        // PointerClick 이벤트 항목 생성
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;

        // PointerClick 이벤트에 대한 콜백 추가
        entry.callback.AddListener((eventData) => { OnPointerClick((PointerEventData)eventData, item); });

        // 이벤트 항목을 EventTrigger에 추가
        trigger.triggers.Add(entry);
    }

    // 마우스 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData, Item item)
    {
        //Debug.Log("오른쪽 클릭");
        //마우스 오른쪽 버튼으로 아이템을 클릭했는지 확인
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 인벤토리에 아이템을 추가하고 UI를 업데이트하는 메소드 호출
            //if (item == null)
            //    Debug.Log("item null");
            ShopUI.Instance.BuyItem(item);
            //ShopUI.Instance.UpdateByeItem();
        }
    }

    #region 아이템 툴팁 관련
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
    #endregion
}
