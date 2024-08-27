using System.Collections.Generic;
using UnityEngine;

public class SlimeMovement : MonoBehaviour
{
    public static SlimeMovement Instance { get; private set; }

    public float moveSpeed = 2.0f;
    public float moveDistance = 3.0f;
    public float hitPauseDuration =  0.5f;

    private bool movingRight = true;
    public bool isHit = false;
    private float hitTimer = 0f;
    private float leftEdge;
    private float rightEdge;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    //public GameObject healthBarPrefab; // ü�¹� ������
    //private GameObject healthBar; // ü�¹� �ν��Ͻ�
    public Transform healthBarBackground; // ü�¹� ���
    public Transform healthBarFill; // ü�¹� ä������ �κ�
    public Vector2 healthBarOffset = new Vector2(0, 1.0f);

    public int health = 50;

    private int maxHealth = 50;

    public int monsterEXP = 100;

    public DamageNumber damagePopupPrefab; // DamagePopup ������

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ü�� �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����Ѵٸ� �ߺ� ������ ���� ��ü�� �ı�
        }
    }

    void Start()
    {
        //damagePopupPrefab = GetComponent<DamageNumber>();
        leftEdge = transform.position.x - moveDistance;
        rightEdge = transform.position.x + moveDistance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // ü�¹� �ʱ� ��ġ ����
        InitializeHealthBar();
    }
    void InitializeHealthBar()
    {
        // ü�¹� ��濡 ���� �ʱ� ����
        healthBarBackground.localPosition = new Vector2(0, 0.3f); // ���� ���� ��ġ

        // ü�¹� ä��⸦ ����� ���� ���� ����ϴ�.
        healthBarFill.localPosition = new Vector2(-healthBarBackground.localScale.x / 2, healthBarBackground.localPosition.y);
    }

    void Update()
    {
        UpdateHealthBar();

        if (isHit)
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= hitPauseDuration)
            {
                isHit = false;
                hitTimer = 0f;
                animator.SetBool("IsHit", false);  // �ǰ� ���� ����
            }
            return;
        }

        Move();
    }

    void Move()
    {
        if (movingRight)
        {
            if (transform.position.x > rightEdge)
            {
                movingRight = false;
                spriteRenderer.flipX = false;
            }
            transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            if (transform.position.x < leftEdge)
            {
                movingRight = true;
                spriteRenderer.flipX = true;
            }
            transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
        }
    }

    public void TakeHit()
    {
        isHit = true;
        hitTimer = 0;
        animator.SetBool("IsHit", true);  // �ǰ� �ִϸ��̼� ����
    }

    public void TakeDamage(int damage)
    {
        // ������ �˾� ǥ��
        Vector3 spawnPosition = transform.position + new Vector3(0, 0.5f, 0); // ���� ��ġ�� ĳ���� ���� ����
        DamageNumber damagePopupInstance = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity);
        damagePopupInstance.ShowDamage(damage, spawnPosition);

        isHit = true;
        hitTimer = 0;
        health -= damage; // ���� ���ظ�ŭ ü�� ����
        animator.SetBool("IsHit", true);  // �ǰ� �ִϸ��̼� ����

        //UpdateHealthBar(); // ü�¹� ������Ʈ

        if (health <= 0)
        {
            Die(); // ü���� 0 ���ϸ� ��� ó��
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die"); // ��� �ִϸ��̼� ���
        GetComponent<Collider2D>().enabled = false; // �浹�� �����ϱ� ���� Collider ��Ȱ��ȭ
        GetComponent<m_ItemDrop>().DropItems();

        PlayerItemContainer.Instance.currentEXP += monsterEXP;
    }

    public void OnDieStart()
    {
        // �״� �ִϸ��̼� ���� ������ ����
        moveSpeed = 0;
    }

    public void OnDieEnd()
    {
        gameObject.SetActive(false); // ������Ʈ ��Ȱ��ȭ
        Invoke("ResetSlime", 1f); // 1�� �Ŀ� ResetSlime �޼ҵ� ȣ��
    }

    private void ResetSlime()
    {
        gameObject.SetActive(true); // ������Ʈ �ٽ� Ȱ��ȭ
        health = 100; // ü�� �ʱ�ȭ
        moveSpeed = 2.0f; // ������ �ӵ� �ʱ�ȭ
        animator.SetBool("IsHit", false); // �ִϸ��̼� ���� �ʱ�ȭ
        animator.ResetTrigger("Die");
        movingRight = true; // �ʱ� �������� �缳��
        transform.position = new Vector2(leftEdge, transform.position.y); // �ʱ� ��ġ�� ����
        GetComponent<Collider2D>().enabled = true; // Collider �ٽ� Ȱ��ȭ
    }


    void UpdateHealthBar()
    {
        // ü�¹� ������ ����
        float healthRatio = (float)health / maxHealth;
        healthBarFill.localScale = new Vector3(healthRatio * 2.0f, 0.2f, 1); // ���⼭ X �������� �ִ밪�� 2��

        // ü�¹��� ��ġ�� ���ʿ��� �����ϵ��� ����
        // ���� ���� ���������� ����ϹǷ�, X ��ġ�� ü�¹� ����� ���� ���� ���߰� �����մϴ�.
        healthBarFill.localPosition = new Vector2(-1.0f + healthRatio, healthBarOffset.y);
    }

    private Vector3 GetPopupPosition()
    {
        // ���� �������� ������ ���ڸ� ǥ���� ��ġ�� ��ȯ�մϴ�.
        // ������ ��ġ�� �������� �߰��ϰų�, UI ĵ�������� ��ġ�� ������ �� �ֽ��ϴ�.
        return transform.position + new Vector3(0, 1f, 0);
    }
}
