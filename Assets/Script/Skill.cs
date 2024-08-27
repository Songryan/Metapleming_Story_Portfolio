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
    private SpriteRenderer spriteRenderer; // SpriteRenderer ������Ʈ ����

    public Animator effectAnimator;
    public GameObject effectAnimatorObject;

    private Rigidbody2D rb; // Rigidbody2D ������Ʈ ����
    public bool isCastingSkill = false; // ��ų ��� ������ Ȯ���ϴ� �÷���

    public Image skillCooldownBar; // ��ų ��ٿ��� ǥ���� UI ��

    private void Awake()
    {
        var InputActionMap = InputActionAsset.FindActionMap("Player", true);
        skillAction = InputActionMap.FindAction("Skill", true);
        skillAction.Enable();
        skillAction.performed += SkillAction;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();  // SpriteRenderer ������Ʈ ��������
        //effectAnimator.SetBool("EffectTrigger", true);
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ������Ʈ ��������
    }

    void Update()
    {
        // ��ų ��� ���� �ƴ� ���� skillTimer ����
        if (!isCastingSkill)
        {
            if (skillTimer > 0)
            {
                skillTimer -= Time.deltaTime;
                // ��ų ��ٿ� �� ������Ʈ
                if (skillCooldownBar != null)
                {
                    skillCooldownBar.fillAmount = skillTimer / skillCooldown;
                }
            }
            else
            {
                // ��ٿ��� ������ �� ��ų�� �ٽ� ����� �� �ֵ��� ����
                isCastingSkill = false;
                skillCooldownBar.fillAmount = 0; // ��ٿ� �� ���
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

    }

    public void SkillAction(InputAction.CallbackContext context)
    {
        //��ų ���� �Ҹ�� 0�϶� ������ ����
        

        if (skillTimer <= 0 && !isCastingSkill && spendMana())
        {
            CastSkill();
            skillTimer = skillCooldown;

            isCastingSkill = true;
            // ��ų ��� �� ĳ���� ���߱�
            rb.velocity = Vector2.zero;

            // ��ų ��ٿ� �ٸ� ���� �� ���·� ���� (��ٿ� ����)
            if (skillCooldownBar != null)
            {
                skillCooldownBar.fillAmount = 1;
            }
        }
    }

    private void CastSkill()
    {
        animator.SetTrigger("CastSkill");
        // ���� ��꿡 flipX ���� ����Ͽ� ������ ����
        Vector2 direction = spriteRenderer.flipX ? Vector2.right : Vector2.left;
        RaycastHit2D hitEnemy = Physics2D.Raycast(firePoint.position, direction, skillRange, enemyLayers);

        if (hitEnemy.collider != null)
        {
            // ����Ʈ Ȱ��ȭ
            effectAnimatorObject.SetActive(true);

            hitEnemy.collider.GetComponent<SlimeMovement>().TakeDamage(PlayerItemContainer.Instance.skillDamage);
            Debug.Log("Hit " + hitEnemy.collider.name);

            // �̹� �ִ� ����Ʈ ������Ʈ�� ��ġ�� ����
            effectAnimatorObject.transform.position = hitEnemy.collider.transform.position;

        }
        else
        {
            // �ǰ� ����� ���� ��� ����Ʈ ��Ȱ��ȭ
            effectAnimatorObject.SetActive(false);
        }
    }

    public void FlipDirection()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;  // ��������Ʈ�� flipX ����
        // ���� firePoint�� ���� �����ϵ� �������Ѽ� ������ ����
        firePoint.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && spriteRenderer != null)
        {
            Gizmos.color = Color.red;
            Vector2 position2D = new Vector2(firePoint.position.x, firePoint.position.y); // Vector3���� Vector2�� ��ȯ
            Vector2 direction = spriteRenderer.flipX ? Vector2.right : Vector2.left; // flipX ���� ���� ���� ����
            Vector2 targetPosition = position2D + direction * skillRange; // ����� ������ ����� ��ǥ ��ġ ���
            Gizmos.DrawLine(position2D, targetPosition); // Gizmos�� ������ �׸�
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

        // ����Ʈ Ȱ��ȭ
        effectAnimatorObject.SetActive(false);

        // ��ų �ִϸ��̼� ������ ĳ���� ������ �� ȸ�� �����ϰ� ����
        //rb.constraints = RigidbodyConstraints2D.None;
        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        isCastingSkill = false; // ��ų ��� �÷��� ����
        //skillCooldownBar.fillAmount = 1; // ��ų ��ٿ� �ٸ� ���� �� ���·� ����
        skillTimer = skillCooldown; // ��ų Ÿ�̸� ����
    }

    private bool spendMana()
    {
        int manaCost = 20; // ��ų ��뿡 �ʿ��� ���� ���

        if (PlayerItemContainer.Instance.currentMP >= manaCost)
        {
            PlayerItemContainer.Instance.currentMP -= manaCost;
            return true; // ����� ������ �־� ��ų ����� ����
        }
        else
        {
            Debug.Log("Not enough mana to cast the skill.");
            return false; // ������ �����Ͽ� ��ų ��� �Ұ�
        }
    }
}
