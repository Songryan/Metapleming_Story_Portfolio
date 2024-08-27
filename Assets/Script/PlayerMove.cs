using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private Vector2 inputMovement = Vector2.zero;

    [SerializeField]
    private Rigidbody2D playerRigid;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        // Rigidbody의 useGravity를 비활성화합니다.
        playerRigid.gravityScale = 0;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        Vector2 moveMovement = inputMovement * moveSpeed * Time.deltaTime;
        transform.Translate(moveMovement);
    }
    
    //상하좌우
    private void OnMove(InputValue inputValue)
    {
        inputMovement = inputValue.Get<Vector2>();
    }

    //점프
    public float jumpForce = 5f; // 점프 힘을 설정
    public float gravity = -9.81f;
    private bool isGrounded = true; // 땅에 닿아 있는지 확인하는 변수, 실제로는 raycast 등으로 확인해야 합니다.
    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            playerRigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }
    void FixedUpdate()
    {
        if (!isGrounded)
        {
            // 중력을 직접 적용합니다.
            playerRigid.velocity = new Vector2(playerRigid.velocity.x, playerRigid.velocity.y + gravity * Time.fixedDeltaTime);
        }
    }
    // OnCollisionEnter를 사용하여 땅에 닿았는지 확인합니다.
    void OnCollisionEnter2D(Collision2D other)
    {
        // 여기서 'Ground'는 땅을 의미하는 레이어 이름입니다. 실제 게임에 맞게 조정해야 합니다.
        if (other.gameObject.layer == LayerMask.NameToLayer("ForeGround"))
        {
            isGrounded = true;
        }
    }
}
