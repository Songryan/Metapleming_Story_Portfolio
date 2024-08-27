using UnityEngine;
using static Item;

public class m_ItemDrop : MonoBehaviour
{
    public ItemDrop itemDropPrefab; // ������ ��� ������
    public Item[] itemsToDrop; // ����� ������ ����Ʈ
    public float spacing = 0.5f; // ������ ������ ����

    [Header("ETC Icon Image List")]
    public Sprite MesoIcon;
    public Sprite BranchIcon;
    public Sprite SlimeIcon;

    private void Awake()
    {
        itemsToDrop = new Item[]
        {
            new Item("Meso", 1, MesoIcon, ItemType.ETC)
            {
                quantity = 10
            },
            new Item("��������", 10, BranchIcon, ItemType.ETC)
            {
                stat_array = new int[4] { 0, 0, 0, 0 },
                description = "---------------------\r\n\r\n������ �ȸ� �ȴ�.",
                reSell_price = 7
            },
            new Item("������ ��ü", 15, SlimeIcon, ItemType.ETC)
            {
                stat_array = new int[4] { 0, 0, 0, 0 },
                description = "---------------------\r\n\r\n������ �ȸ� �ȴ�.",
                reSell_price = 11
            }
        };
    }

    public void DropItems()
    {
        float startX = transform.position.x - (spacing * (itemsToDrop.Length - 1)) / 2; // ù �������� x ��ġ ���
        for (int i = 0; i < itemsToDrop.Length; i++)
        {
            Vector2 dropPosition = new Vector2(startX + spacing * i, transform.position.y);
            ItemDrop drop = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
            drop.item = itemsToDrop[i]; // ����� ������ ����
            drop.Setup(itemsToDrop[i]); // ������ ��������Ʈ ���� ȣ��
        }
    }
}
