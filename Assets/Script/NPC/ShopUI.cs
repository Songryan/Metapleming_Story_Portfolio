using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Item;
public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }   // InventoryManager의 싱글톤 인스턴스

    public GameObject ShopPanel; // 상점 패널

    [Header("Sell Prefab")]
    public GameObject Sell_Prefab;

    [Header("Buy Prefab")]
    public GameObject Buy_Prefab;
    public Dictionary<string, Item> inventoryItems;

    [Header("Player Item Info")]
    public int playerMoney; // 플레이어의 돈을 저장하는 변수

    public Transform itemSlotContainer; // GridLayoutGroup이 있는 곳
    public Transform itemBuySlotContainer; // GridLayoutGroup이 있는 곳

    public Dictionary<string, Item> itemsForSale;

    //[Header("Item Icon List")]
    //public Sprite RedPotionIcon;
    //public Sprite OrangePotionIcon;

    [Header("Shop Money")]
    //public int shopMoney; // 플레이어의 돈을 저장하는 변수
    public TextMeshProUGUI TextMeso;

    private void Awake()
    {
        inventoryItems = new Dictionary<string, Item>();
        inventoryItems = PlayerItemContainer.Instance.inventoryItems;

        playerMoney = PlayerItemContainer.Instance.PlayerMoney;
        TextMeso.text = PlayerItemContainer.Instance.PlayerMoney.ToString();

        ShopPanel.SetActive(false);

        // 싱글톤 인스턴스를 설정합니다.
        if (Instance == null)
        {
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
        
    }

    public void UpdateShop()
    {
        // 초기 아이템 목록 설정
        itemsForSale = new Dictionary<string, Item>();

        // 아이템 추가 ...
        Item RedPotion = ItemManager.Instance.RedPotion;
        itemsForSale.Add(RedPotion.itemName, RedPotion);

        Item OrangePotion = ItemManager.Instance.OrangePotion;
        itemsForSale.Add(OrangePotion.itemName, OrangePotion);

        Item BluePotion = ItemManager.Instance.BluePotion;
        itemsForSale.Add(BluePotion.itemName, BluePotion);

        Item Armor1 = ItemManager.Instance.Armor1;
        itemsForSale.Add(Armor1.itemName, Armor1);

        Item Armor2 = ItemManager.Instance.Armor2;
        itemsForSale.Add(Armor2.itemName, Armor2);

        Item Pants1 = ItemManager.Instance.Pants1;
        itemsForSale.Add(Pants1.itemName, Pants1);

        Item Pants2 = ItemManager.Instance.Pants2;
        itemsForSale.Add(Pants2.itemName, Pants2);

        Item Sword1 = ItemManager.Instance.Sword1;
        itemsForSale.Add(Sword1.itemName, Sword1);

        Item Sword2 = ItemManager.Instance.Sword2;
        itemsForSale.Add(Sword2.itemName, Sword2);

        // 기존의 아이템 슬롯들을 모두 제거
        foreach (Transform child in itemSlotContainer)
        {
            Destroy(child.gameObject);
        }

        // NPC의 판매 아이템을 새로운 슬롯으로 추가
        foreach (var item in itemsForSale)
        {
            AddItemSlot(item.Key, item.Value);
        }
    }
    public void AddItemSlot(string itemName, Item item)
    {
        var slot = Instantiate(Sell_Prefab, itemSlotContainer);
        slot.GetComponent<ItemUI>().Setup(item); // 아이템 데이터로 UI 설정
    }

    public void UpdateByeItem()
    {
        // 기존의 아이템 슬롯들을 모두 제거
        foreach (Transform child in itemBuySlotContainer)
        {
            Destroy(child.gameObject);
        }

        // NPC의 판매 아이템을 새로운 슬롯으로 추가
        foreach (var item in PlayerItemContainer.Instance.inventoryItems)
        {
            //Debug.Log(item.Key);
            // 슬롯추가전에 수량 TMP Quantity 적용.

            AddItemBuySlot(item.Key, item.Value);
        }
    }

    public void AddItemBuySlot(string itemName, Item item)
    {
        var slot = Instantiate(Buy_Prefab, itemBuySlotContainer);
        slot.GetComponent<ItemBuyUI>().Setup(item); // 아이템 데이터로 UI 설정
    }

    public void OnShopOpenClick()
    {
        ShopPanel.SetActive(true); // 패널 숨김
        UpdateShop();
        UpdateByeItem();
    }

    public void OnShopCloseClick()
    {
        ShopPanel.SetActive(false); // 패널 숨김
    }

    #region 구매 기능
    public void BuyItem(Item item)
    {
        // 아이템을 구매할 수 있는지 검사 (예: 충분한 돈이 있는지 등)
        if (CanBuyItem(item))
        {
            // 돈을 차감
            SpendMoney(item.price);
            // 아이템 추가
            AddItem(item);
            // 인벤토리 UI 업데이트는 AddItem에서 처리
        }
    }

    // 아이템을 살 수 있는지 판단
    public bool CanBuyItem(Item item)
    {
        if (PlayerItemContainer.Instance.PlayerMoney >= item.price)
        {
            return true;
        }
        return false;
    }

    // 아이템을 인벤토리에 추가하는 메소드
    public void AddItem(Item item)
    {
        if (!inventoryItems.ContainsKey(item.itemName))
        {
            inventoryItems.Add(item.itemName, item);
            // TODO: 인벤토리 UI 업데이트 로직을 호출합니다.
            // 예: InventoryUI.UpdateInventoryDisplay();

            // 인벤 초기화
            //OnClickALL();
            // 상점 초기화
            UpdateByeItem();
        }
        else
        {
            // 기존에 있는 아이템이라면
            inventoryItems[item.itemName].quantity++;
            //AddItemToUI(item);

            //인벤 초기화
            //OnClickALL();
            // 상점 초기화
            UpdateByeItem();
        }
    }
    #endregion
    #region 판매 기능
    public void SellItem(Item item)
    {
        // 아이템을 구매할 수 있는지 검사 (예: 충분한 돈이 있는지 등)
        if (CanSellItem(item))
        {
            // 돈을 추가
            AddMoney(item.reSell_price);
            // 아이템 차감
            SpendItem(item);
            // 인벤토리 UI 업데이트는 AddItem에서 처리
        }
    }

    // 아이템을 인벤토리에 추가하는 메소드
    public void SpendItem(Item item)
    {
        if (item.quantity > 1)
        {
            inventoryItems[item.itemName].quantity--;
            UpdateByeItem();
        }
        else
        {
            RemoveItem(item);
            UpdateByeItem();
        }
    }

    // 아이템을 구매할 수 있는지 검사 (예: 충분한 돈이 있는지 등)
    public bool CanSellItem(Item item)
    {
        if (item.quantity >= 1)
        {
            return true;
        }
        return false;
    }

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
    #endregion

    #region 메소 추가 사용

    // 플레이어의 돈을 추가하는 메소드
    public void AddMoney(int amount)
    {
        PlayerItemContainer.Instance.PlayerMoney += amount;
        // TODO: 돈의 UI 업데이트 로직을 호출합니다.
        // 예: MoneyUI.UpdateMoneyDisplay();
        TextMeso.text = PlayerItemContainer.Instance.PlayerMoney.ToString();
    }

    // 플레이어의 돈을 사용하는 메소드
    public bool SpendMoney(int amount)
    {
        if (PlayerItemContainer.Instance.PlayerMoney >= amount)
        {
            PlayerItemContainer.Instance.PlayerMoney -= amount;
            // TODO: 돈의 UI 업데이트 로직을 호출합니다.
            // 예: MoneyUI.UpdateMoneyDisplay();
            TextMeso.text = PlayerItemContainer.Instance.PlayerMoney.ToString();
            return true;
        }
        return false;
    }
    #endregion
}
