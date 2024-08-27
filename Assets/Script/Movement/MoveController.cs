using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{
    [Header("Movement Speed Setting")]
    public float moveSpeed = 3f; // 플레이어의 이동 속도
    public float climbSpeed = 3f;   // 사다리를 오르는 속도
    public float jumpForce = 7f;    // 점프 힘
    public float addForce = 1f;
    [Header("Layer Masks")]
    private LayerMask currentGroundLayer; // 현재 밟고 있는 레이어
    public LayerMask midGroundLayer; // MidGround 레이어 마스크
    public LayerMask foreGroundLayer; // ForeGround 레이어 마스크
    private string currentGroundTag; // 현재 밟고 있는 레이어의 태그 저장
    [Header("Movement Object Setting")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // 반전을 위한 오브젝트
    private Animator animator; // Animator 컴포넌트에 대한 참조
    [Header("Movement Checking Setting")]
    public float groundCheckRadius = 0.74f;  // 땅 감지 범위
    private bool isGrounded;    // 캐릭터가 바닥에 있는지 여부

    [Header("Attack Setting")]
    public GameObject Attack;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;    // 중력의 영향을 받지 않도록 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    private void FixedUpdate() {
        ChceckGrounded();
        //DetectWallInFront();
    }

    private void ChceckGrounded()
    {
        Vector2 position = transform.position;

        // Raycast 방향
        Vector2 downDirection = Vector2.down;

        // 레이캐스트 범위를 보여줍니다.
        Debug.DrawRay(position, downDirection * groundCheckRadius, Color.red);

        // 바로 아래 방향의 레이캐스트
        RaycastHit2D hitDown = Physics2D.Raycast(position, downDirection, groundCheckRadius, foreGroundLayer | midGroundLayer);
        
        // 땅에 닿아 있다면 isGrounded를 true로 설정합니다.
        if (hitDown.collider != null)
        {
            isGrounded = true;
            // 땅에 닿아있을떈 0
            rb.gravityScale = 0;

            //rb.AddForce(new Vector2(0, addForce), ForceMode2D.Impulse);

            currentGroundTag = hitDown.collider.tag; // 현재 밟고 있는 레이어의 태그 저장
            currentGroundLayer = 1 << hitDown.collider.gameObject.layer; // 감지된 객체의 레이어를 LayerMask 형태로 변환
        }
        else
        {
            isGrounded = false;
            // 땅에 안닿아있을떈 1
            rb.gravityScale = 1f;
            currentGroundTag = "";
        }

        // 이동 여부에 따른 애니메이터 파라미터 설정
        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        //Debug.Log($"Speed : {Mathf.Abs(rb.velocity.x)}");
        //Debug.Log($"isGrounded : {isGrounded}");
        // 점프 여부에 따른 애니메이터 파라미터 설정
        animator.SetBool("isGrounded", isGrounded);
    }

    //public void OnMove(InputValue inputValue)
    //{
    //    if (!Attack.GetComponent<Attack>().isAttacking)
    //    {
    //        Vector2 moveInput = inputValue.Get<Vector2>();
    //
    //        rb.velocity = new Vector2(moveSpeed * moveInput.x, rb.velocity.y);
    //
    //
    //        // 캐릭터가 왼쪽으로 이동하는 경우 이미지 반전을 해제합니다.
    //        if (moveInput.x < 0 && spriteRenderer.flipX == true)
    //        {
    //            spriteRenderer.flipX = false;
    //            Attack.GetComponent<Attack>().Flip();
    //        }
    //        // 캐릭터가 오른쪽으로 이동하는 경우 이미지를 반전시킵니다.
    //        else if (moveInput.x > 0 && spriteRenderer.flipX == false)
    //        {
    //            spriteRenderer.flipX = true;
    //            Attack.GetComponent<Attack>().Flip();
    //        }
    //    }
    //}

    public void OnMove(InputValue inputValue)
    {

        // 공격 중이 아닐 때만 실제 이동을 처리합니다.
        if (Attack.GetComponent<Attack>().isAttacking && GetComponent<Skill>().isCastingSkill)
        {
            return;
        }

        Vector2 moveInput = inputValue.Get<Vector2>();
        rb.velocity = new Vector2(moveSpeed * moveInput.x, rb.velocity.y);

        // 공격 중일 때도 방향 전환은 가능하게 합니다.
        if (moveInput.x < 0 && spriteRenderer.flipX != false)
        {
            spriteRenderer.flipX = false;
            if (Attack.GetComponent<Attack>() != null)
            {
                Attack.GetComponent<Attack>().Flip(); // Attack 스크립트의 Flip 메서드 호출
            }
        }
        else if (moveInput.x > 0 && spriteRenderer.flipX != true)
        {
            spriteRenderer.flipX = true;
            if (Attack.GetComponent<Attack>() != null)
            {
                Attack.GetComponent<Attack>().Flip(); // Attack 스크립트의 Flip 메서드 호출
            }
        }
    }


    public void OnJump(InputValue inputValue)
    {
        if (Attack.GetComponent<Attack>().isAttacking && GetComponent<Skill>().isCastingSkill)
        {
            return;
        }

        if (!isGrounded) return; // 땅에 닿지 않았다면 점프하지 않습니다.

        //Debug.Log(isGrounded);
        // 입력된 2D 벡터 값을 읽어옵니다.
        Vector2 moveInput = inputValue.Get<Vector2>();

        // Rigidbody2D의 속도를 설정하여 점프을 구현합니다.
        // 상하 이동이 있을 경우 rb.velocity.y를 유지합니다.
        rb.velocity = new Vector2(rb.velocity.x, moveInput.y * jumpForce);

        // 캐릭터가 왼쪽으로 이동하는 경우 이미지 반전을 해제합니다.
        if (rb.velocity.x < 0 && spriteRenderer.flipX == false)
        {
            spriteRenderer.flipX = false;
            Attack.GetComponent<Attack>().Flip();
        }
        //캐릭터가 오른쪽으로 이동하는 경우 이미지를 반전시킵니다.
        else if (rb.velocity.x > 0 && spriteRenderer.flipX == false)
        {
            spriteRenderer.flipX = true;
            Attack.GetComponent<Attack>().Flip();
        }
    }

    void DetectWallInFront()
    {
        Vector2 direction = spriteRenderer.flipX ? Vector2.right : Vector2.left;
        float distance = 0f;

        Vector2 boxSize = new Vector2(1f, 1f);
        // BoxCast의 시작점을 설정
        Vector2 boxOrigin = transform.position + new Vector3(0, boxSize.y / 2, 0); // BoxCast의 시작점, 플레이어 위치를 기준으로 조정

        // BoxCast를 실행하여 앞에 있는 오브젝트 검출
        RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, boxSize, 0, direction, distance, midGroundLayer);

        //Debug.Log($"currentGroundTag : {currentGroundTag}");

        if (hit.collider != null)
        {
            //Debug.Log($"True/False : {currentGroundLayer != 1 << hit.collider.gameObject.layer}");
            //Debug.Log($"True/False : {currentGroundLayer == midGroundLayer}");
            if (currentGroundLayer != (1 << hit.collider.gameObject.layer))
            {
                Physics2D.IgnoreLayerCollision(gameObject.layer, hit.collider.gameObject.layer, true);
            }
            // MidGround에 올라와 있을 때는 충돌을 다시 감지
            else if (currentGroundLayer == midGroundLayer)
            {
                Physics2D.IgnoreLayerCollision(gameObject.layer, hit.collider.gameObject.layer, false);
            }
        }

    }

    // Animation Event로 호출될 메서드
    public void OnAttackHit()
    {
        rb.velocity = Vector2.zero;
        Attack.GetComponent<SpriteRenderer>().enabled = true;
        Attack.GetComponent<PolygonCollider2D>().enabled = true; // 공격 범위 콜라이더를 활성화합니다.
    }

    public void OnAttackEnd()
    {
        Attack.GetComponent<PolygonCollider2D>().enabled = false; // 공격 범위 콜라이더를 비활성화합니다.
        Attack.GetComponent<SpriteRenderer>().enabled = false;
        Attack.GetComponent<Attack>().isAttacking = false;
    }
}
