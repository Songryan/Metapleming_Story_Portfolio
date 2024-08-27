using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public InputActionAsset InputActionAsset;
    private InputAction skillAction;

    public Animator animator;
    public Transform firePoint;
    public LayerMask enemyLayers;
    public float skillRange = 5f;
    public int skillDamage;
    public float skillCooldown = 3f;

    private float skillTimer;
    private SpriteRenderer spriteRenderer; // SpriteRenderer 컴포넌트 참조

    public Animator effectAnimator;
    public GameObject effectAnimatorObject;

    private Rigidbody2D rb; // Rigidbody2D 컴포넌트 참조
    public bool isCastingSkill = false; // 스킬 사용 중인지 확인하는 플래그

    public Image skillCooldownBar; // 스킬 쿨다운을 표시할 UI 바

    private void Awake()
    {
        var InputActionMap = InputActionAsset.FindActionMap("Player", true);
        skillAction = InputActionMap.FindAction("Skill", true);
        skillAction.Enable();
        skillAction.performed += SkillAction;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();  // SpriteRenderer 컴포넌트 가져오기
        //effectAnimator.SetBool("EffectTrigger", true);
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
    }

    void Update()
    {
        // 스킬 사용 중이 아닐 때만 skillTimer 감소
        if (!isCastingSkill)
        {
            if (skillTimer > 0)
            {
                skillTimer -= Time.deltaTime;
                // 스킬 쿨다운 바 업데이트
                if (skillCooldownBar != null)
                {
                    skillCooldownBar.fillAmount = skillTimer / skillCooldown;
                }
            }
            else
            {
                // 쿨다운이 끝났을 때 스킬을 다시 사용할 수 있도록 리셋
                isCastingSkill = false;
                skillCooldownBar.fillAmount = 0; // 쿨다운 바 비움
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

    }

    public void SkillAction(InputAction.CallbackContext context)
    {
        //스킬 마나 소모와 0일때 못쓰는 판정
        

        if (skillTimer <= 0 && !isCastingSkill && spendMana())
        {
            CastSkill();
            skillTimer = skillCooldown;

            isCastingSkill = true;
            // 스킬 사용 중 캐릭터 멈추기
            rb.velocity = Vector2.zero;

            // 스킬 쿨다운 바를 가득 찬 상태로 설정 (쿨다운 시작)
            if (skillCooldownBar != null)
            {
                skillCooldownBar.fillAmount = 1;
            }
        }
    }

    private void CastSkill()
    {
        animator.SetTrigger("CastSkill");
        // 방향 계산에 flipX 값을 사용하여 방향을 결정
        Vector2 direction = spriteRenderer.flipX ? Vector2.right : Vector2.left;
        RaycastHit2D hitEnemy = Physics2D.Raycast(firePoint.position, direction, skillRange, enemyLayers);

        if (hitEnemy.collider != null)
        {
            // 이펙트 활성화
            effectAnimatorObject.SetActive(true);

            hitEnemy.collider.GetComponent<SlimeMovement>().TakeDamage(PlayerItemContainer.Instance.skillDamage);
            Debug.Log("Hit " + hitEnemy.collider.name);

            // 이미 있는 이펙트 오브젝트의 위치를 조정
            effectAnimatorObject.transform.position = hitEnemy.collider.transform.position;

        }
        else
        {
            // 피격 대상이 없을 경우 이펙트 비활성화
            effectAnimatorObject.SetActive(false);
        }
    }

    public void FlipDirection()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;  // 스프라이트의 flipX 반전
        // 또한 firePoint의 로컬 스케일도 반전시켜서 방향을 맞춤
        firePoint.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && spriteRenderer != null)
        {
            Gizmos.color = Color.red;
            Vector2 position2D = new Vector2(firePoint.position.x, firePoint.position.y); // Vector3에서 Vector2로 변환
            Vector2 direction = spriteRenderer.flipX ? Vector2.right : Vector2.left; // flipX 값에 따른 방향 설정
            Vector2 targetPosition = position2D + direction * skillRange; // 방향과 범위를 고려한 목표 위치 계산
            Gizmos.DrawLine(position2D, targetPosition); // Gizmos로 방향을 그림
        }
    }

    public void OnSkillEffect()
    {
        effectAnimator.SetBool("EffectTrigger", true);
        effectAnimatorObject.GetComponent<Animator>().SetBool("EffectObjectTrigger", true);
    }

    public void EndSkillEffect()
    {
        effectAnimator.SetBool("EffectTrigger", false);
        effectAnimatorObject.GetComponent<Animator>().SetBool("EffectObjectTrigger", false);

        // 이펙트 활성화
        effectAnimatorObject.SetActive(false);

        // 스킬 애니메이션 끝나면 캐릭터 움직임 및 회전 가능하게 복구
        //rb.constraints = RigidbodyConstraints2D.None;
        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        isCastingSkill = false; // 스킬 사용 플래그 해제
        //skillCooldownBar.fillAmount = 1; // 스킬 쿨다운 바를 가득 찬 상태로 설정
        skillTimer = skillCooldown; // 스킬 타이머 리셋
    }

    private bool spendMana()
    {
        int manaCost = 20; // 스킬 사용에 필요한 마나 비용

        if (PlayerItemContainer.Instance.currentMP >= manaCost)
        {
            PlayerItemContainer.Instance.currentMP -= manaCost;
            return true; // 충분한 마나가 있어 스킬 사용이 가능
        }
        else
        {
            Debug.Log("Not enough mana to cast the skill.");
            return false; // 마나가 부족하여 스킬 사용 불가
        }
    }
}
