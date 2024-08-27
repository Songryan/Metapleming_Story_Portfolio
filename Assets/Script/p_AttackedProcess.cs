using UnityEngine;
using System.Collections;

public class p_AttackedProcess : MonoBehaviour
{
    public float knockbackForce = 2.7f;  // �ݵ� ���� ũ��
    public float knockbackDuration = 0.4f;  // �ݵ� ���� �ð�
    public float invincibleDuration = 3.0f;  // ���� �ð�
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;  // �ִϸ����� ������Ʈ �߰�

    public DamageNumber damagePopupPrefab; // DamagePopup ������

    public Sprite defaultSprite; // �⺻ ��������Ʈ
    public Sprite deadSprite; // ��� �� ����� ��������Ʈ
    public float deathDuration = 5.0f; // ��� ���� ���� �ð�
    private Vector2 originalPosition;  // ĳ������ ���� ��ġ ����

    public GameObject Tomb;
    public Animator DieAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();  // �ִϸ����� ������Ʈ �Ҵ�
        originalPosition = transform.position;  // ���� ��ġ ����
    }

    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        // ������ �˾� ǥ��
        Vector3 spawnPosition = transform.position + new Vector3(0, 0.5f, 0); // ���� ��ġ�� ĳ���� ���� ����
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
        // ���̾� ������ ���� �浹 ����
        int playerLayer = gameObject.layer;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // ���� ����
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

        yield return new WaitForSeconds(duration);

        // ���� ���� �� �浹 ���� ����
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // ��� �ִϸ��̼� �� ���� ���� ó��

        // ����ġ �ٿ� ó��
        expDownProcess();

        animator.enabled = false;  // �ִϸ����� ��Ȱ��ȭ
        // ��� �� ��������Ʈ ����
        spriteRenderer.sprite = deadSprite;

        // ��� �̵� �� �ִϸ��̼� ����
        rb.velocity = Vector2.zero;
        rb.isKinematic = true; // ���� ���� ���� ����

        // 5�� �Ŀ� ���� ���·� ����
        Vector3 deathPosition = transform.position;
        StartCoroutine(DropTomb(deathPosition)); // �ڷ�ƾ ȣ��
        StartCoroutine(ResetAfterDeath());
        StartCoroutine(BecomeInvincible(deathDuration));
    }

    IEnumerator ResetAfterDeath()
    {
        DieAnimator.SetBool("TombBool", true); // �ִϸ��̼� ����
        // 5�ʰ� ���
        yield return new WaitForSeconds(deathDuration);
        DieAnimator.SetBool("TombBool", false); // �ִϸ��̼� ����
        Tomb.SetActive(false);

        // ��� ���� ���� �� �ʱ�ȭ
        spriteRenderer.sprite = defaultSprite;
        rb.isKinematic = false; // ���� ���� ���� �ٽ� ����
        //transform.position = originalPosition;
        animator.enabled = true;  // �ִϸ����� �ٽ� Ȱ��ȭ

        PlayerItemContainer.Instance.currentHP = PlayerItemContainer.Instance.HP;
        PlayerItemContainer.Instance.currentMP = PlayerItemContainer.Instance.MP;

        Debug.Log("Player Reset Complete!");
    }

    IEnumerator DropTomb(Vector3 startPosition)
    {
        float dropDuration = 1.0f; // �������� �� �ɸ��� �ð�
        float elapsedTime = 0;
        float dropHeight = 6.0f; // ���� ���� (�÷��̾� ��ġ�κ��� Y������ �󸶳� ��������)

        Vector3 endPosition = transform.position; // ���� ��ġ (�÷��̾��� ���� ��ġ)
        Vector3 currentPosition = startPosition + new Vector3(0, dropHeight, 0); // ���� ��ġ (�÷��̾� ��ġ�� Y�� ��)

        Tomb.transform.position = currentPosition; // ���� ��ġ���� Tomb ��ġ ����
        Tomb.SetActive(true);
        Tomb.transform.SetParent(null); // ���� �������� �и�

        while (elapsedTime < dropDuration)
        {
            // ���� ������ ����Ͽ� �ε巴�� ��ġ�� ����
            Tomb.transform.position = Vector3.Lerp(currentPosition, endPosition, elapsedTime / dropDuration);
            elapsedTime += Time.deltaTime; // �ð� ������Ʈ
            yield return null; // ���� �����ӱ��� ���
        }

        Tomb.transform.position = endPosition; // ���� ��ġ Ȯ��
        DieAnimator.SetBool("TombBool", true); // ������ ���� �ִϸ��̼� ����
    }

    public void expDownProcess()
    {
        int cExp = PlayerItemContainer.Instance.currentEXP;
        int Level = PlayerItemContainer.Instance.Level;

        // ������ 50�� ����
        if (cExp >= Level * 50)
            PlayerItemContainer.Instance.currentEXP -= Level * 50;
        else
            PlayerItemContainer.Instance.currentEXP = 0;
    }
}
