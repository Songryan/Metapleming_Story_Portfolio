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
    private LayerMask currentGroundLayer; // ���� ��� �ִ� ���̾�
    public LayerMask midGroundLayer; // MidGround ���̾� ����ũ
    public LayerMask foreGroundLayer; // ForeGround ���̾� ����ũ
    private string currentGroundTag; // ���� ��� �ִ� ���̾��� �±� ����

    [Header("Movement Object Setting")]
    private Vector2 lastPosition;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // ������ ���� ������Ʈ
    private Animator animator; // Animator ������Ʈ�� ���� ����

    [Header("Movement Checking Setting")]
    public float groundCheckRadius = 0.8f;  // �� ���� ����
    private bool isGrounded;    // ĳ���Ͱ� �ٴڿ� �ִ��� ����

    [Header("Nickname Setting")]
    public TextMeshProUGUI Nickname; // �ν����Ϳ��� �Ҵ��ϰų� Start() �Ǵ� Awake()���� ã���ϴ�.

    public TextMeshProUGUI chatText;
    public GameObject Wordballoon;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;    // �߷��� ������ ���� �ʵ��� ����
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

        // Raycast ����
        Vector2 downDirection = Vector2.down;

        // ����ĳ��Ʈ ������ �����ݴϴ�.
        Debug.DrawRay(position, downDirection * groundCheckRadius, Color.red);

        // �ٷ� �Ʒ� ������ ����ĳ��Ʈ
        RaycastHit2D hitDown = Physics2D.Raycast(position, downDirection, groundCheckRadius, foreGroundLayer | midGroundLayer);

        // ���� ��� �ִٸ� isGrounded�� true�� �����մϴ�.
        if (hitDown.collider != null)
        {
            isGrounded = true;
            // ���� ��������� 0
            rb.gravityScale = 0;

            //rb.AddForce(new Vector2(0, addForce), ForceMode2D.Impulse);

            currentGroundTag = hitDown.collider.tag; // ���� ��� �ִ� ���̾��� �±� ����
            currentGroundLayer = 1 << hitDown.collider.gameObject.layer; // ������ ��ü�� ���̾ LayerMask ���·� ��ȯ
        }
        else
        {
            isGrounded = false;
            // ���� �ȴ�������� 1
            rb.gravityScale = 1f;
            currentGroundTag = "";
        }

        // �̵� ���ο� ���� �ִϸ����� �Ķ���� ����
        //animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        //Debug.Log($"Speed : {Mathf.Abs(rb.velocity.x)}");
        //Debug.Log($"isGrounded : {isGrounded}");
        // ���� ���ο� ���� �ִϸ����� �Ķ���� ����
        animator.SetBool("isGrounded", isGrounded);
    }

    // �÷��̾� ������ ������Ʈ
    public void UpdatePlayer(AKMessage data)
    {
        UserId = data.UserId;
        HP = data.HP;
        MP = data.MP;
        //AniState = data.AniState;

        Nickname.text = data.UserNickname;

        // �ڷ�ƾ ����
        StopCoroutine("MoveToPosition");
        //Debug.Log("UpdatePlayer ����!");
        // �ִϸ��̼� ���� ������Ʈ ��
        // ������ ����
        StartCoroutine(MoveToPosition(data.Position));
    }

    /*IEnumerator MoveToPosition(GameObject player, Vector2 newPosition)
    {
        Vector2 startPosition = player.transform.position;
        float timeToMove = 0.1f; // �������� �� ��ġ�� ������ �� �ɸ��� ���� �ð�
        float elapsedTime = 0;

        //Debug.Log($"While �� {elapsedTime < timeToMove}");
        while (elapsedTime < timeToMove)
        {
            //Debug.Log("���� �ڷ�ƾ ����");
            player.transform.position = Vector2.Lerp(startPosition, newPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.position = newPosition; // ���� ��ġ�� �����մϴ�.
    }*/
    IEnumerator MoveToPosition(Vector2 newPosition)
    {
        Vector2 startPosition = rb.position;
        float timeToMove = 0.7f; // ���ϴ� �̵� �Ϸ� �ð�
        float elapsedTime = 0;

        while (elapsedTime < timeToMove)
        {
            elapsedTime += Time.deltaTime;
            float blend = elapsedTime / timeToMove;
            Vector2 nextPosition = Vector2.Lerp(startPosition, newPosition, blend);
            rb.MovePosition(nextPosition);

            // �ӵ� ��� �� �ִϸ��̼� �Ķ���� ����
            float speed = (nextPosition - startPosition).magnitude / Time.deltaTime;
            animator.SetFloat("speed", speed);

            // ���� �ִϸ��̼�
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

        rb.position = newPosition; // ���� ��ġ�� �����մϴ�.
    }

    public void chating(string content)
    {
        DisplayChatMessage(content);
    }


    // ��ǳ���� ǥ���ϰ� ���� �ð� �� ��Ȱ��ȭ�ϴ� �ڷ�ƾ
    private IEnumerator DisplayWordBalloonCoroutine(string text, float delay)
    {
        Wordballoon.SetActive(true); // ��ǳ�� Ȱ��ȭ
        chatText.text = text;
        yield return new WaitForSeconds(delay); // ������ �ð���ŭ ���
        Wordballoon.SetActive(false); // ��ǳ�� ��Ȱ��ȭ
    }

    private void DisplayChatMessage(string message)
    {
        // ������ �������� ��ǳ�� �ڷ�ƾ�� �ִٸ� ����
        StopCoroutine("DisplayWordBalloonCoroutine");
        // ���ο� ��ǳ�� �ڷ�ƾ ����
        StartCoroutine(DisplayWordBalloonCoroutine(message, 3.0f));
    }


}