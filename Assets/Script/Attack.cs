using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public InputActionAsset InputActionAsset; // InputActionAsset을 Inspector에서 할당
    private InputAction attackAction;
    private InputAction skillAction;

    private Animator animator; // 공격 애니메이션을 관리할 Animator 컴포넌트

    public GameObject PlayerObject;

    public PolygonCollider2D attackCollider; // 공격 범위 콜라이더

    public float offsetDistance = 1.0f; // A와 B 사이의 거리
    private bool isFacingRight = true; // A가 오른쪽을 보고 있는지 여부

    // 공격 중인지 여부를 나타내는 변수
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
        // 공격 범위 콜라이더를 비활성화합니다.
        attackCollider.enabled = false;

        animator = PlayerObject.GetComponent<Animator>();
    }

    private void Update()
    {
        // A의 방향에 따라 B의 위치를 업데이트합니다.
        if (isFacingRight)
        {
            // A가 오른쪽을 볼 때, B를 A의 오른쪽에 위치시킵니다.
            transform.position = new Vector2(PlayerObject.transform.position.x - offsetDistance, PlayerObject.transform.position.y);
        }
        else
        {
            // A가 왼쪽을 볼 때, B를 A의 왼쪽에 위치시킵니다.
            transform.position = new Vector2(PlayerObject.transform.position.x + offsetDistance, PlayerObject.transform.position.y);
        }
    }

    public void Flip()
    {
        // A의 방향을 바꿉니다.
        isFacingRight = !isFacingRight;
        // 필요하다면 A의 스프라이트 반전 처리도 여기서 수행합니다.
        GetComponent<SpriteRenderer>().flipX = !isFacingRight;
    }

    public void AttackAction(InputAction.CallbackContext context)
    {
        if (!isAttacking)  // 공격 중이 아닐 때만 공격을 허용
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
            collision.GetComponent<SlimeMovement>().TakeDamage(PlayerItemContainer.Instance.AttackDamage); // 몬스터 피격 처리
        }
    }

}
