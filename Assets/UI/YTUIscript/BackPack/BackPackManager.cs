using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BackPackManager : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public Button ConsumableButton; // Reference to the Consumable button
    public Button StickerButton; // Reference to the Sticker button
    public Button ImportantButton; // Reference to the Important Item button


    private Button[] buttons;
    public int currentIndex = 0;
    private bool canAcceptInput = true;

    public GameObject consumablesItem;
    public List<TextMeshProUGUI> consuamblesTexts;
    public List<int> consuamblesNumber;
    private Dictionary<string, int> ConsumablesInventory = new Dictionary<string, int>();

    public GameObject stickerItem;
    private Dictionary<string, int> StickersInventory = new Dictionary<string, int>();
    public List<TextMeshProUGUI> stickersTexts;
    public List<int> stickersNumber;

    public GameObject importantItem;

    void Start()
    {
        // Add listeners to the buttons
        buttons = new Button[] { ConsumableButton, StickerButton, ImportantButton };
        currentIndex = 0; 
    }
    public void OnConsumableButtonClick()
    {
        if (currentIndex != 0)
        {
            animator.SetTrigger("Exit" + buttons[currentIndex].name);
            animator.SetTrigger("To" + ConsumableButton.name);
            currentIndex = 0;
            canAcceptInput = false;
            StartCoroutine(EnableInputAfterDelay(1.0f));
        }
    }
    public void OnStickerButtonClick()
    {
        if (currentIndex != 1)
        {
            animator.SetTrigger("Exit" + buttons[currentIndex].name);
            animator.SetTrigger("To" + StickerButton.name);
            currentIndex = 1;
            canAcceptInput = false;
            StartCoroutine(EnableInputAfterDelay(1.0f));
        }
       
    }
    public void OnImportantItemButtonClick()
    {
        if (currentIndex != 2)
        {
            animator.SetTrigger("Exit" + buttons[currentIndex].name);
            animator.SetTrigger("To" + ImportantButton.name);
            currentIndex = 2;
            canAcceptInput = false;
            StartCoroutine(EnableInputAfterDelay(1.0f));
        }
    }

    void RefreshTable()
    {
        if (currentIndex == 0)
        {
            Clear();
            ConsumablesRefresh();
        }
        else if (currentIndex == 1)
        {
            Clear();
            StickersRefresh();
        }
        else if (currentIndex == 2)
        {
            Clear();
            ImportantsRefresh();
        }
    }

    void Clear()
    {
        consumablesItem.SetActive(false);
        stickerItem.SetActive(false);
        importantItem.SetActive(false);
    }

    void ConsumablesRefresh()
    {
        ConsumablesInventory = InventoryManager.ConsumablesInventory;
        consuamblesNumber[0] = ConsumablesInventory["MedKit"];
        consuamblesNumber[1] = ConsumablesInventory["SprayCan"];
        consuamblesNumber[2] = ConsumablesInventory["Mint"];
        consuamblesNumber[3] = ConsumablesInventory["PaperCutter"];
        consuamblesNumber[4] = ConsumablesInventory["FracturedPocketWatch"];
        for (int i = 0; i < consuamblesNumber.Count; i++)
        {
            consuamblesTexts[i].text = consuamblesNumber[i].ToString();
        }
        consumablesItem.SetActive(true);
    }

    void StickersRefresh()
    {
        ConsumablesInventory = InventoryManager.ConsumablesInventory;
        consuamblesNumber[0] = ConsumablesInventory["MedKit"];
        consuamblesNumber[1] = ConsumablesInventory["SprayCan"];
        consuamblesNumber[2] = ConsumablesInventory["Mint"];
        consuamblesNumber[3] = ConsumablesInventory["PaperCutter"];
        consuamblesNumber[4] = ConsumablesInventory["FracturedPocketWatch"];
        for (int i = 0; i < consuamblesNumber.Count; i++)
        {
            consuamblesTexts[i].text = consuamblesNumber[i].ToString();
        }
        consumablesItem.SetActive(true);
    }

    void ImportantsRefresh()
    {
        ConsumablesInventory = InventoryManager.ConsumablesInventory;
        consuamblesNumber[0] = ConsumablesInventory["MedKit"];
        consuamblesNumber[1] = ConsumablesInventory["SprayCan"];
        consuamblesNumber[2] = ConsumablesInventory["Mint"];
        consuamblesNumber[3] = ConsumablesInventory["PaperCutter"];
        consuamblesNumber[4] = ConsumablesInventory["FracturedPocketWatch"];
        for (int i = 0; i < consuamblesNumber.Count; i++)
        {
            consuamblesTexts[i].text = consuamblesNumber[i].ToString();
        }
        consumablesItem.SetActive(true);
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
                    animator.SetTrigger("Exit" + buttons[previousIndex].name);
                    animator.SetTrigger("To" + buttons[currentIndex].name);
                    canAcceptInput = false;
                    StartCoroutine(EnableInputAfterDelay(1.0f));
                }
            }
        }
    }

    IEnumerator EnableInputAfterDelay(float delay)
    {
        
        yield return new WaitForSecondsRealtime(delay);
        canAcceptInput = true; 
    }
}