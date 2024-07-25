using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BackPackManager : MonoBehaviour
{
    public Canvas BackPackCanvas;
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
    public GameObject Sprint;
    public GameObject Key;
    public GameObject Flashlight;
    public GameObject Grappling;
    public GameObject Mag;

    void Start()
    {
        // Add listeners to the buttons
        buttons = new Button[] { ConsumableButton, StickerButton, ImportantButton };
        currentIndex = 0;
        
    }

    private void OnEnable()
    {
        RefreshTable();
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
            RefreshTable();
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
            RefreshTable();
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
            RefreshTable();
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
        StickersInventory = InventoryManager.StickersInventory;
        stickersNumber[0] = StickersInventory["Critical"];
        stickersNumber[1] = StickersInventory["Pierce"];
        stickersNumber[2] = StickersInventory["Sober"];
        stickersNumber[3] = StickersInventory["Swordmaster"];
        stickersNumber[4] = StickersInventory["Gunslinger"];
        for (int i = 0; i < stickersNumber.Count; i++)
        {
            stickersTexts[i].text = stickersNumber[i].ToString();
        }
        stickerItem.SetActive(true);
    }

    void ImportantsRefresh()
    {
        Sprint.SetActive(InventoryManager.sprintB);
        Key.SetActive(InventoryManager.keyB);
        Flashlight.SetActive(InventoryManager.flashlightB);
        Grappling.SetActive(InventoryManager.grappleB);
        Mag.SetActive(InventoryManager.bootsB);
        importantItem.SetActive(true);
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
                    RefreshTable();
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
                    RefreshTable();
                }
            }
        }
    }

    IEnumerator EnableInputAfterDelay(float delay)
    {
        
        yield return new WaitForSecondsRealtime(delay);
        canAcceptInput = true; 
    }

    public void CloseCanvas()
    {
        BackPackCanvas.gameObject.SetActive(false);
    }
}