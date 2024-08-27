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
        // 초기 아이템 목록 설정
        itemsForSale = new Dictionary<string, Item>();

        // 아이템 추가 ...
        Item RedPotion = new Item("RedPotion",50, RedPotionIcon, ItemType.Consumable);
        itemsForSale.Add("RedPotion", RedPotion);

        Item OrangePotion = new Item("OrangePotion", 80, OrangePotionIcon, ItemType.Consumable);
        itemsForSale.Add("OrangePotion", OrangePotion);

        //Item BluePotion = new Item("BluePotionIcon", 100, BluePotionIcon, ItemType.Consumable);
        //itemsForSale.Add("BluePotion", BluePotion);
    }

    // 판매할 아이템을 추가하는 메소드
    public void AddItemForSale(Item item)
    {
        itemsForSale.Add($"{item.itemName}", item);
    }
}

