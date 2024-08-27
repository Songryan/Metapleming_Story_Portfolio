using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class NPC : MonoBehaviour
{
    public Dictionary<string,Item> itemsForSale;

    [Header("Item Icon List")]
    public Sprite RedPotionIcon;
    public Sprite OrangePotionIcon;
    //public Sprite BluePotionIcon;

    void Start()
    {
        // �ʱ� ������ ��� ����
        itemsForSale = new Dictionary<string, Item>();

        // ������ �߰� ...
        Item RedPotion = new Item("RedPotion",50, RedPotionIcon, ItemType.Consumable);
        itemsForSale.Add("RedPotion", RedPotion);

        Item OrangePotion = new Item("OrangePotion", 80, OrangePotionIcon, ItemType.Consumable);
        itemsForSale.Add("OrangePotion", OrangePotion);

        //Item BluePotion = new Item("BluePotionIcon", 100, BluePotionIcon, ItemType.Consumable);
        //itemsForSale.Add("BluePotion", BluePotion);
    }

    // �Ǹ��� �������� �߰��ϴ� �޼ҵ�
    public void AddItemForSale(Item item)
    {
        itemsForSale.Add($"{item.itemName}", item);
    }
}

