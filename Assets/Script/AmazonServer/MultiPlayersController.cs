using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiPlayersController : MonoBehaviour
{
    [Header("MultiPlayer State Setting")]
    private int HP;
    private int MP;
    private string UserId;

    [Header("Layer Masks")]
    private LayerMask currentGroundLayer; // 현재 밟고 있는 레이어
    public LayerMask midGroundLayer; // MidGround 레이어 마스크
    public LayerMask foreGroundLayer; // ForeGround 레이어 마스크
    private string currentGroundTag; // 현재 밟고 있는 레이어의 태그 저장

    [Header("Movement Object Setting")]
    private Vector2 lastPosition;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // 반전을 위한 오브젝트
    private Animator animator; // Animator 컴포넌트에 대한 참조

    [Header("Movement Checking Setting")]
    public float groundCheckRadius = 0.8f;  // 땅 감지 범위
    private bool isGrounded;    // 캐릭터가 바닥에 있는지 여부

    [Header("Nickname Setting")]
    public TextMeshProUGUI Nickname; // 인스펙터에서 할당하거나 Start() 또는 Awake()에서 찾습니다.

    public TextMeshProUGUI chatText;
    public GameObject Wordballoon;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;    // 중력의 영향을 받지 않도록 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Nickname = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        ChceckGrounded();
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
        //animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        //Debug.Log($"Speed : {Mathf.Abs(rb.velocity.x)}");
        //Debug.Log($"isGrounded : {isGrounded}");
        // 점프 여부에 따른 애니메이터 파라미터 설정
        animator.SetBool("isGrounded", isGrounded);
    }

    // 플레이어 데이터 업데이트
    public void UpdatePlayer(AKMessage data)
    {
        UserId = data.UserId;
        HP = data.HP;
        MP = data.MP;
        //AniState = data.AniState;

        Nickname.text = data.UserNickname;

        // 코루틴 중지
        StopCoroutine("MoveToPosition");
        //Debug.Log("UpdatePlayer 실행!");
        // 애니메이션 상태 업데이트 등
        // 움직임 시작
        StartCoroutine(MoveToPosition(data.Position));
    }

    /*IEnumerator MoveToPosition(GameObject player, Vector2 newPosition)
    {
        Vector2 startPosition = player.transform.position;
        float timeToMove = 0.1f; // 서버에서 새 위치를 보내는 데 걸리는 예상 시간
        float elapsedTime = 0;

        //Debug.Log($"While 문 {elapsedTime < timeToMove}");
        while (elapsedTime < timeToMove)
        {
            //Debug.Log("보간 코루틴 시작");
            player.transform.position = Vector2.Lerp(startPosition, newPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.position = newPosition; // 최종 위치를 보장합니다.
    }*/
    IEnumerator MoveToPosition(Vector2 newPosition)
    {
        Vector2 startPosition = rb.position;
        float timeToMove = 0.7f; // 원하는 이동 완료 시간
        float elapsedTime = 0;

        while (elapsedTime < timeToMove)
        {
            elapsedTime += Time.deltaTime;
            float blend = elapsedTime / timeToMove;
            Vector2 nextPosition = Vector2.Lerp(startPosition, newPosition, blend);
            rb.MovePosition(nextPosition);

            // 속도 계산 및 애니메이션 파라미터 갱신
            float speed = (nextPosition - startPosition).magnitude / Time.deltaTime;
            animator.SetFloat("speed", speed);

            // 반전 애니메이션
            if (newPosition.x > startPosition.x)
            {
                spriteRenderer.flipX = true;
            }
            else if (newPosition.x < startPosition.x)
            {
                spriteRenderer.flipX = false;
            }


            yield return null;
        }

        rb.position = newPosition; // 최종 위치를 보장합니다.
    }

    public void chating(string content)
    {
        DisplayChatMessage(content);
    }


    // 말풍선을 표시하고 일정 시간 후 비활성화하는 코루틴
    private IEnumerator DisplayWordBalloonCoroutine(string text, float delay)
    {
        Wordballoon.SetActive(true); // 말풍선 활성화
        chatText.text = text;
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        Wordballoon.SetActive(false); // 말풍선 비활성화
    }

    private void DisplayChatMessage(string message)
    {
        // 이전에 실행중인 말풍선 코루틴이 있다면 멈춤
        StopCoroutine("DisplayWordBalloonCoroutine");
        // 새로운 말풍선 코루틴 시작
        StartCoroutine(DisplayWordBalloonCoroutine(message, 3.0f));
    }


}