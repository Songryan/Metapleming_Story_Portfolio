using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipMenu : MonoBehaviour
{

    public Image Arm;
    public Image Armor;
    public Image Pants;

    // Start is called before the first frame update
    void Awake()
    {
        Arm = transform.Find("ARM").GetComponent<Image>();
        Armor = transform.Find("ARMOR").GetComponent<Image>();
        Pants = transform.Find("PANTS").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        EqiptMenuUpdate();
    }

    public void EqiptMenuUpdate()
    {
        bool isArmEquipped = false;
        bool isArmorEquipped = false;
        bool isPantsEquipped = false;

        foreach (var item in PlayerItemContainer.Instance.inventoryItems)
        {
            Debug.Log("이건타지?");
            // Arm에 대한 처리
            if (item.Value.equipType == Item.EquipType.Arm)
            {
                if (item.Value.isEquipped)
                {
                    Arm.sprite = item.Value.icon;
                    Arm.color = new Color(1, 1, 1, 1);
                    isArmEquipped = true;
                }
            }

            // Armor에 대한 처리
            if (item.Value.equipType == Item.EquipType.Armor)
            {
                if (item.Value.isEquipped)
                {
                    Armor.sprite = item.Value.icon;
                    Armor.color = new Color(1, 1, 1, 1);
                    isArmorEquipped = true;
                }
            }

            // Pants에 대한 처리
            if (item.Value.equipType == Item.EquipType.Pants)
            {
                if (item.Value.isEquipped)
                {
                    Pants.sprite = item.Value.icon;
                    Pants.color = new Color(1, 1, 1, 1);
                    isPantsEquipped = true;
                }
            }
        }

        // 각 장비의 장착 여부에 따라 이미지 표시 여부 결정
        if (!isArmEquipped)
        {
            Arm.color = new Color(1, 1, 1, 0);
        }
        if (!isArmorEquipped)
        {
            Armor.color = new Color(1, 1, 1, 0);
        }
        if (!isPantsEquipped)
        {
            Pants.color = new Color(1, 1, 1, 0);
        }
    }
}
