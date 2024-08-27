using UnityEngine;
using static Item;

public class m_ItemDrop : MonoBehaviour
{
    public ItemDrop itemDropPrefab; // 아이템 드롭 프리팹
    public Item[] itemsToDrop; // 드롭할 아이템 리스트
    public float spacing = 0.5f; // 아이템 사이의 간격

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
            new Item("나뭇가지", 10, BranchIcon, ItemType.ETC)
            {
                stat_array = new int[4] { 0, 0, 0, 0 },
                description = "---------------------\r\n\r\n상점에 팔면 된다.",
                reSell_price = 7
            },
            new Item("슬라임 액체", 15, SlimeIcon, ItemType.ETC)
            {
                stat_array = new int[4] { 0, 0, 0, 0 },
                description = "---------------------\r\n\r\n상점에 팔면 된다.",
                reSell_price = 11
            }
        };
    }

    public void DropItems()
    {
        float startX = transform.position.x - (spacing * (itemsToDrop.Length - 1)) / 2; // 첫 아이템의 x 위치 계산
        for (int i = 0; i < itemsToDrop.Length; i++)
        {
            Vector2 dropPosition = new Vector2(startX + spacing * i, transform.position.y);
            ItemDrop drop = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
            drop.item = itemsToDrop[i]; // 드롭할 아이템 설정
            drop.Setup(itemsToDrop[i]); // 아이템 스프라이트 설정 호출
        }
    }
}
