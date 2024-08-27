using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public Sprite[] numberSprites; // 0-9������ ���� ��������Ʈ �迭
    public GameObject numberPrefab; // �ϳ��� ���ڸ� ǥ���ϴ� ������

    public float moveSpeed = 1f; // ���ڰ� �ö󰡴� �ӵ�
    public float fadeSpeed = 1f; // ���̵� �ƿ� �Ǵ� �ӵ�
    public float duration = 1f; // ǥ�õǴ� �ð�

    private float timer; // ������� ����� �ð��� ����

    private void Start()
    {
        // Ÿ�̸Ӹ� ���� �ð����� �ʱ�ȭ
        timer = duration;
    }

    private void Update()
    {
        // ���� �̵�
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // ���� ����
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = Mathf.Clamp01(color.a - (fadeSpeed * Time.deltaTime));
        spriteRenderer.color = color;

        // Ÿ�̸� ������Ʈ �� �ð��� ������ ������Ʈ �ı�
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ShowDamage(int damage, Vector3 spawnPosition)
    {
        string damageStr = damage.ToString();
        Vector3 nextPosition = spawnPosition; // ����: ���� ��ġ�� spawnPosition���� �ʱ�ȭ�մϴ�.

        foreach (char c in damageStr)
        {
            int number = c - '0';
            if (number < 0 || number >= numberSprites.Length) continue;

            GameObject numberObject = Instantiate(numberPrefab, nextPosition, Quaternion.identity);
            numberObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // ũ�� ����
            DamageNumber damageNumberComponent = numberObject.GetComponent<DamageNumber>();
            if (damageNumberComponent != null)
            {
                damageNumberComponent.SetupNumber(number, nextPosition);
            }

            // ���� SpriteRenderer�� �ʺ� ����Ͽ� ���� ��ġ�� �����մϴ�.
            SpriteRenderer sr = numberObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                nextPosition.x += sr.bounds.size.x * 1.0f; // ũ�� ������ ���� ��ġ ����
            }
        }
    }


    public void SetupNumber(int number, Vector3 spawnPosition)
    {
        // �� �Լ������� ���� ���ڸ� �����մϴ�.
        if (number < 0 || number >= numberSprites.Length) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = numberSprites[number];
        }
        transform.position = spawnPosition;
    }
}
