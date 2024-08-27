using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Item;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }   // InventoryManager�� �̱��� �ν��Ͻ�

    public Dictionary<string, Item> inventoryItems;

    public Transform invenGridLayer; // InvenGridLayer�� Transform�� ����

    public string activeSlot;
    
    public TextMeshProUGUI TextMeso;

    [Header("Prefab Item List")]
    public GameObject Inven_Prefab;

    [Header("Player Money")]
    public int playerMoney; // �÷��̾��� ���� �����ϴ� ����

    [Header("Button List")]
    public Button ALL;
    public Button Equipment;
    public Button Consumable;
    public Button ETC;
    
    //[Header("Color List")] // �� ��ư�� �⺻ ������ ������ ����
    private Color defaultButtonColor = new Color(255f/255f, 255f/255f, 255f/255f); // �⺻ ����
    private Color activeButtonColor = new Color(255f/255f, 71f/255f, 66f/255f);  // Ȱ��ȭ ����

    void Awake()
    {
        // �̱��� �ν��Ͻ��� �����մϴ�.
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
            DontDestroyOnLoad(gameObject); // ���� �ٲ� ������Ʈ�� �ı����� �ʵ��� �մϴ�.
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϴ� ��� ���� ������Ʈ�� �ı��մϴ�.
        }
    }

    private void Start()
    {
        OnClickALL();
    }

    

    #region �κ��丮 �߰� ���� ��� �߰�
    /*public void AddItemToUI(Item item)
    {
        // itemIconPrefab ��ųʸ��� ����Ͽ� ���ο� ������ ������ �ν��Ͻ��� �����մϴ�.
        GameObject itemIconInstance = Instantiate(itemIconPrefab[item.itemName], invenGridLayer);

        // ������ �ν��Ͻ� ���� QuantityText ������Ʈ�� ã�� ������ �����մϴ�.
        TextMeshProUGUI quantityText = itemIconInstance.transform.Find("quantity").GetComponent<TextMeshProUGUI>();
        if (quantityText != null)
        {
            quantityText.text = "X"+item.quantity.ToString(); // ������ ���ڿ��� ��ȯ�Ͽ� ����
        }

        // �߰����� ������ �ʿ��ϸ� ���⿡ �߰��մϴ�.
        // ���� ���, Ŭ�� �̺�Ʈ �����ʸ� �߰��� �� �ֽ��ϴ�.
    }*/

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

    // ������ ��� �޼ҵ�
    public void UseItem(Item item)
    {
        if (inventoryItems.ContainsKey(item.itemName))
        {
            item.Use();
            // ������ ��� �Ŀ��� �Ϲ������� �κ��丮���� ���ŵǰų� ������ �پ��ϴ�.
            // ��: RemoveItem(item);
        }
    }
    #endregion


    #region �κ� ��ư
    public void AddInvenSlot(string itemName, Item item)
    {
        var slot = Instantiate(Inven_Prefab, invenGridLayer);
        slot.GetComponent<ItemInvenUI>().Setup(item); // ������ �����ͷ� UI ����
    }
    
    public void OnClickALL()
    {
        activeSlot = "ALL";

        SetButtonColors(ALL);

        // ������ ������ ���Ե��� ��� ����
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

        // ������ ������ ���Ե��� ��� ����
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

        // ������ ������ ���Ե��� ��� ����
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

        // ������ ������ ���Ե��� ��� ����
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

    // ��ư ������ �����ϴ� �޼ҵ�
    private void SetButtonColors(Button activeButton)
    {
        // ��� ��ư�� �⺻ �������� ����
        ALL.GetComponent<Image>().color = defaultButtonColor;
        Equipment.GetComponent<Image>().color = defaultButtonColor;
        Consumable.GetComponent<Image>().color = defaultButtonColor;
        ETC.GetComponent<Image>().color = defaultButtonColor;

        // Ȱ��ȭ�� ��ư�� Ư���� �������� ����
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
                // ������ ������ ���Ե��� ��� ����
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
