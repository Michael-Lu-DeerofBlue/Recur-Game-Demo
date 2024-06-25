using System.Collections.Generic;
using UnityEngine;
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

    private Dictionary<string, Sprite> itemSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    void Start()
    {
        itemEventHandler = GetComponent<ItemEventHandler>();

        foreach (var sprite in itemSpritesList)
        {
            itemSprites[sprite.name] = sprite;
        }


        inventory = ConsumablesManager.TestConsumablesInventory;

        foreach (var item in inventory)
        {
            Debug.Log($"Inventory item: {item.Key}, quantity: {item.Value}");
        }

        int index = 0;
        foreach (var item in inventory)
        {
            if (index >= inventoryButtons.Count)
                break;


            SetButton(inventoryButtons[index], item.Key, item.Value);
            index++;
        }
    }

    void SetButton(InventoryButton inventoryButton, string itemName, int quantity)
    {
        if (itemSprites.TryGetValue(itemName, out Sprite sprite))
        {
            inventoryButton.image.sprite = sprite;
            inventoryButton.quantityText.text = quantity.ToString();
            inventoryButton.button.onClick.AddListener(() => OnButtonClick(inventoryButton)); 
            Debug.Log($"Set button for item: {itemName}, quantity: {quantity}, sprite: {sprite.name}");
        }
        else
        {
            Debug.LogWarning("Item sprite not found for: " + itemName);
        }
    }
    void OnButtonClick(InventoryButton inventoryButton)
    {
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
}


