using UnityEngine;
using System.Collections;

public class p_AttackedProcess : MonoBehaviour
{
    public float knockbackForce = 2.7f;  // 반동 힘의 크기
    public float knockbackDuration = 0.4f;  // 반동 지속 시간
    public float invincibleDuration = 3.0f;  // 무적 시간
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;  // 애니메이터 컴포넌트 추가

    public DamageNumber damagePopupPrefab; // DamagePopup 프리팹

    public Sprite defaultSprite; // 기본 스프라이트
    public Sprite deadSprite; // 사망 시 사용할 스프라이트
    public float deathDuration = 5.0f; // 사망 상태 지속 시간
    private Vector2 originalPosition;  // 캐릭터의 원래 위치 저장

    public GameObject Tomb;
    public Animator DieAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();  // 애니메이터 컴포넌트 할당
        originalPosition = transform.position;  // 시작 위치 저장
    }

    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        // 데미지 팝업 표시
        Vector3 spawnPosition = transform.position + new Vector3(0, 0.5f, 0); // 생성 위치를 캐릭터 위로 조정
        DamageNumber damagePopupInstance = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity);
        damagePopupInstance.ShowDamage(damage, spawnPosition);

        PlayerItemContainer.Instance.currentHP -= damage;
        Vector2 knockbackDir = ((Vector2)transform.position - attackDirection).normalized + Vector2.up;
        knockbackDir.Normalize();
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        //StartCoroutine(ResetVelocityAfterDelay(knockbackDuration));
        //StartCoroutine(BecomeInvincible(invincibleDuration));

        if (PlayerItemContainer.Instance.currentHP <= 0)
        {
            PlayerItemContainer.Instance.currentHP = 0;
            Die();
        }
        else
        {
            StartCoroutine(ResetVelocityAfterDelay(knockbackDuration));
            StartCoroutine(BecomeInvincible(invincibleDuration));
        }
    }

    IEnumerator ResetVelocityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector2.zero;
    }

    IEnumerator BecomeInvincible(float duration)
    {
        // 레이어 변경을 통한 충돌 무시
        int playerLayer = gameObject.layer;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // 투명도 조절
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

        yield return new WaitForSeconds(duration);

        // 투명도 복원 및 충돌 설정 복원
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // 사망 애니메이션 및 게임 오버 처리

        // 경험치 다운 처리
        expDownProcess();

        animator.enabled = false;  // 애니메이터 비활성화
        // 사망 시 스프라이트 변경
        spriteRenderer.sprite = deadSprite;

        // 모든 이동 및 애니메이션 중지
        rb.velocity = Vector2.zero;
        rb.isKinematic = true; // 물리 엔진 영향 제거

        // 5초 후에 원래 상태로 복구
        Vector3 deathPosition = transform.position;
        StartCoroutine(DropTomb(deathPosition)); // 코루틴 호출
        StartCoroutine(ResetAfterDeath());
        StartCoroutine(BecomeInvincible(deathDuration));
    }

    IEnumerator ResetAfterDeath()
    {
        DieAnimator.SetBool("TombBool", true); // 애니메이션 시작
        // 5초간 대기
        yield return new WaitForSeconds(deathDuration);
        DieAnimator.SetBool("TombBool", false); // 애니메이션 종료
        Tomb.SetActive(false);

        // 사망 상태 종료 후 초기화
        spriteRenderer.sprite = defaultSprite;
        rb.isKinematic = false; // 물리 엔진 영향 다시 적용
        //transform.position = originalPosition;
        animator.enabled = true;  // 애니메이터 다시 활성화

        PlayerItemContainer.Instance.currentHP = PlayerItemContainer.Instance.HP;
        PlayerItemContainer.Instance.currentMP = PlayerItemContainer.Instance.MP;

        Debug.Log("Player Reset Complete!");
    }

    IEnumerator DropTomb(Vector3 startPosition)
    {
        float dropDuration = 1.0f; // 떨어지는 데 걸리는 시간
        float elapsedTime = 0;
        float dropHeight = 6.0f; // 시작 높이 (플레이어 위치로부터 Y축으로 얼마나 떨어질지)

        Vector3 endPosition = transform.position; // 최종 위치 (플레이어의 현재 위치)
        Vector3 currentPosition = startPosition + new Vector3(0, dropHeight, 0); // 시작 위치 (플레이어 위치의 Y축 위)

        Tomb.transform.position = currentPosition; // 시작 위치에서 Tomb 위치 설정
        Tomb.SetActive(true);
        Tomb.transform.SetParent(null); // 계층 구조에서 분리

        while (elapsedTime < dropDuration)
        {
            // 선형 보간을 사용하여 부드럽게 위치를 변경
            Tomb.transform.position = Vector3.Lerp(currentPosition, endPosition, elapsedTime / dropDuration);
            elapsedTime += Time.deltaTime; // 시간 업데이트
            yield return null; // 다음 프레임까지 대기
        }

        Tomb.transform.position = endPosition; // 최종 위치 확정
        DieAnimator.SetBool("TombBool", true); // 떨어진 후의 애니메이션 시작
    }

    public void expDownProcess()
    {
        int cExp = PlayerItemContainer.Instance.currentEXP;
        int Level = PlayerItemContainer.Instance.Level;

        // 레벨당 50씩 차감
        if (cExp >= Level * 50)
            PlayerItemContainer.Instance.currentEXP -= Level * 50;
        else
            PlayerItemContainer.Instance.currentEXP = 0;
    }
}
