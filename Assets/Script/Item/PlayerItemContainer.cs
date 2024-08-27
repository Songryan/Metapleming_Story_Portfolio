using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemContainer : MonoBehaviour
{
    public static PlayerItemContainer Instance { get; private set; }   // InventoryManager의 싱글톤 인스턴스

    [Header("Player Stat UnderBar Object")]
    public GameObject NickNameText_Charactor;

    [Header("Player Stat UnderBar Object")]
    public GameObject LV;
    public GameObject NickNameText;

    [Header("Player Inventory")]
    public Dictionary<string, Item> inventoryItems;   // 인벤토리 내 아이템 목록

    public Animator effectAnimator;

    //[Header("Player Money")]
    public int playerMoney { get; set; } // 플레이어의 돈을 저장하는 변수

    //플레이어 스탯
    public string PlayerName { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public int MP { get; set; }
    public int EXP { get; set; }
    public int currentHP { get; set; }
    public int currentMP { get; set; }
    public int currentEXP { get; set; }
    public int STR { get; set; }
    public int DEX { get; set; }
    public int INT { get; set; }
    public int LUK { get; set; }
    public int POINT { get; set; }
    public int skillDamage { get; set; }
    public int AttackDamage { get; set; }

    //Bar Image
    [Header("Bar Image")]
    public Image hpBar;
    public Image mpBar;
    public Image expBar;

    [Header("Bar Text")]
    public GameObject HpBarText;
    public GameObject MpBarText;
    public GameObject ExpBarText;
    private TextMeshProUGUI hpText, mpText, expText;

    private void Awake()
    {
        playerMoney = 10000;

        inventoryItems = new Dictionary<string, Item>();

        PlayerName = "테스트";
        Level = 1;
        HP = 100;
        MP = 100;
        EXP = 100;
        currentHP = 50;
        currentMP = 50;
        currentEXP = 0;
        STR = 4;
        DEX = 4;
        INT = 4;
        LUK = 4;
        POINT = 0;

        //this.LV = LV.GetComponent<TextMeshPro>();

        // 싱글톤 인스턴스를 설정합니다.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 오브젝트가 파괴되지 않도록 합니다.
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하는 경우 현재 오브젝트를 파괴합니다.
        }
    }

    private void Start()
    {
        NickNameText.GetComponent<TextMeshProUGUI>().text = PlayerName;
        NickNameText_Charactor.GetComponent<TextMeshProUGUI>().text = PlayerName;

        hpText = HpBarText.GetComponent<TextMeshProUGUI>();
        mpText = MpBarText.GetComponent<TextMeshProUGUI>();
        expText = ExpBarText.GetComponent<TextMeshProUGUI>();

        StartCoroutine(UpdateStatsUI());
    }

    public int PlayerMoney
    {
        get { return playerMoney; }
        set
        {
            playerMoney = value;
            // 필요하다면 UI 업데이트 로직 호출
            //UpdateMoneyUI();
        }
    }

    private void FixedUpdate()
    {
        if(Level < 10)
            LV.GetComponent<TextMeshProUGUI>().text = "0" + Level.ToString();
        else
            LV.GetComponent<TextMeshProUGUI>().text = Level.ToString();

        // 경험치가 최대치를 넘었는지 확인하고 레벨업 처리합니다.
        CheckForLevelUp();

        //Attack Skiil 데미지연산처리
        CalcDamege();
    }

    private void CalcDamege()
    {
        // 기본공격 합연산
        AttackDamage = 6 + STR;
        // 스킬공격 곱연산
        skillDamage = (int)(20 + Mathf.Round(INT*1.5f));
    }
    private void CheckForLevelUp()
    {
        while (currentEXP >= EXP)
        {
            currentEXP -= EXP; // 다음 레벨로 필요한 경험치 만큼 감소
            Level++; // 레벨 증가
            EXP = CalculateExpForNextLevel(Level); // 다음 레벨에 필요한 경험치를 계산합니다.

            // 스탯 증가 로직 (예시)
            POINT += 4;

            HP += 20;
            MP += 20;
            
            currentHP = HP;
            currentMP = MP;

            // 필요하다면 여기에서 레벨업 애니메이션, 사운드 등을 처리할 수 있습니다.
            // 레벨업 애니메이션 실행
            TriggerLevelUpAnimation();

            // UI 업데이트
            //NickNameText.GetComponent<TextMeshProUGUI>().text = $"Lv. {Level} {PlayerName}";
            //UpdateStatBar(hpBar, currentHP / (float)HP);
            //UpdateStatBar(mpBar, currentMP / (float)MP);
            //UpdateStatBar(expBar, currentEXP / (float)EXP);
        }
    }

    public void TriggerLevelUpAnimation()
    {
        StartCoroutine(LevelUpSequence());
    }

    private IEnumerator LevelUpSequence()
    {
        effectAnimator.SetBool("LevelUP", true); // 애니메이션 시작
        yield return new WaitForSeconds(2.4f); // 2초 대기
        effectAnimator.SetBool("LevelUP", false); // 애니메이션 종료
    }

    //public void levelUPStart()
    //{
    //    effectAnimator.SetBool("LevelUP", true);
    //}
    //public void levelUPEnd()
    //{
    //    effectAnimator.SetBool("LevelUP", false);
    //}

    private int CalculateExpForNextLevel(int level)
    {
        // 다음 레벨에 필요한 경험치 계산 로직
        // 예를 들어 각 레벨마다 100씩 증가한다고 가정
        return 100 * level;
    }

    // 코루틴을 사용한 이미지 점점 변경 너낌적너낌
    IEnumerator UpdateStatsUI()
    {
        float textChangeRate = 0.3f; // 텍스트가 변경될 비율을 정의합니다.

        while (true)
        {
            // HP, MP, EXP 바 업데이트
            UpdateStatBar(hpBar, currentHP / (float)HP);
            UpdateStatBar(mpBar, currentMP / (float)MP);
            UpdateStatBar(expBar, currentEXP / (float)EXP);

            UpdateStatText(hpText, currentHP, HP, textChangeRate);
            UpdateStatText(mpText, currentMP, MP, textChangeRate);
            UpdateStatText(expText, currentEXP, EXP, textChangeRate);

            yield return null; // 다음 프레임까지 기다립니다.
        }
    }

    void UpdateStatBar(Image bar, float targetFill)
    {
        // 상태 바가 각 프레임마다 업데이트 될 수치입니다.
        // 이 값을 조절하여 바의 채워짐 속도를 변경할 수 있습니다.
        float changeAmount = 0.005f; 

        // 현재 바의 채워진 정도와 목표 채워진 정도의 차이가 changeAmount보다 큰 경우 점진적으로 채워나갑니다.
        if (Mathf.Abs(bar.fillAmount - targetFill) > changeAmount)
        {
            // 바의 채워짐 정도를 목표 채워짐 정도에 점점 가깝게 조정합니다.
            // 목표가 현재보다 크면 증가, 작으면 감소시킵니다.
            bar.fillAmount += (targetFill - bar.fillAmount > 0) ? changeAmount : -changeAmount;
        }
        else
        {
            // 만약 차이가 changeAmount 이하라면 바로 목표 채워짐 정도를 적용합니다.
            // 이는 미세 조정을 마무리하고 정확한 위치에 맞추는 역할을 합니다.
            bar.fillAmount = targetFill;
        }
    }

    void UpdateStatText(TextMeshProUGUI textMesh, int currentValue, int maxValue, float changeRate)
    {
        int displayedValue;
        if (int.TryParse(textMesh.text.Split('/')[0], out displayedValue))
        {
            // 증가하는 보간 비율
            float newValue = Mathf.Lerp(displayedValue, currentValue, changeRate);
            int newDisplayedValue = Mathf.RoundToInt(newValue);

            // 목표 값에 가까워졌을 때 목표 값으로 직접 설정
            if (Mathf.Abs(newDisplayedValue - currentValue) <= 1)
            {
                newDisplayedValue = currentValue;
            }

            textMesh.text = $"{newDisplayedValue}/{maxValue}";
        }
        else
        {
            // 파싱 실패 시 텍스트를 초기화
            textMesh.text = $"{currentValue}/{maxValue}";
        }
    }



}
