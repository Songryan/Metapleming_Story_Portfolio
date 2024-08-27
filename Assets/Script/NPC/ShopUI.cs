using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Item;
public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }   // InventoryManager�� �̱��� �ν��Ͻ�

    public GameObject ShopPanel; // ���� �г�

    [Header("Sell Prefab")]
    public GameObject Sell_Prefab;

    [Header("Buy Prefab")]
    public GameObject Buy_Prefab;
    public Dictionary<string, Item> inventoryItems;

    [Header("Player Item Info")]
    public int playerMoney; // �÷��̾��� ���� �����ϴ� ����

    public Transform itemSlotContainer; // GridLayoutGroup�� �ִ� ��
    public Transform itemBuySlotContainer; // GridLayoutGroup�� �ִ� ��

    public Dictionary<string, Item> itemsForSale;

    //[Header("Item Icon List")]
    //public Sprite RedPotionIcon;
    //public Sprite OrangePotionIcon;

    [Header("Shop Money")]
    //public int shopMoney; // �÷��̾��� ���� �����ϴ� ����
    public TextMeshProUGUI TextMeso;

    private void Awake()
    {
        inventoryItems = new Dictionary<string, Item>();
        inventoryItems = PlayerItemContainer.Instance.inventoryItems;

        playerMoney = PlayerItemContainer.Instance.PlayerMoney;
        TextMeso.text = PlayerItemContainer.Instance.PlayerMoney.ToString();

        ShopPanel.SetActive(false);

        // �̱��� �ν��Ͻ��� �����մϴ�.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� ������Ʈ�� �ı����� �ʵ��� �մϴ�.
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϴ� ��� ���� ������Ʈ�� �ı��մϴ�.
        }
    }

    private void Start()
    {
        
    }

    public void UpdateShop()
    {
        // �ʱ� ������ ��� ����
        itemsForSale = new Dictionary<string, Item>();

        // ������ �߰� ...
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

        // ������ ������ ���Ե��� ��� ����
        foreach (Transform child in itemSlotContainer)
        {
            Destroy(child.gameObject);
        }

        // NPC�� �Ǹ� �������� ���ο� �������� �߰�
        foreach (var item in itemsForSale)
        {
            AddItemSlot(item.Key, item.Value);
        }
    }
    public void AddItemSlot(string itemName, Item item)
    {
        var slot = Instantiate(Sell_Prefab, itemSlotContainer);
        slot.GetComponent<ItemUI>().Setup(item); // ������ �����ͷ� UI ����
    }

    public void UpdateByeItem()
    {
        // ������ ������ ���Ե��� ��� ����
        foreach (Transform child in itemBuySlotContainer)
        {
            Destroy(child.gameObject);
        }

        // NPC�� �Ǹ� �������� ���ο� �������� �߰�
        foreach (var item in PlayerItemContainer.Instance.inventoryItems)
        {
            //Debug.Log(item.Key);
            // �����߰����� ���� TMP Quantity ����.

            AddItemBuySlot(item.Key, item.Value);
        }
    }

    public void AddItemBuySlot(string itemName, Item item)
    {
        var slot = Instantiate(Buy_Prefab, itemBuySlotContainer);
        slot.GetComponent<ItemBuyUI>().Setup(item); // ������ �����ͷ� UI ����
    }

    public void OnShopOpenClick()
    {
        ShopPanel.SetActive(true); // �г� ����
        UpdateShop();
        UpdateByeItem();
    }

    public void OnShopCloseClick()
    {
        ShopPanel.SetActive(false); // �г� ����
    }

    #region ���� ���
    public void BuyItem(Item item)
    {
        // �������� ������ �� �ִ��� �˻� (��: ����� ���� �ִ��� ��)
        if (CanBuyItem(item))
        {
            // ���� ����
            SpendMoney(item.price);
            // ������ �߰�
            AddItem(item);
            // �κ��丮 UI ������Ʈ�� AddItem���� ó��
        }
    }

    // �������� �� �� �ִ��� �Ǵ�
    public bool CanBuyItem(Item item)
    {
        if (PlayerItemContainer.Instance.PlayerMoney >= item.price)
        {
            return true;
        }
        return false;
    }

    // �������� �κ��丮�� �߰��ϴ� �޼ҵ�
    public void AddItem(Item item)
    {
        if (!inventoryItems.ContainsKey(item.itemName))
        {
            inventoryItems.Add(item.itemName, item);
            // TODO: �κ��丮 UI ������Ʈ ������ ȣ���մϴ�.
            // ��: InventoryUI.UpdateInventoryDisplay();

            // �κ� �ʱ�ȭ
            //OnClickALL();
            // ���� �ʱ�ȭ
            UpdateByeItem();
        }
        else
        {
            // ������ �ִ� �������̶��
            inventoryItems[item.itemName].quantity++;
            //AddItemToUI(item);

            //�κ� �ʱ�ȭ
            //OnClickALL();
            // ���� �ʱ�ȭ
            UpdateByeItem();
        }
    }
    #endregion
    #region �Ǹ� ���
    public void SellItem(Item item)
    {
        // �������� ������ �� �ִ��� �˻� (��: ����� ���� �ִ��� ��)
        if (CanSellItem(item))
        {
            // ���� �߰�
            AddMoney(item.reSell_price);
            // ������ ����
            SpendItem(item);
            // �κ��丮 UI ������Ʈ�� AddItem���� ó��
        }
    }

    // �������� �κ��丮�� �߰��ϴ� �޼ҵ�
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

    // �������� ������ �� �ִ��� �˻� (��: ����� ���� �ִ��� ��)
    public bool CanSellItem(Item item)
    {
        if (item.quantity >= 1)
        {
            return true;
        }
        return false;
    }

    // �������� �κ��丮���� �����ϴ� �޼ҵ�
    public void RemoveItem(Item item)
    {
        if (inventoryItems.ContainsKey(item.itemName))
        {
            inventoryItems.Remove(item.itemName);
            // TODO: �κ��丮 UI ������Ʈ ������ ȣ���մϴ�.
            // ��: InventoryUI.UpdateInventoryDisplay();
        }
    }
    #endregion

    #region �޼� �߰� ���

    // �÷��̾��� ���� �߰��ϴ� �޼ҵ�
    public void AddMoney(int amount)
    {
        PlayerItemContainer.Instance.PlayerMoney += amount;
        // TODO: ���� UI ������Ʈ ������ ȣ���մϴ�.
        // ��: MoneyUI.UpdateMoneyDisplay();
        TextMeso.text = PlayerItemContainer.Instance.PlayerMoney.ToString();
    }

    // �÷��̾��� ���� ����ϴ� �޼ҵ�
    public bool SpendMoney(int amount)
    {
        if (PlayerItemContainer.Instance.PlayerMoney >= amount)
        {
            PlayerItemContainer.Instance.PlayerMoney -= amount;
            // TODO: ���� UI ������Ʈ ������ ȣ���մϴ�.
            // ��: MoneyUI.UpdateMoneyDisplay();
            TextMeso.text = PlayerItemContainer.Instance.PlayerMoney.ToString();
            return true;
        }
        return false;
    }
    #endregion
}
