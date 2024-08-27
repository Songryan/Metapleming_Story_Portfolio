using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUpdate : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI HP;
    public TextMeshProUGUI MP;
    public TextMeshProUGUI EXP;
    public TextMeshProUGUI STR;
    public TextMeshProUGUI DEX;
    public TextMeshProUGUI INT;
    public TextMeshProUGUI LUK;
    public TextMeshProUGUI POINT;

    void Awake()
    {
        // 자식 컴포넌트에서 UI 컴포넌트 찾기
        PlayerName = transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
        Level = transform.Find("Level").GetComponent<TextMeshProUGUI>();
        HP = transform.Find("HP").GetComponent<TextMeshProUGUI>();
        MP = transform.Find("MP").GetComponent<TextMeshProUGUI>();
        EXP = transform.Find("EXP").GetComponent<TextMeshProUGUI>();
        STR = transform.Find("STR").GetComponent<TextMeshProUGUI>();
        DEX = transform.Find("DEX").GetComponent<TextMeshProUGUI>();
        INT = transform.Find("INT").GetComponent<TextMeshProUGUI>();
        LUK = transform.Find("LUK").GetComponent<TextMeshProUGUI>();
        POINT = transform.Find("POINT").GetComponent<TextMeshProUGUI>();
    }

    public void Setup()
    {
        PlayerName.text = PlayerItemContainer.Instance.PlayerName;
        Level.text = PlayerItemContainer.Instance.Level.ToString();
        HP.text = PlayerItemContainer.Instance.HP.ToString();
        MP.text = PlayerItemContainer.Instance.MP.ToString();
        EXP.text = PlayerItemContainer.Instance.EXP.ToString();
        STR.text = PlayerItemContainer.Instance.STR.ToString();
        DEX.text = PlayerItemContainer.Instance.DEX.ToString();
        INT.text = PlayerItemContainer.Instance.INT.ToString();
        LUK.text = PlayerItemContainer.Instance.LUK.ToString();
        POINT.text = PlayerItemContainer.Instance.POINT.ToString();
    }

    private void Update()
    {
        Setup();
        statUPactivation();
    }

    void statUPactivation()
    {
        // 만약 포인트가 0 이상이면 버튼을 활성화합니다.
        if (PlayerItemContainer.Instance.POINT > 0)
        {
            // 모든 Button 컴포넌트를 찾습니다 (비활성화된 오브젝트도 포함)
            Button[] buttons = GetComponentsInChildren<Button>(true);

            // 각 버튼을 순회하면서 활성화 상태를 설정합니다.
            foreach (Button button in buttons)
            {
                if (button.interactable == true) continue;

                button.interactable = true;
            }
        }
        else
        {
            // 모든 Button 컴포넌트를 찾습니다 (비활성화된 오브젝트도 포함)
            Button[] buttons = GetComponentsInChildren<Button>(true);

            // 각 버튼을 순회하면서 비활성화합니다.
            foreach (Button button in buttons)
            {
                if (button.interactable == false) continue;
                
                button.interactable = false;
            }
        }
    }

    public void strUp()
    {
        PlayerItemContainer.Instance.POINT--;
        PlayerItemContainer.Instance.STR++;
    }
    public void dexUp()
    {
        PlayerItemContainer.Instance.POINT--;

        PlayerItemContainer.Instance.DEX++;
    }
    public void intUp()
    {
        PlayerItemContainer.Instance.POINT--;

        PlayerItemContainer.Instance.INT++;
    }
    public void lukUp()
    {
        PlayerItemContainer.Instance.POINT--;

        PlayerItemContainer.Instance.LUK++;
    }
}