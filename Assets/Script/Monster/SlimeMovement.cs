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

    //public GameObject healthBarPrefab; // 체력바 프리팹
    //private GameObject healthBar; // 체력바 인스턴스
    public Transform healthBarBackground; // 체력바 배경
    public Transform healthBarFill; // 체력바 채워지는 부분
    public Vector2 healthBarOffset = new Vector2(0, 1.0f);

    public int health = 50;

    private int maxHealth = 50;

    public int monsterEXP = 100;

    public DamageNumber damagePopupPrefab; // DamagePopup 프리팹

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 객체를 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재한다면 중복 생성된 현재 객체를 파괴
        }
    }

    void Start()
    {
        //damagePopupPrefab = GetComponent<DamageNumber>();
        leftEdge = transform.position.x - moveDistance;
        rightEdge = transform.position.x + moveDistance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // 체력바 초기 위치 설정
        InitializeHealthBar();
    }
    void InitializeHealthBar()
    {
        // 체력바 배경에 대한 초기 설정
        healthBarBackground.localPosition = new Vector2(0, 0.3f); // 몬스터 위에 위치

        // 체력바 채우기를 배경의 왼쪽 끝에 맞춥니다.
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
                animator.SetBool("IsHit", false);  // 피격 상태 종료
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
        animator.SetBool("IsHit", true);  // 피격 애니메이션 시작
    }

    public void TakeDamage(int damage)
    {
        // 데미지 팝업 표시
        Vector3 spawnPosition = transform.position + new Vector3(0, 0.5f, 0); // 생성 위치를 캐릭터 위로 조정
        DamageNumber damagePopupInstance = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity);
        damagePopupInstance.ShowDamage(damage, spawnPosition);

        isHit = true;
        hitTimer = 0;
        health -= damage; // 받은 피해만큼 체력 감소
        animator.SetBool("IsHit", true);  // 피격 애니메이션 시작

        //UpdateHealthBar(); // 체력바 업데이트

        if (health <= 0)
        {
            Die(); // 체력이 0 이하면 사망 처리
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die"); // 사망 애니메이션 재생
        GetComponent<Collider2D>().enabled = false; // 충돌을 방지하기 위해 Collider 비활성화
        GetComponent<m_ItemDrop>().DropItems();

        PlayerItemContainer.Instance.currentEXP += monsterEXP;
    }

    public void OnDieStart()
    {
        // 죽는 애니메이션 동안 움직임 중지
        moveSpeed = 0;
    }

    public void OnDieEnd()
    {
        gameObject.SetActive(false); // 오브젝트 비활성화
        Invoke("ResetSlime", 1f); // 1초 후에 ResetSlime 메소드 호출
    }

    private void ResetSlime()
    {
        gameObject.SetActive(true); // 오브젝트 다시 활성화
        health = 100; // 체력 초기화
        moveSpeed = 2.0f; // 움직임 속도 초기화
        animator.SetBool("IsHit", false); // 애니메이션 상태 초기화
        animator.ResetTrigger("Die");
        movingRight = true; // 초기 방향으로 재설정
        transform.position = new Vector2(leftEdge, transform.position.y); // 초기 위치로 리셋
        GetComponent<Collider2D>().enabled = true; // Collider 다시 활성화
    }


    void UpdateHealthBar()
    {
        // 체력바 스케일 조정
        float healthRatio = (float)health / maxHealth;
        healthBarFill.localScale = new Vector3(healthRatio * 2.0f, 0.2f, 1); // 여기서 X 스케일은 최대값의 2배

        // 체력바의 위치를 왼쪽에서 시작하도록 조정
        // 왼쪽 끝을 기준점으로 사용하므로, X 위치를 체력바 배경의 왼쪽 끝에 맞추고 조정합니다.
        healthBarFill.localPosition = new Vector2(-1.0f + healthRatio, healthBarOffset.y);
    }

    private Vector3 GetPopupPosition()
    {
        // 월드 공간에서 데미지 숫자를 표시할 위치를 반환합니다.
        // 몬스터의 위치에 오프셋을 추가하거나, UI 캔버스에서 위치를 설정할 수 있습니다.
        return transform.position + new Vector3(0, 1f, 0);
    }
}
