using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemContainer : MonoBehaviour
{
    public static PlayerItemContainer Instance { get; private set; }   // InventoryManager�� �̱��� �ν��Ͻ�

    [Header("Player Stat UnderBar Object")]
    public GameObject NickNameText_Charactor;

    [Header("Player Stat UnderBar Object")]
    public GameObject LV;
    public GameObject NickNameText;

    [Header("Player Inventory")]
    public Dictionary<string, Item> inventoryItems;   // �κ��丮 �� ������ ���

    public Animator effectAnimator;

    //[Header("Player Money")]
    public int playerMoney { get; set; } // �÷��̾��� ���� �����ϴ� ����

    //�÷��̾� ����
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

        PlayerName = "�׽�Ʈ";
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

        // �̱��� �ν��Ͻ��� �����մϴ�.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� ������Ʈ�� �ı����� �ʵ��� �մϴ�.
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϴ� ��� ���� ������Ʈ�� �ı��մϴ�.
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
            // �ʿ��ϴٸ� UI ������Ʈ ���� ȣ��
            //UpdateMoneyUI();
        }
    }

    private void FixedUpdate()
    {
        if(Level < 10)
            LV.GetComponent<TextMeshProUGUI>().text = "0" + Level.ToString();
        else
            LV.GetComponent<TextMeshProUGUI>().text = Level.ToString();

        // ����ġ�� �ִ�ġ�� �Ѿ����� Ȯ���ϰ� ������ ó���մϴ�.
        CheckForLevelUp();

        //Attack Skiil ����������ó��
        CalcDamege();
    }

    private void CalcDamege()
    {
        // �⺻���� �տ���
        AttackDamage = 6 + STR;
        // ��ų���� ������
        skillDamage = (int)(20 + Mathf.Round(INT*1.5f));
    }
    private void CheckForLevelUp()
    {
        while (currentEXP >= EXP)
        {
            currentEXP -= EXP; // ���� ������ �ʿ��� ����ġ ��ŭ ����
            Level++; // ���� ����
            EXP = CalculateExpForNextLevel(Level); // ���� ������ �ʿ��� ����ġ�� ����մϴ�.

            // ���� ���� ���� (����)
            POINT += 4;

            HP += 20;
            MP += 20;
            
            currentHP = HP;
            currentMP = MP;

            // �ʿ��ϴٸ� ���⿡�� ������ �ִϸ��̼�, ���� ���� ó���� �� �ֽ��ϴ�.
            // ������ �ִϸ��̼� ����
            TriggerLevelUpAnimation();

            // UI ������Ʈ
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
        effectAnimator.SetBool("LevelUP", true); // �ִϸ��̼� ����
        yield return new WaitForSeconds(2.4f); // 2�� ���
        effectAnimator.SetBool("LevelUP", false); // �ִϸ��̼� ����
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
        // ���� ������ �ʿ��� ����ġ ��� ����
        // ���� ��� �� �������� 100�� �����Ѵٰ� ����
        return 100 * level;
    }

    // �ڷ�ƾ�� ����� �̹��� ���� ���� �ʳ����ʳ�
    IEnumerator UpdateStatsUI()
    {
        float textChangeRate = 0.3f; // �ؽ�Ʈ�� ����� ������ �����մϴ�.

        while (true)
        {
            // HP, MP, EXP �� ������Ʈ
            UpdateStatBar(hpBar, currentHP / (float)HP);
            UpdateStatBar(mpBar, currentMP / (float)MP);
            UpdateStatBar(expBar, currentEXP / (float)EXP);

            UpdateStatText(hpText, currentHP, HP, textChangeRate);
            UpdateStatText(mpText, currentMP, MP, textChangeRate);
            UpdateStatText(expText, currentEXP, EXP, textChangeRate);

            yield return null; // ���� �����ӱ��� ��ٸ��ϴ�.
        }
    }

    void UpdateStatBar(Image bar, float targetFill)
    {
        // ���� �ٰ� �� �����Ӹ��� ������Ʈ �� ��ġ�Դϴ�.
        // �� ���� �����Ͽ� ���� ä���� �ӵ��� ������ �� �ֽ��ϴ�.
        float changeAmount = 0.005f; 

        // ���� ���� ä���� ������ ��ǥ ä���� ������ ���̰� changeAmount���� ū ��� ���������� ä�������ϴ�.
        if (Mathf.Abs(bar.fillAmount - targetFill) > changeAmount)
        {
            // ���� ä���� ������ ��ǥ ä���� ������ ���� ������ �����մϴ�.
            // ��ǥ�� ���纸�� ũ�� ����, ������ ���ҽ�ŵ�ϴ�.
            bar.fillAmount += (targetFill - bar.fillAmount > 0) ? changeAmount : -changeAmount;
        }
        else
        {
            // ���� ���̰� changeAmount ���϶�� �ٷ� ��ǥ ä���� ������ �����մϴ�.
            // �̴� �̼� ������ �������ϰ� ��Ȯ�� ��ġ�� ���ߴ� ������ �մϴ�.
            bar.fillAmount = targetFill;
        }
    }

    void UpdateStatText(TextMeshProUGUI textMesh, int currentValue, int maxValue, float changeRate)
    {
        int displayedValue;
        if (int.TryParse(textMesh.text.Split('/')[0], out displayedValue))
        {
            // �����ϴ� ���� ����
            float newValue = Mathf.Lerp(displayedValue, currentValue, changeRate);
            int newDisplayedValue = Mathf.RoundToInt(newValue);

            // ��ǥ ���� ��������� �� ��ǥ ������ ���� ����
            if (Mathf.Abs(newDisplayedValue - currentValue) <= 1)
            {
                newDisplayedValue = currentValue;
            }

            textMesh.text = $"{newDisplayedValue}/{maxValue}";
        }
        else
        {
            // �Ľ� ���� �� �ؽ�Ʈ�� �ʱ�ȭ
            textMesh.text = $"{currentValue}/{maxValue}";
        }
    }



}
