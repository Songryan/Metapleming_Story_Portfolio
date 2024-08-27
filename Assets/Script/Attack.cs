using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public InputActionAsset InputActionAsset; // InputActionAsset�� Inspector���� �Ҵ�
    private InputAction attackAction;
    private InputAction skillAction;

    private Animator animator; // ���� �ִϸ��̼��� ������ Animator ������Ʈ

    public GameObject PlayerObject;

    public PolygonCollider2D attackCollider; // ���� ���� �ݶ��̴�

    public float offsetDistance = 1.0f; // A�� B ������ �Ÿ�
    private bool isFacingRight = true; // A�� �������� ���� �ִ��� ����

    // ���� ������ ���θ� ��Ÿ���� ����
    public bool isAttacking = false;

    public int AttackDamage;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;

        attackCollider = GetComponent<PolygonCollider2D>();    

        var InputActionMap = InputActionAsset.FindActionMap("Player", true);
        attackAction = InputActionMap.FindAction("Attack", true);

        attackAction.Enable();
        attackAction.performed += AttackAction;
    }

    void Start()
    {
        // ���� ���� �ݶ��̴��� ��Ȱ��ȭ�մϴ�.
        attackCollider.enabled = false;

        animator = PlayerObject.GetComponent<Animator>();
    }

    private void Update()
    {
        // A�� ���⿡ ���� B�� ��ġ�� ������Ʈ�մϴ�.
        if (isFacingRight)
        {
            // A�� �������� �� ��, B�� A�� �����ʿ� ��ġ��ŵ�ϴ�.
            transform.position = new Vector2(PlayerObject.transform.position.x - offsetDistance, PlayerObject.transform.position.y);
        }
        else
        {
            // A�� ������ �� ��, B�� A�� ���ʿ� ��ġ��ŵ�ϴ�.
            transform.position = new Vector2(PlayerObject.transform.position.x + offsetDistance, PlayerObject.transform.position.y);
        }
    }

    public void Flip()
    {
        // A�� ������ �ٲߴϴ�.
        isFacingRight = !isFacingRight;
        // �ʿ��ϴٸ� A�� ��������Ʈ ���� ó���� ���⼭ �����մϴ�.
        GetComponent<SpriteRenderer>().flipX = !isFacingRight;
    }

    public void AttackAction(InputAction.CallbackContext context)
    {
        if (!isAttacking)  // ���� ���� �ƴ� ���� ������ ���
        {
            animator.SetTrigger("AttackTrigger");
            isAttacking = true;
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Debug.Log("Valid hit detected on Monster by Attack!");
            collision.GetComponent<SlimeMovement>().TakeDamage(PlayerItemContainer.Instance.AttackDamage); // ���� �ǰ� ó��
        }
    }

}
