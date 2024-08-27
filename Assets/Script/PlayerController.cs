using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Speed Setting")]
    public float moveSpeed = 3f; // 플레이어의 이동 속도
    public float climbSpeed = 30f;   // 사다리를 오르는 속도
    public float jumpForce = 7f;    // 점프 힘
    [Header("Layer Masks")]
    private LayerMask currentGroundLayer; // 현재 밟고 있는 레이어
    public LayerMask foreGroundLayer; // ForeGround 레이어 마스크
    public LayerMask midGroundLayer; // MidGround 레이어 마스크
    public LayerMask backGroundLayer; // BackGround 레이어 마스크
    public LayerMask ladderLayer;   // 사다리를 감지할 레이어 마스크
    [Header("Movement Object Setting")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // 반전을 위한 오브젝트
    private Animator animator; // Animator 컴포넌트에 대한 참조
    [Header("Movement Checking Setting")]
    public float groundCheckRadius = 1.5f;  // 땅 감지 범위
    public float stairCheckRadius = 0.2f;  // 계단 감지 범위
    private bool isGrounded;    // 캐릭터가 바닥에 있는지 여부
    private bool isOnSlope;    // 캐릭터가 경사면에 있는지 여부
    private bool isClimbing;    // 캐릭터가 밧줄에있는지 여부
    private bool isOnSlopeEnd;   // 경사면 끝 감지
    private float slopeDownAngle; // 경사면의 아래 방향 각도
    private float slopeDownAngleOld; // 이전 프레임에서의 경사면 각도
    private Vector2 slopeNormalPerp; // 경사면의 법선에 수직인 벡터
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;    // 중력의 영향을 받지 않도록 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        isClimbing = false;
    }

    void Update()
    {
        ChceckGrounded();
    }

    void FixedUpdate()
    {
        DetectWallInFront();
    }

    void DetectWallInFront()
    {
        Vector2 direction = spriteRenderer.flipX ? Vector2.right : Vector2.left;
        Vector2 boxSize = new Vector2(1.0f, 1.0f); // BoxCast에 사용될 박스의 크기
        float distance = 1.0f;

        // BoxCast의 시작점을 설정
        //Vector2 boxOrigin = (Vector2)transform.position + (Vector2.up * (boxSize.y / 2)) + (direction * (boxSize.x / 2));
        Vector2 boxOrigin = (Vector2)transform.position + (Vector2.up * (boxSize.y / 2)) + (direction * (boxSize.x / 2));
        
        // 시각화
        Debug.DrawRay(boxOrigin, direction * distance, Color.red);
        Debug.DrawRay(boxOrigin + new Vector2(0f, boxSize.y), direction * distance, Color.red);
        Debug.DrawRay(boxOrigin, new Vector2(0f, boxSize.y), Color.red);
        Debug.DrawRay(boxOrigin + (Vector2)(direction * distance), new Vector2(0f, boxSize.y), Color.red);

        RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, boxSize, 0, direction, distance, foreGroundLayer | midGroundLayer | backGroundLayer | ladderLayer);

        if (hit.collider != null)
        {
            // 현재 밟고 있는 레이어와 감지된 벽의 레이어가 다른 경우 충돌 무시
            if (currentGroundLayer != (1 << hit.collider.gameObject.layer))
            {
                Physics2D.IgnoreLayerCollision(gameObject.layer, hit.collider.gameObject.layer, true);
            }
            else if(!isGrounded)
            {
                // 같은 레이어의 벽과는 충돌을 유지
                Physics2D.IgnoreLayerCollision(gameObject.layer, hit.collider.gameObject.layer, false);
            }
        }
    }

    // 땅에 닿아 있는지 여부를 확인합니다. (땅에 닿아 있을 떄만 점프할 수 있도록)
    private void ChceckGrounded()
    {
        Vector2 position = transform.position;

        // Raycast 방향 (계단을 위한 좌우 아래 대각선)
        Vector2 downDirection = Vector2.down;

        // 레이캐스트 범위를 보여줍니다.
        Debug.DrawRay(position, downDirection * groundCheckRadius, Color.red);

        // 바로 아래 방향의 레이캐스트
        RaycastHit2D hitDown = Physics2D.Raycast(position, downDirection, groundCheckRadius, foreGroundLayer | midGroundLayer | backGroundLayer | ladderLayer);

        // 땅에 닿아 있다면 isGrounded를 true로 설정합니다.
        if (hitDown.collider != null)
        {
            isGrounded = true;
            // 땅에 닿아있을떈 0
            rb.gravityScale = 0;
            // Raycast에 의해 감지된 레이어를 currentGroundLayer 변수에 저장
            currentGroundLayer = 1 << hitDown.collider.gameObject.layer; // 감지된 객체의 레이어를 LayerMask 형태로 변환
        }
        else
        {
            isGrounded = false;
            // 땅에 안닿아있을떈 1
            rb.gravityScale = 1f;
        }

        if (isGrounded)
        {
            //CheckSlope(); // 경사면 확인
        }

        // 이동 여부에 따른 애니메이터 파라미터 설정
        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        //Debug.Log($"Speed : {Mathf.Abs(rb.velocity.x)}");
        // 점프 여부에 따른 애니메이터 파라미터 설정
        animator.SetBool("isGrounded", isGrounded);
    }

    // 플레이어 좌우 이동 로직 구현
    public void OnMove(InputValue inputValue)
    {
        //Debug.Log(isGrounded);
        // 입력된 2D 벡터 값을 읽어옵니다.
        Vector2 moveInput = inputValue.Get<Vector2>();

        // 경사면에서 이동 로직
        if (isGrounded)
        {
            if (isOnSlope && !isOnSlopeEnd && slopeDownAngle != 0f && slopeDownAngle <= 75f)
            {
                // 경사면에 따라 조정된 속도 벡터 계산. 이는 경사면을 따라 자연스럽게 이동하기 위함입니다.
                Vector2 adjustedMove = Vector2.zero;

                if (moveInput.x != 0) // 좌우 이동 입력이 있는 경우에만 적용
                {
                    // 경사면을 따라 이동하기 위해 slopeNormalPerp 벡터와 이동 방향을 고려하여 속도 벡터 조정
                    adjustedMove = new Vector2(slopeNormalPerp.x * moveSpeed * Mathf.Sign(moveInput.x), slopeNormalPerp.y * moveSpeed);
                    rb.velocity = adjustedMove;
                }
            }
            else if (isOnSlope && isOnSlopeEnd)
            {
                // 경사면 끝에서 평지로 이동할 때의 속도 처리
                if (moveInput.x != 0) // 좌우 이동 입력이 있는 경우에만 적용
                {
                    rb.velocity = new Vector2(moveSpeed * moveInput.x, Mathf.Clamp(rb.velocity.y, -Mathf.Infinity, 0));
                }
            }
            else
            {
                // 평지에서의 이동
                rb.velocity = new Vector2(moveSpeed * moveInput.x, rb.velocity.y);
            }
        }

        // 캐릭터가 왼쪽으로 이동하는 경우 이미지 반전을 해제합니다.
        if (moveInput.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        // 캐릭터가 오른쪽으로 이동하는 경우 이미지를 반전시킵니다.
        else if (moveInput.x > 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void OnJump(InputValue inputValue)
    {
        //Debug.Log(isGrounded);
        // 입력된 2D 벡터 값을 읽어옵니다.
        Vector2 moveInput = inputValue.Get<Vector2>();

        // Rigidbody2D의 속도를 설정하여 점프을 구현합니다.
        // 상하 이동이 있을 경우 rb.velocity.y를 유지합니다.
        rb.velocity = new Vector2(rb.velocity.x, moveInput.y * jumpForce);

        // 캐릭터가 왼쪽으로 이동하는 경우 이미지 반전을 해제합니다.
        if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        //캐릭터가 오른쪽으로 이동하는 경우 이미지를 반전시킵니다.
        else if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void OnClimb(InputValue inputValue)
    {
        //isClimbing = Physics2D.OverlapCircle(transform.position, groundCheckRadius, ladderLayer);

        Collider2D ladderCollider = Physics2D.OverlapCircle(transform.position, groundCheckRadius, ladderLayer);
        
        // 사다리가 감지되었을 경우
        if (ladderCollider != null && inputValue.Get<Vector2>().y != 0)
        {
            // 상하 입력값을 읽어옵니다.
            float vertical = inputValue.Get<Vector2>().y;
            
            // 중력의 영향을 제거하고, 애니메이션 상태를 업데이트합니다.
            rb.gravityScale = 0;
            animator.SetBool("isClimbing", true);

            // 캐릭터의 y축 위치를 조정합니다.
            Vector2 position = rb.position;
            position.y += vertical * climbSpeed * Time.deltaTime;
            rb.position = position;

            Debug.Log(position.y);
        }
        else
        {
            // 사다리에서 내려왔을 경우, 중력을 다시 적용합니다.
            rb.gravityScale = 1;
            animator.SetBool("isClimbing", false);
        }
    }

    public void OnDropThrough(InputAction.CallbackContext context)
    {
        // Ctrl + 아래 방향키를 눌렀을 때 플랫폼을 빠르게 하강
        if(context.performed && isGrounded)
        {
            // 현재 플레이어가 서있는 플랫폼의 PlatformEffector2D 컴포넌트를 비활성화하여 통과
            // 플레이어의 collider를 일시적으로 비활성화
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // 사다리 태그나 로프태그를 갖는 오브젝트와 충돌했을 때 처리

    }

    private void CheckSlope()
    {
        // 캐릭터 하단 중심점을 기준으로 아래로 레이캐스트하여 경사면 확인
        Vector2 checkPos = transform.position - new Vector3(0.0f, GetComponent<Collider2D>().bounds.extents.y);
        SlopeCheckVertical(checkPos);

        // 슬로프 끝에 도달했을 때의 추가 처리
        if (isOnSlopeEnd)
        {
            // 경사면 끝에 도달했을 때, 캐릭터가 살짝 뜨지 않도록 y축 위치를 조금 조정합니다.
            // 이는 현재 캐릭터의 속도, 슬로프 각도, 그리고 캐릭터의 현재 위치를 기반으로 계산할 수 있습니다.
            // 여기서는 간단한 예시로, y축 속도를 조금 줄이는 방법을 사용합니다.
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x * 2f , rb.velocity.y * 0.3f);
            }

            // 슬로프 끝에서 평지로 이동할 때의 속도 조정
            // 슬로프를 내려올 때의 속도를 유지하면서 평지로의 전환을 보다 부드럽게 합니다.
            rb.velocity = new Vector2(moveSpeed * Mathf.Sign(rb.velocity.x), rb.velocity.y);

            // 중력 조정: 슬로프 끝에서의 중력 효과를 조금 증가시킵니다.
            rb.gravityScale = 0f;

            // 몇 프레임 후 중력 스케일을 원래대로 되돌립니다.
            StartCoroutine(ResetGravityScale());
        }
    }

    IEnumerator ResetGravityScale()
    {
        // 잠시 기다린 후 중력 스케일을 초기 설정값으로 되돌립니다.
        yield return new WaitForSeconds(0.1f);
        rb.gravityScale = 0f;
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        // 아래 방향으로 레이캐스트하여 바닥 감지
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, groundCheckRadius, currentGroundLayer);

        // 레이캐스트 경로를 Scene 뷰에 그립니다 (빨간색으로 표시)
        Debug.DrawRay(checkPos, Vector2.down * groundCheckRadius, Color.red);

        if (hit) // 레이캐스트에 충돌한 경우
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized; // 충돌 지점의 법선에 수직인 벡터 계산
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up); // 경사면 각도 계산
            
            Debug.DrawRay(hit.point, hit.normal, Color.blue); // 법선을 파란색으로 표시
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.green); // 법선에 수직인 벡터를 녹색으로 표시

            if (slopeDownAngle != slopeDownAngleOld) // 경사면 각도가 변한 경우
            {
                isOnSlope = true;
            }

            // 경사면 끝 감지
            isOnSlopeEnd = Mathf.Approximately(hit.normal.x, 0f) && Mathf.Approximately(hit.normal.y, 1f);

            slopeDownAngleOld = slopeDownAngle; // 이전 경사면 각도 갱신
        }
        else
        {
            isOnSlope = false;
            isOnSlopeEnd = false;
        }
    }
}
