using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public Sprite[] numberSprites; // 0-9까지의 숫자 스프라이트 배열
    public GameObject numberPrefab; // 하나의 숫자를 표현하는 프리팹

    public float moveSpeed = 1f; // 숫자가 올라가는 속도
    public float fadeSpeed = 1f; // 페이드 아웃 되는 속도
    public float duration = 1f; // 표시되는 시간

    private float timer; // 현재까지 경과한 시간을 추적

    private void Start()
    {
        // 타이머를 지속 시간으로 초기화
        timer = duration;
    }

    private void Update()
    {
        // 위로 이동
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // 투명도 감소
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = Mathf.Clamp01(color.a - (fadeSpeed * Time.deltaTime));
        spriteRenderer.color = color;

        // 타이머 업데이트 및 시간이 지나면 오브젝트 파괴
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ShowDamage(int damage, Vector3 spawnPosition)
    {
        string damageStr = damage.ToString();
        Vector3 nextPosition = spawnPosition; // 변경: 시작 위치를 spawnPosition으로 초기화합니다.

        foreach (char c in damageStr)
        {
            int number = c - '0';
            if (number < 0 || number >= numberSprites.Length) continue;

            GameObject numberObject = Instantiate(numberPrefab, nextPosition, Quaternion.identity);
            numberObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // 크기 조정
            DamageNumber damageNumberComponent = numberObject.GetComponent<DamageNumber>();
            if (damageNumberComponent != null)
            {
                damageNumberComponent.SetupNumber(number, nextPosition);
            }

            // 계산된 SpriteRenderer의 너비를 사용하여 다음 위치를 조정합니다.
            SpriteRenderer sr = numberObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                nextPosition.x += sr.bounds.size.x * 1.0f; // 크기 조정에 맞춰 위치 조정
            }
        }
    }


    public void SetupNumber(int number, Vector3 spawnPosition)
    {
        // 이 함수에서는 단일 숫자를 설정합니다.
        if (number < 0 || number >= numberSprites.Length) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = numberSprites[number];
        }
        transform.position = spawnPosition;
    }
}
