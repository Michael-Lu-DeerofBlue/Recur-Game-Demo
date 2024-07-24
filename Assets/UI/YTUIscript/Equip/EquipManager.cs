using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EquipManager : MonoBehaviour
{
    public Canvas EquipCanvas;
    public Animator animator; // Reference to the Animator component
    public Button ActionBankButton; // Reference to the Consumable button
    public Button WeaponButton; // Reference to the Sticker button
    public Button ToolsButton; // Reference to the Important Item button
    public Button ConsumableButton; // Reference to the Important Item button
    public GameObject actionBank;
    public GameObject weapon;
    public GameObject tools;
    public GameObject consumable;

    public Button[] buttons;
    private int currentIndex = 0;
    private bool canAcceptInput = true;
    void Start()
    {
        

    }

    private void OnEnable()
    {
        // Add listeners to the buttons
        buttons = new Button[] { ActionBankButton, WeaponButton, ToolsButton, ConsumableButton };
        ActionBankButton.onClick.AddListener(OnStickerButtonClick);
        WeaponButton.onClick.AddListener(OnConsumableButtonClick);
        ToolsButton.onClick.AddListener(OnImportantItemButtonClick);
        ConsumableButton.onClick.AddListener(OnImportantItemButtonClick);
        currentIndex = 0;
        animator.SetTrigger("To" + buttons[currentIndex].name);
    }

    void Clear() { 
        actionBank.SetActive(false);
        weapon.SetActive(false);
        tools.SetActive(false);
        consumable.SetActive(false);
    }

    void SetToShow()
    {
        if (currentIndex == 0)
        {
            actionBank.SetActive(true);
        }
        else if(currentIndex == 1)
        {
            weapon.SetActive(true);
        }
        else if (currentIndex == 2)
        {
            tools.SetActive(true);
        }
        else if (currentIndex == 3)
        {
            consumable.SetActive(true);
        }
    }

    void OnStickerButtonClick()
    {

    }

    void OnConsumableButtonClick()
    {

    }

    void OnImportantItemButtonClick()
    {

    }

    void Update()
    {
        if (canAcceptInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                int previousIndex = currentIndex;
                currentIndex = (currentIndex + 1) % buttons.Length;
                if (currentIndex != previousIndex)
                {
                    Clear();
                    animator.SetTrigger("Exit" + buttons[previousIndex].name);
                    animator.SetTrigger("To" + buttons[currentIndex].name);
                    canAcceptInput = false;
                     StartCoroutine(EnableInputAfterDelay(1.0f));
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                int previousIndex = currentIndex;
                currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
                if (currentIndex != previousIndex)
                {
                    Clear();
                    animator.SetTrigger("Exit" + buttons[previousIndex].name);
                    animator.SetTrigger("To" + buttons[currentIndex].name);
                    canAcceptInput = false;
                    StartCoroutine(EnableInputAfterDelay(2.0f));
                }
            }
        }
    }

    IEnumerator EnableInputAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        canAcceptInput = true;
        SetToShow();
    }

    public void CloseCanvas()
    {
        EquipCanvas.gameObject.SetActive(false);
    }
}