using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public InputActionAsset InputActionAsset; // InputActionAsset을 Inspector에서 할당
    private InputAction InvenAction;

    //public InputActionAsset StatActionsAsset; // InputActionAsset을 Inspector에서 할당
    private InputAction StatAction;

    //public InputActionAsset EquipActionsAsset; // InputActionAsset을 Inspector에서 할당
    private InputAction EquipAction;

    public GameObject InvenPanel; // 인벤 패널
    public GameObject StatPanel; // 스탯 패널
    public GameObject EquipPanel; // 장비 패널

    public GameObject StatWindow; // Stat 정보창

    public GameObject CanvasInterface; // Stat 정보창

    private void Awake()
    {
        InvenPanel.SetActive(false);
        StatPanel.SetActive(false);
        EquipPanel.SetActive(false);

        var InputActionMap = InputActionAsset.FindActionMap("UI", true);
        InvenAction = InputActionMap.FindAction("Inventory", true);

        //var StatActionMap = InputActionAsset.FindActionMap("UI", true);
        StatAction = InputActionMap.FindAction("Stat", true);

        //var EquipActionMap = InputActionAsset.FindActionMap("UI", true);
        EquipAction = InputActionMap.FindAction("Equip", true);
    }

    private void OnEnable()
    {
        InvenAction.Enable();
        InvenAction.performed += PressInvenOpen;

        StatAction.Enable();
        StatAction.performed += PressStatOpen;

        EquipAction.Enable();
        EquipAction.performed += PressEquipOpen;
    }

    private void OnDisable()
    {
        InvenAction.performed -= PressInvenOpen;
        InvenAction.Disable();

        StatAction.performed -= PressStatOpen;
        StatAction.Disable();

        EquipAction.performed -= PressEquipOpen;
        EquipAction.Disable();
    }


    public void PressInvenOpen(InputAction.CallbackContext context)
    {
        bool isPanelActive = InvenPanel.activeSelf;
        InvenPanel.SetActive(!isPanelActive);

        if (!isPanelActive)
        {
            InventoryManager.Instance.OnClickALL();
        }
        else
        {
            DestroyTooltipContainers();
        }
    }

    public void PressStatOpen(InputAction.CallbackContext context)
    {
        bool isPanelActive = StatPanel.activeSelf;
        StatPanel.SetActive(!isPanelActive);

        StatWindow.GetComponent<StatUpdate>().Setup();
    }
    public void PressEquipOpen(InputAction.CallbackContext context)
    {
        bool isPanelActive = EquipPanel.activeSelf;
        EquipPanel.SetActive(!isPanelActive);

    }

    void DestroyTooltipContainers()
    {
        // Canvas 컴포넌트를 가져옵니다.
        Canvas canvas = CanvasInterface.GetComponent<Canvas>();

        if (canvas != null)
        {
            // Canvas의 모든 자식 오브젝트를 순회합니다.
            foreach (Transform child in canvas.transform)
            {
                // 이름이 "TooltipContainer"인 자식 오브젝트를 찾습니다.
                if (child.name == "TooltipContainer")
                {
                    // 해당 오브젝트를 파괴합니다.
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
