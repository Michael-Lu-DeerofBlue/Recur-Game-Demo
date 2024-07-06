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



    void Start()
    {
       
        battleManager = FindObjectOfType<BattleManager>();
        itemEventHandler = GetComponent<ItemEventHandler>();

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
        enable = !enable; // ÇÐ»» enable µÄÖµ
        if (enable)
        {
            Debug.Log("enable is true");
            StartRotation(19.5f);
        }
        else
        {
            Debug.Log("enable is false");
            StartRotation(0f);
        }
    }


    void StartRotation(float targetRotationZ)
    {

        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        rotationCoroutine = StartCoroutine(RotateInventoryPiviot(targetRotationZ));
    }

    IEnumerator RotateInventoryPiviot(float targetRotationZ)
    {
        RectTransform rectTransform = InventoryPiviot.GetComponent<RectTransform>();
        float startRotationZ = rectTransform.localEulerAngles.z;
        float elapsedTime = 0f;
        float duration = 0.2f; 

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newRotationZ = Mathf.Lerp(startRotationZ, targetRotationZ, elapsedTime / duration);
            rectTransform.localEulerAngles = new Vector3(rectTransform.localEulerAngles.x, rectTransform.localEulerAngles.y, newRotationZ);
            yield return null;
        }


        rectTransform.localEulerAngles = new Vector3(rectTransform.localEulerAngles.x, rectTransform.localEulerAngles.y, targetRotationZ);
    }
}


