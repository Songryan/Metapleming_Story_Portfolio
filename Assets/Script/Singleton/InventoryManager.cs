using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Item;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }   // InventoryManager의 싱글톤 인스턴스

    public Dictionary<string, Item> inventoryItems;

    public Transform invenGridLayer; // InvenGridLayer의 Transform을 참조

    public string activeSlot;
    
    public TextMeshProUGUI TextMeso;

    [Header("Prefab Item List")]
    public GameObject Inven_Prefab;

    [Header("Player Money")]
    public int playerMoney; // 플레이어의 돈을 저장하는 변수

    [Header("Button List")]
    public Button ALL;
    public Button Equipment;
    public Button Consumable;
    public Button ETC;
    
    //[Header("Color List")] // 각 버튼의 기본 색상을 저장할 변수
    private Color defaultButtonColor = new Color(255f/255f, 255f/255f, 255f/255f); // 기본 색상
    private Color activeButtonColor = new Color(255f/255f, 71f/255f, 66f/255f);  // 활성화 색상

    void Awake()
    {
        // 싱글톤 인스턴스를 설정합니다.
        if (Instance == null)
        {
            inventoryItems = new Dictionary<string, Item>();
            inventoryItems = PlayerItemContainer.Instance.inventoryItems;

            playerMoney = PlayerItemContainer.Instance.PlayerMoney;
            TextMeso.text = playerMoney.ToString();
            //inventoryItems = new Dictionary<string, Item>();
            //Item RedPotion = ItemManager.Instance.RedPotion;
            //AddItem(RedPotion);
            //Item OrangePotion = ItemManager.Instance.OrangePotion;
            //AddItem(OrangePotion);

            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 오브젝트가 파괴되지 않도록 합니다.
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하는 경우 현재 오브젝트를 파괴합니다.
        }
    }

    private void Start()
    {
        OnClickALL();
    }

    

    #region 인벤토리 추가 제거 사용 추가
    /*public void AddItemToUI(Item item)
    {
        // itemIconPrefab 딕셔너리를 사용하여 새로운 아이템 아이콘 인스턴스를 생성합니다.
        GameObject itemIconInstance = Instantiate(itemIconPrefab[item.itemName], invenGridLayer);

        // 생성된 인스턴스 내의 QuantityText 컴포넌트를 찾아 수량을 설정합니다.
        TextMeshProUGUI quantityText = itemIconInstance.transform.Find("quantity").GetComponent<TextMeshProUGUI>();
        if (quantityText != null)
        {
            quantityText.text = "X"+item.quantity.ToString(); // 수량을 문자열로 변환하여 설정
        }

        // 추가적인 설정이 필요하면 여기에 추가합니다.
        // 예를 들어, 클릭 이벤트 리스너를 추가할 수 있습니다.
    }*/

    // 아이템을 인벤토리에서 제거하는 메소드
    public void RemoveItem(Item item)
    {
        if (inventoryItems.ContainsKey(item.itemName))
        {
            inventoryItems.Remove(item.itemName);
            // TODO: 인벤토리 UI 업데이트 로직을 호출합니다.
            // 예: InventoryUI.UpdateInventoryDisplay();
        }
    }

    // 아이템 사용 메소드
    public void UseItem(Item item)
    {
        if (inventoryItems.ContainsKey(item.itemName))
        {
            item.Use();
            // 아이템 사용 후에는 일반적으로 인벤토리에서 제거되거나 수량이 줄어듭니다.
            // 예: RemoveItem(item);
        }
    }
    #endregion


    #region 인벤 버튼
    public void AddInvenSlot(string itemName, Item item)
    {
        var slot = Instantiate(Inven_Prefab, invenGridLayer);
        slot.GetComponent<ItemInvenUI>().Setup(item); // 아이템 데이터로 UI 설정
    }
    
    public void OnClickALL()
    {
        activeSlot = "ALL";

        SetButtonColors(ALL);

        // 기존의 아이템 슬롯들을 모두 제거
        foreach (Transform child in invenGridLayer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in inventoryItems)
        {
            AddInvenSlot(item.Key,item.Value);
        }
    }

    public void OnClickEquipment()
    {
        activeSlot = "Equipment";

        SetButtonColors(Equipment);

        // 기존의 아이템 슬롯들을 모두 제거
        foreach (Transform child in invenGridLayer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in inventoryItems)
        {
            if (inventoryItems[item.Key].itemType == ItemType.Equipment)
                AddInvenSlot(item.Key, item.Value);
        }
    }

    public void OnClickConsumable()
    {
        activeSlot = "Consumable";

        SetButtonColors(Consumable);

        // 기존의 아이템 슬롯들을 모두 제거
        foreach (Transform child in invenGridLayer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in inventoryItems)
        {
            if (inventoryItems[item.Key].itemType == ItemType.Consumable)
                AddInvenSlot(item.Key, item.Value);
        }
    }

    public void OnClickETC()
    {
        activeSlot = "ETC";

        SetButtonColors(ETC);

        // 기존의 아이템 슬롯들을 모두 제거
        foreach (Transform child in invenGridLayer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in inventoryItems)
        {
            if (inventoryItems[item.Key].itemType == ItemType.ETC)
                AddInvenSlot(item.Key, item.Value);
        }
    }

    // 버튼 색상을 설정하는 메소드
    private void SetButtonColors(Button activeButton)
    {
        // 모든 버튼을 기본 색상으로 설정
        ALL.GetComponent<Image>().color = defaultButtonColor;
        Equipment.GetComponent<Image>().color = defaultButtonColor;
        Consumable.GetComponent<Image>().color = defaultButtonColor;
        ETC.GetComponent<Image>().color = defaultButtonColor;

        // 활성화된 버튼만 특별한 색상으로 변경
        activeButton.GetComponent<Image>().color = activeButtonColor;
    }
    #endregion
    
    public void InvenUpdate()
    {
        playerMoney = PlayerItemContainer.Instance.PlayerMoney;
        TextMeso.text = playerMoney.ToString();

        switch (activeSlot)
        {
            case "ALL":
                foreach (Transform child in invenGridLayer)
                {
                    Destroy(child.gameObject);
                }

                foreach (var item in inventoryItems)
                {
                    AddInvenSlot(item.Key, item.Value);
                }
                break;
            case "Equipment":
                foreach (Transform child in invenGridLayer)
                {
                    Destroy(child.gameObject);
                }

                foreach (var item in inventoryItems)
                {
                    if (inventoryItems[item.Key].itemType == ItemType.Equipment)
                        AddInvenSlot(item.Key, item.Value);
                }
                break;
            case "Consumable":
                foreach (Transform child in invenGridLayer)
                {
                    Destroy(child.gameObject);
                }

                foreach (var item in inventoryItems)
                {
                    if (inventoryItems[item.Key].itemType == ItemType.Consumable)
                        AddInvenSlot(item.Key, item.Value);
                }
                break;
            case "ETC":
                // 기존의 아이템 슬롯들을 모두 제거
                foreach (Transform child in invenGridLayer)
                {
                    Destroy(child.gameObject);
                }

                foreach (var item in inventoryItems)
                {
                    if (inventoryItems[item.Key].itemType == ItemType.ETC)
                        AddInvenSlot(item.Key, item.Value);
                }
                break;
            default:
                break;
        }
    }
}
