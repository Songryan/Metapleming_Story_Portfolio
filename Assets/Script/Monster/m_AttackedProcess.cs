using UnityEngine;

public class m_AttackedProcess : MonoBehaviour
{
    public int damage = 10;

    // Collider�� ����� �÷��̾���� �浹 ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            p_AttackedProcess player = collision.gameObject.GetComponent<p_AttackedProcess>();
            if (player != null)
            {
                // �ǰ� ������ ���� (���⼭�� ������ ��ġ�� �����մϴ�.)
                player.TakeDamage(damage, transform.position);
            }
        }
    }
}
