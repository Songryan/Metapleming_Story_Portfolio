using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public Item item; // ������ ������

    private SpriteRenderer spriteRenderer; // ��������Ʈ ������ �߰�

    private Rigidbody2D rb; // Rigidbody2D ������Ʈ
    public float attractSpeed = 5f; // �������� �÷��̾�� �������� �ӵ�
    private bool isAttracting = false; // �������� �÷��̾�� ���������� ����
    public Transform playerTransform; // �÷��̾��� ��ġ
    public float pickupDistance = 0.2f; // �Ⱦ��� Ȱ��ȭ�� �ּ� �Ÿ�

    private float originalY; // �������� �ʱ� Y ��ġ
    public float floatStrength = 0.3f; // ���� ����
    public float floatSpeed = 2f; // ���� �ӵ�

    public float disappearTime = 5f; // �������� ������������ �ð�

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // ��������Ʈ ������ ������Ʈ ��������
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D �߰�
        rb.isKinematic = true; // ���� ���� ���� ���� �ʰ� ����
        originalY = transform.position.y;

        // 5�� �� ������ �ڵ� �ı�
        Destroy(gameObject, disappearTime);
    }

    void Update()
    {
        if (isAttracting)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * attractSpeed; // �÷��̾�� �̵�

            // �÷��̾���� �Ÿ��� pickupDistance ���ϸ� �Ⱦ� ����
            if (Vector2.Distance(transform.position, playerTransform.position) <= pickupDistance)
            {
                Pickup(playerTransform.gameObject);
            }
        }

        // ���� ȿ�� ����
        float newY = originalY + Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // ��������Ʈ ������Ʈ�� ���� �޼ҵ�
    public void Setup(Item dropitem)
    {
        if (spriteRenderer != null && item != null)
        {
            spriteRenderer.sprite = dropitem.icon; // ������ ���������� ��������Ʈ ����
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform; // �÷��̾� ��ġ ����
            isAttracting = true; // �ڼ� ȿ�� Ȱ��ȭ
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttracting = false; // �ڼ� ȿ�� ��Ȱ��ȭ
            rb.velocity = Vector2.zero; // �ӵ� �ʱ�ȭ
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
            // �޼� ���� ������ �
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
                    // ������ �ִ� �������̶��
                    PlayerItemContainer.Instance.inventoryItems[item.itemName].quantity++;
                }
            }

        }
        Destroy(gameObject); // ������ ������Ʈ ����
    }

    // �κ��丮 ó�� ������ ������ �޼ҵ�� �и��Ͽ� ����
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
