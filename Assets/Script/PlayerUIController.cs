using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public InputActionAsset InputActionAsset; // InputActionAsset�� Inspector���� �Ҵ�
    private InputAction InvenAction;

    //public InputActionAsset StatActionsAsset; // InputActionAsset�� Inspector���� �Ҵ�
    private InputAction StatAction;

    //public InputActionAsset EquipActionsAsset; // InputActionAsset�� Inspector���� �Ҵ�
    private InputAction EquipAction;

    public GameObject InvenPanel; // �κ� �г�
    public GameObject StatPanel; // ���� �г�
    public GameObject EquipPanel; // ��� �г�

    public GameObject StatWindow; // Stat ����â

    public GameObject CanvasInterface; // Stat ����â

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
        // Canvas ������Ʈ�� �����ɴϴ�.
        Canvas canvas = CanvasInterface.GetComponent<Canvas>();

        if (canvas != null)
        {
            // Canvas�� ��� �ڽ� ������Ʈ�� ��ȸ�մϴ�.
            foreach (Transform child in canvas.transform)
            {
                // �̸��� "TooltipContainer"�� �ڽ� ������Ʈ�� ã���ϴ�.
                if (child.name == "TooltipContainer")
                {
                    // �ش� ������Ʈ�� �ı��մϴ�.
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
