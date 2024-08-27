using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Item;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }   // ItemManager�� �̱��� �ν��Ͻ�

    [Header("Equipment Item Object List")]
    public Item Armor1;
    public Item Armor2;
    public Item Pants1;
    public Item Pants2;
    public Item Sword1;
    public Item Sword2;

    [Header("Equipment Icon Image List")]
    public Sprite Armor1Icon;
    public Sprite Armor2Icon;
    public Sprite Pants1Icon;
    public Sprite Pants2Icon;
    public Sprite Sword1Icon;
    public Sprite Sword2Icon;

    [Header("ETC Item Object List")]
    public Item Meso;
    //public Item OrangePotion;

    [Header("ETC Icon Image List")]
    public Sprite MesoIcon;
    //public Sprite OrangePotionIcon;

    [Header("Consumable Item Object List")]
    public Item RedPotion;
    public Item OrangePotion;
    public Item BluePotion;

    [Header("Consumable Icon Image List")]
    public Sprite RedPotionIcon;
    public Sprite OrangePotionIcon;
    public Sprite BluePotionIcon;
    // Start is called before the first frame update

    [Header("NickName Setting")]
    [SerializeField] private TextMeshProUGUI tmpUI;

    void Awake()
    {
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
        tmpUI.text = PlayerManager.Instance.UserNickName;

        #region Equipment
        Armor1 = new Item("�Ͼ� Ƽ����", 10, Armor1Icon, ItemType.Equipment)
        {
            equipType = EquipType.Armor,
            stat_array = new int[4] { 1,1,0,0 },
            description = "---------------------\r\n\r\n�⺻���� ����.",
            reSell_price = 7
        };
        Armor1.onTakeoff = () => {
            Armor1.isEquipped = false;
            PlayerItemContainer.Instance.STR -= Armor1.stat_array[0];
            PlayerItemContainer.Instance.DEX -= Armor1.stat_array[1];
            PlayerItemContainer.Instance.INT -= Armor1.stat_array[2];
            PlayerItemContainer.Instance.LUK -= Armor1.stat_array[3];
        };
        Armor1.onPuton = () => {
            foreach (var item in PlayerItemContainer.Instance.inventoryItems)
            {
                if (item.Value.equipType == Armor1.equipType &&
                    item.Value.itemType == Armor1.itemType &&
                    item.Value.isEquipped
                   )
                {
                    item.Value.takeOff();
                    //item.Value.isEquipped = false;
                }
            }
            Armor1.isEquipped = true;
            PlayerItemContainer.Instance.STR += Armor1.stat_array[0];
            PlayerItemContainer.Instance.DEX += Armor1.stat_array[1];
            PlayerItemContainer.Instance.INT += Armor1.stat_array[2];
            PlayerItemContainer.Instance.LUK += Armor1.stat_array[3];
        };

        Armor2 = new Item("�߽� ������ ����", 500, Armor2Icon, ItemType.Equipment)
        {
            equipType = EquipType.Armor,
            stat_array = new int[4] { 1, 1, 1, 1 },
            description = "---------------------\r\n\r\n�߽� ������� ����.",
            reSell_price = 350
        };
        Armor2.onTakeoff = () => {
            Armor2.isEquipped = false;
            PlayerItemContainer.Instance.STR -= Armor2.stat_array[0];
            PlayerItemContainer.Instance.DEX -= Armor2.stat_array[1];
            PlayerItemContainer.Instance.INT -= Armor2.stat_array[2];
            PlayerItemContainer.Instance.LUK -= Armor2.stat_array[3];
        };
        Armor2.onPuton = () => {
            foreach (var item in PlayerItemContainer.Instance.inventoryItems)
            {
                if (item.Value.equipType == Armor2.equipType &&
                    item.Value.itemType == Armor2.itemType &&
                    item.Value.isEquipped
                   )
                {
                    item.Value.takeOff();
                    //item.Value.isEquipped = false;
                }
            }
            Armor2.isEquipped = true;
            PlayerItemContainer.Instance.STR += Armor2.stat_array[0];
            PlayerItemContainer.Instance.DEX += Armor2.stat_array[1];
            PlayerItemContainer.Instance.INT += Armor2.stat_array[2];
            PlayerItemContainer.Instance.LUK += Armor2.stat_array[3];
        };

        Pants1 = new Item("û����", 10, Pants1Icon, ItemType.Equipment)
        {
            equipType = EquipType.Pants,
            stat_array = new int[4] { 1, 1, 0, 0 },
            description = "---------------------\r\n\r\n�⺻���� ����.",
            reSell_price = 7
        };
        Pants1.onTakeoff = () => {
            Pants1.isEquipped = false;
            PlayerItemContainer.Instance.STR -= Pants1.stat_array[0];
            PlayerItemContainer.Instance.DEX -= Pants1.stat_array[1];
            PlayerItemContainer.Instance.INT -= Pants1.stat_array[2];
            PlayerItemContainer.Instance.LUK -= Pants1.stat_array[3];
        };
        Pants1.onPuton = () => {
            foreach (var item in PlayerItemContainer.Instance.inventoryItems)
            {
                if (item.Value.equipType == Pants1.equipType &&
                    item.Value.itemType == Pants1.itemType &&
                    item.Value.isEquipped
                   )
                {
                    item.Value.takeOff();
                    //item.Value.isEquipped = false;
                }
            }
            Pants1.isEquipped = true;
            PlayerItemContainer.Instance.STR += Pants1.stat_array[0];
            PlayerItemContainer.Instance.DEX += Pants1.stat_array[1];
            PlayerItemContainer.Instance.INT += Pants1.stat_array[2];
            PlayerItemContainer.Instance.LUK += Pants1.stat_array[3];
        };

        Pants2 = new Item("�߽� ������ ����", 450, Pants2Icon, ItemType.Equipment)
        {
            equipType = EquipType.Pants,
            stat_array = new int[4] { 1, 1, 1, 1 },
            description = "---------------------\r\n\r\n�߽� ������� ����.",
            reSell_price = 315
        };
        Pants2.onTakeoff = () => {
            Pants2.isEquipped = false;
            PlayerItemContainer.Instance.STR -= Pants2.stat_array[0];
            PlayerItemContainer.Instance.DEX -= Pants2.stat_array[1];
            PlayerItemContainer.Instance.INT -= Pants2.stat_array[2];
            PlayerItemContainer.Instance.LUK -= Pants2.stat_array[3];
        };
        Pants2.onPuton = () => {
            foreach (var item in PlayerItemContainer.Instance.inventoryItems)
            {
                if (item.Value.equipType == Pants2.equipType &&
                    item.Value.itemType == Pants2.itemType &&
                    item.Value.isEquipped
                   )
                {
                    item.Value.takeOff();
                    //item.Value.isEquipped = false;
                }
            }
            Pants2.isEquipped = true;
            PlayerItemContainer.Instance.STR += Pants2.stat_array[0];
            PlayerItemContainer.Instance.DEX += Pants2.stat_array[1];
            PlayerItemContainer.Instance.INT += Pants2.stat_array[2];
            PlayerItemContainer.Instance.LUK += Pants2.stat_array[3];
        };

        Sword1 = new Item("�Ϲ� ��", 300, Sword1Icon, ItemType.Equipment)
        {
            equipType = EquipType.Arm,
            stat_array = new int[4] { 2, 2, 0, 0 },
            description = "---------------------\r\n\r\n�⺻���� ���̴�.",
            reSell_price = 210
        };
        Sword1.onTakeoff = () => {
            Sword1.isEquipped = false;
            PlayerItemContainer.Instance.STR -= Sword1.stat_array[0];
            PlayerItemContainer.Instance.DEX -= Sword1.stat_array[1];
            PlayerItemContainer.Instance.INT -= Sword1.stat_array[2];
            PlayerItemContainer.Instance.LUK -= Sword1.stat_array[3];
        };
        Sword1.onPuton = () => {
            foreach (var item in PlayerItemContainer.Instance.inventoryItems)
            {
                if (item.Value.equipType == Sword1.equipType &&
                    item.Value.itemType == Sword1.itemType &&
                    item.Value.isEquipped
                   )
                {
                    item.Value.takeOff();
                    //item.Value.isEquipped = false;
                }
            }
            Sword1.isEquipped = true;
            PlayerItemContainer.Instance.STR += Sword1.stat_array[0];
            PlayerItemContainer.Instance.DEX += Sword1.stat_array[1];
            PlayerItemContainer.Instance.INT += Sword1.stat_array[2];
            PlayerItemContainer.Instance.LUK += Sword1.stat_array[3];
        };

        Sword2 = new Item("1500¥�� ��", 1500, Sword2Icon, ItemType.Equipment)
        {
            equipType = EquipType.Arm,
            stat_array = new int[4] { 5, 5, 0, 0 },
            description = "---------------------\r\n\r\n���̴�.",
            reSell_price = 1050
        };
        Sword2.onTakeoff = () => {
            Sword2.isEquipped = false;
            PlayerItemContainer.Instance.STR -= Sword2.stat_array[0];
            PlayerItemContainer.Instance.DEX -= Sword2.stat_array[1];
            PlayerItemContainer.Instance.INT -= Sword2.stat_array[2];
            PlayerItemContainer.Instance.LUK -= Sword2.stat_array[3];
        };
        Sword2.onPuton = () => {
            foreach (var item in PlayerItemContainer.Instance.inventoryItems)
            {
                if (item.Value.equipType == Sword2.equipType &&
                    item.Value.itemType == Sword2.itemType &&
                    item.Value.isEquipped
                   )
                {
                    item.Value.takeOff();
                    //item.Value.isEquipped = false;
                }
            }
            Sword2.isEquipped = true;
            PlayerItemContainer.Instance.STR += Sword2.stat_array[0];
            PlayerItemContainer.Instance.DEX += Sword2.stat_array[1];
            PlayerItemContainer.Instance.INT += Sword2.stat_array[2];
            PlayerItemContainer.Instance.LUK += Sword2.stat_array[3];
        };

        #endregion

        #region Consumable
        RedPotion = new Item("���� ����", 50, RedPotionIcon, ItemType.Consumable)
        {
            stat_array = new int[4] { 0, 0, 0, 0 },
            description = "---------------------\r\n\r\nü���� 50 ȸ���Ѵ�.",
            reSell_price = 35
        };
        RedPotion.onUse = () => {
            int p_HP = PlayerItemContainer.Instance.HP;
            int p_currentHP = PlayerItemContainer.Instance.currentHP;


            if (p_HP >= p_currentHP + 50)
                PlayerItemContainer.Instance.currentHP+=50;
            else
                PlayerItemContainer.Instance.currentHP = PlayerItemContainer.Instance.HP;

            if (RedPotion.quantity > 1)
            {
                PlayerItemContainer.Instance.inventoryItems[RedPotion.itemName].quantity--;
            }
            else
            {
                if (PlayerItemContainer.Instance.inventoryItems.ContainsKey(RedPotion.itemName))
                {
                    PlayerItemContainer.Instance.inventoryItems.Remove(RedPotion.itemName);
                }
            }
        };

        OrangePotion = new Item("��Ȳ ����", 150, OrangePotionIcon, ItemType.Consumable)
        {
            stat_array = new int[4] { 0, 0, 0, 0 },
            description = "---------------------\r\n\r\nü���� 100 ȸ���Ѵ�.",
            reSell_price = 105
        };
        OrangePotion.onUse = () => {
            int p_HP = PlayerItemContainer.Instance.HP;
            int p_currentHP = PlayerItemContainer.Instance.currentHP;

            if (p_HP >= p_currentHP + 100)
                PlayerItemContainer.Instance.currentHP+=100;
            else
                PlayerItemContainer.Instance.currentHP = PlayerItemContainer.Instance.HP;

            if (OrangePotion.quantity > 1)
            {
                PlayerItemContainer.Instance.inventoryItems[OrangePotion.itemName].quantity--;
            }
            else
            {
                if (PlayerItemContainer.Instance.inventoryItems.ContainsKey(OrangePotion.itemName))
                {
                    PlayerItemContainer.Instance.inventoryItems.Remove(OrangePotion.itemName);
                }
            }
        };

        BluePotion = new Item("�Ķ� ����", 80, BluePotionIcon, ItemType.Consumable)
        {
            stat_array = new int[4] { 0, 0, 0, 0 },
            description = "---------------------\r\n\r\n������ 100 ȸ���Ѵ�.",
            reSell_price = 56
        };
        BluePotion.onUse = () => {
            int p_MP = PlayerItemContainer.Instance.MP;
            int p_currentMP = PlayerItemContainer.Instance.currentMP;

            if (p_MP >= p_currentMP + 100)
                PlayerItemContainer.Instance.currentMP+=100;
            else
                PlayerItemContainer.Instance.currentMP = PlayerItemContainer.Instance.MP;

            if (BluePotion.quantity > 1)
            {
                PlayerItemContainer.Instance.inventoryItems[BluePotion.itemName].quantity--;
            }
            else
            {
                if (PlayerItemContainer.Instance.inventoryItems.ContainsKey(BluePotion.itemName))
                {
                    PlayerItemContainer.Instance.inventoryItems.Remove(BluePotion.itemName);
                }
            }
        };
        #endregion

        #region ETC
        Meso = new Item("Meso", 1, MesoIcon, ItemType.ETC)
        {
            //stat_array = new int[4] { 0, 0, 0, 0 },
            //description = "---------------------\r\n\r\nü���� 50 ȸ���Ѵ�.",
            //reSell_price = 35
        };
        #endregion
    }

}
