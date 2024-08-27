using UnityEngine;

public class m_AttackedProcess : MonoBehaviour
{
    public int damage = 10;

    // Collider를 사용해 플레이어와의 충돌 감지
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            p_AttackedProcess player = collision.gameObject.GetComponent<p_AttackedProcess>();
            if (player != null)
            {
                // 피격 방향을 전달 (여기서는 몬스터의 위치를 전달합니다.)
                player.TakeDamage(damage, transform.position);
            }
        }
    }
}
