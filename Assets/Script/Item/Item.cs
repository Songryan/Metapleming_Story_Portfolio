using System;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int price;
    public int reSell_price;
    public int quantity;
    public Sprite icon;
    public string description;
    public int[] stat_array;

    public enum ItemType { Equipment, Consumable, ETC }
    public ItemType itemType;

    public bool isEquipped = false;

    public enum EquipType { Armor, Pants, Arm }
    public EquipType equipType;

    // 아이템 사용시 호출될 델리게이트
    public Action onUse;
    public Action onPuton;
    public Action onTakeoff;

    public Item(string itemName, int price, Sprite icon, ItemType itemType)
    {
        this.itemName = itemName;
        this.price = price;
        this.icon = icon;
        this.itemType = itemType;
        this.quantity = 1;  // 디폴트 quantity는 1
        this.stat_array = new int[4];
    }

    // Equipment-specific properties
    public int attackBonus;
    public int defenseBonus;
    // ... and so on

    public virtual void Use()
    {
        // Define what happens when the item is used
        // 델리게이트를 통해 설정된 기능을 호출합니다.
        onUse?.Invoke();
    }
    public virtual void putOn()
    {
        // Define what happens when the item is used
        // 델리게이트를 통해 설정된 기능을 호출합니다.
        onPuton?.Invoke();
    }
    public virtual void takeOff()
    {
        // Define what happens when the item is used
        // 델리게이트를 통해 설정된 기능을 호출합니다.
        onTakeoff?.Invoke();
    }
}
