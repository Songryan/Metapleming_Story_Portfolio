using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    //public GameObject tooltipPrefab; // 툴팁 프리팹에 대한 참조
    //public Canvas canvas; // 툴팁이 위치할 캔버스
    //private GameObject tooltipInstance; // 생성된 툴팁 인스턴스
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI ItemDescription;
    public TextMeshProUGUI ItemStat;

    //public GameObject tooltipPrefab; // 툴팁 프리팹 참조

    void Awake()
    {
        // 자식 컴포넌트에서 UI 컴포넌트 찾기
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        itemNameText = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        ItemDescription = transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
        ItemStat = transform.Find("ItemStat").GetComponent<TextMeshProUGUI>();
    }

    public void Setup(Item item)
    {
        itemIcon.sprite = item.icon; // 아이템 아이콘을 설정합니다.
        itemNameText.text = $"{item.itemName}"; // 아이템 이름을 설정합니다.
        ItemDescription.text = $"{item.description}"; // 아이템 설명을 설정합니다.
        ItemStat.text = $"STR : {item.stat_array[0]}\r\nDEX : {item.stat_array[1]}\r\nINT : {item.stat_array[2]}\r\nLUK : {item.stat_array[3]}"; // 아이템 갯수을 설정합니다.
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    tooltipInstance.SetActive(true); // 툴팁을 활성화합니다.
    //    UpdateTooltipPosition();
    //}
    //
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    tooltipInstance.SetActive(false); // 툴팁을 숨깁니다.
    //}
    //
    //void UpdateTooltipPosition()
    //{
    //    Vector3 mousePosition = Input.mousePosition;
    //    // z 축 값을 카메라에서 캔버스 평면까지의 거리로 설정합니다.
    //    float distanceToCamera = Vector3.Distance(canvas.transform.position, Camera.main.transform.position);
    //    mousePosition.z = distanceToCamera;
    //    // 스크린 좌표를 월드 좌표로 변환합니다.
    //    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
    //
    //    // Tooltip의 RectTransform을 조정합니다.
    //    RectTransform tooltipRect = tooltipInstance.GetComponent<RectTransform>();
    //    tooltipRect.position = worldPoint;
    //
    //    // Canvas 내에서 툴팁이 올바르게 배치되도록 조정합니다.
    //    tooltipRect.localPosition = new Vector3(tooltipRect.localPosition.x, tooltipRect.localPosition.y, 0f);
    //}
    //
    //void Update()
    //{
    //    // 마우스가 이동하면 툴팁 위치를 업데이트합니다.
    //    if (tooltipInstance.activeSelf)
    //    {
    //        UpdateTooltipPosition();
    //    }
    //}
}
