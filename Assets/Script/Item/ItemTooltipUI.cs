using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    //public GameObject tooltipPrefab; // ���� �����տ� ���� ����
    //public Canvas canvas; // ������ ��ġ�� ĵ����
    //private GameObject tooltipInstance; // ������ ���� �ν��Ͻ�
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI ItemDescription;
    public TextMeshProUGUI ItemStat;

    //public GameObject tooltipPrefab; // ���� ������ ����

    void Awake()
    {
        // �ڽ� ������Ʈ���� UI ������Ʈ ã��
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        itemNameText = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        ItemDescription = transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
        ItemStat = transform.Find("ItemStat").GetComponent<TextMeshProUGUI>();
    }

    public void Setup(Item item)
    {
        itemIcon.sprite = item.icon; // ������ �������� �����մϴ�.
        itemNameText.text = $"{item.itemName}"; // ������ �̸��� �����մϴ�.
        ItemDescription.text = $"{item.description}"; // ������ ������ �����մϴ�.
        ItemStat.text = $"STR : {item.stat_array[0]}\r\nDEX : {item.stat_array[1]}\r\nINT : {item.stat_array[2]}\r\nLUK : {item.stat_array[3]}"; // ������ ������ �����մϴ�.
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    tooltipInstance.SetActive(true); // ������ Ȱ��ȭ�մϴ�.
    //    UpdateTooltipPosition();
    //}
    //
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    tooltipInstance.SetActive(false); // ������ ����ϴ�.
    //}
    //
    //void UpdateTooltipPosition()
    //{
    //    Vector3 mousePosition = Input.mousePosition;
    //    // z �� ���� ī�޶󿡼� ĵ���� �������� �Ÿ��� �����մϴ�.
    //    float distanceToCamera = Vector3.Distance(canvas.transform.position, Camera.main.transform.position);
    //    mousePosition.z = distanceToCamera;
    //    // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ�մϴ�.
    //    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
    //
    //    // Tooltip�� RectTransform�� �����մϴ�.
    //    RectTransform tooltipRect = tooltipInstance.GetComponent<RectTransform>();
    //    tooltipRect.position = worldPoint;
    //
    //    // Canvas ������ ������ �ùٸ��� ��ġ�ǵ��� �����մϴ�.
    //    tooltipRect.localPosition = new Vector3(tooltipRect.localPosition.x, tooltipRect.localPosition.y, 0f);
    //}
    //
    //void Update()
    //{
    //    // ���콺�� �̵��ϸ� ���� ��ġ�� ������Ʈ�մϴ�.
    //    if (tooltipInstance.activeSelf)
    //    {
    //        UpdateTooltipPosition();
    //    }
    //}
}
