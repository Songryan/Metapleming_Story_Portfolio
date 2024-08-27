using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public Item item; // 아이템 데이터

    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러 추가

    private Rigidbody2D rb; // Rigidbody2D 컴포넌트
    public float attractSpeed = 5f; // 아이템이 플레이어에게 끌려가는 속도
    private bool isAttracting = false; // 아이템이 플레이어에게 끌려가는지 여부
    public Transform playerTransform; // 플레이어의 위치
    public float pickupDistance = 0.2f; // 픽업을 활성화할 최소 거리

    private float originalY; // 아이템의 초기 Y 위치
    public float floatStrength = 0.3f; // 부유 강도
    public float floatSpeed = 2f; // 부유 속도

    public float disappearTime = 5f; // 아이템이 사라지기까지의 시간

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 추가
        rb.isKinematic = true; // 물리 엔진 영향 받지 않게 설정
        originalY = transform.position.y;

        // 5초 후 아이템 자동 파괴
        Destroy(gameObject, disappearTime);
    }

    void Update()
    {
        if (isAttracting)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * attractSpeed; // 플레이어에게 이동

            // 플레이어와의 거리가 pickupDistance 이하면 픽업 실행
            if (Vector2.Distance(transform.position, playerTransform.position) <= pickupDistance)
            {
                Pickup(playerTransform.gameObject);
            }
        }

        // 부유 효과 로직
        float newY = originalY + Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // 스프라이트 업데이트를 위한 메소드
    public void Setup(Item dropitem)
    {
        if (spriteRenderer != null && item != null)
        {
            spriteRenderer.sprite = dropitem.icon; // 아이템 아이콘으로 스프라이트 설정
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform; // 플레이어 위치 저장
            isAttracting = true; // 자석 효과 활성화
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttracting = false; // 자석 효과 비활성화
            rb.velocity = Vector2.zero; // 속도 초기화
        }
    }

    private void Pickup(GameObject player)
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.gameObject.activeSelf)
        {
            ProcessInventoryLogic();
        }
        else
        {
            // 메소 로직 별도로 운영
            if (item.itemName.Equals("Meso"))
            {
                PlayerItemContainer.Instance.playerMoney += item.quantity;
            }
            else
            {
                if (!PlayerItemContainer.Instance.inventoryItems.ContainsKey(item.itemName))
                {
                    PlayerItemContainer.Instance.inventoryItems.Add(item.itemName, item);
                }
                else
                {
                    // 기존에 있는 아이템이라면
                    PlayerItemContainer.Instance.inventoryItems[item.itemName].quantity++;
                }
            }

        }
        Destroy(gameObject); // 아이템 오브젝트 제거
    }

    // 인벤토리 처리 로직을 별도의 메소드로 분리하여 관리
    private void ProcessInventoryLogic()
    {
        if (item.itemName.Equals("Meso"))
        {
            PlayerItemContainer.Instance.playerMoney += item.quantity;
        }
        else
        {
            if (!PlayerItemContainer.Instance.inventoryItems.ContainsKey(item.itemName))
            {
                PlayerItemContainer.Instance.inventoryItems.Add(item.itemName, item);
            }
            else
            {
                PlayerItemContainer.Instance.inventoryItems[item.itemName].quantity++;
            }
        }
        InventoryManager.Instance.InvenUpdate();
    }
}
