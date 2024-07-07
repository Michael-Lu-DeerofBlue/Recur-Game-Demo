using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemManager : MonoBehaviour
{
    [System.Serializable]
    public class InventoryButton
    {
        public Button button; 
        public Image image;
        public TMPro.TextMeshProUGUI quantityText;
    }

    public List<InventoryButton> inventoryButtons = new List<InventoryButton>(); 
    public List<Sprite> itemSpritesList = new List<Sprite>();
    public Sprite emptySprite;
    private ItemEventHandler itemEventHandler;
    private BattleManager battleManager;
    private Dictionary<string, Sprite> itemSprites = new Dictionary<string, Sprite>();
    public Dictionary<string, int> inventory = new Dictionary<string, int>();
    public GameObject InventoryPiviot;
    private bool enable = false;
    private Coroutine rotationCoroutine;
    private TipsInfo tipInfo;
    public Animator Animator;



    void Start()
    {
       
        battleManager = FindObjectOfType<BattleManager>();
        itemEventHandler = GetComponent<ItemEventHandler>();
        tipInfo=FindObjectOfType<TipsInfo>();
        foreach (var sprite in itemSpritesList)
        {
            itemSprites[sprite.name] = sprite;
        }


        inventory = ConsumablesManager.TestConsumablesInventory;

        int index = 0;
        foreach (var item in inventory)
        {
            if (index >= inventoryButtons.Count)
                break;


            SetButton(inventoryButtons[index], item.Key, item.Value);
            index++;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchInventory();
        }
    }

    void SetButton(InventoryButton inventoryButton, string itemName, int quantity)
    {
        if (itemSprites.TryGetValue(itemName, out Sprite sprite))
        {
            inventoryButton.image.sprite = sprite;
            inventoryButton.quantityText.text = quantity.ToString();
            inventoryButton.button.onClick.AddListener(() => OnButtonClick(inventoryButton));


            EventTrigger eventTrigger = inventoryButton.button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => OnButtonHover(inventoryButton));
            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => OnButtonExit(inventoryButton));
            eventTrigger.triggers.Add(entryExit);
        }
        else
        {
            Debug.LogWarning("Item sprite not found for: " + itemName);
        }
    }
    void OnButtonClick(InventoryButton inventoryButton)
    {
        
        if (battleManager.DisablePlayerInput == true || enable==false) return;
        string itemName = inventoryButton.image.sprite.name;

        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName]--;

            if (inventory[itemName] <= 0)
            {

                inventoryButton.image.sprite = emptySprite;
                inventoryButton.quantityText.text = "";
            }
            else
            {
                inventoryButton.quantityText.text = inventory[itemName].ToString();
            }

            itemEventHandler.HandleItemEvent(itemName);
        }
    }
    void SwitchInventory()
    {
        AnimatorStateInfo currentState  =Animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("ShowInventory") && currentState.normalizedTime < 1.0f ||
           currentState.IsName("HideInventory") && currentState.normalizedTime < 1.0f)
        {
            return;
        }
        enable =!enable;
        if (enable)
        {
            Debug.Log(enable);
            Animator.Play("ShowInventory");
        }
        else
        {
            Debug.Log(enable);
            Animator.Play("HideInventory");
        }
    }





    void OnButtonHover(InventoryButton inventoryButton)
    {
        string itemName = inventoryButton.image.sprite.name;
        tipInfo.FindInventoryTipsContext(itemName);
    }

    void OnButtonExit(InventoryButton inventoryButton)
    {
        TTooltipSystem.Hide();
    }
}


