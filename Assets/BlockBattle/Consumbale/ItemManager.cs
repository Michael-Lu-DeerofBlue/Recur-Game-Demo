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
    public GameObject Effect;
    public Sprite[] VFX;
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
    public Button showHideButton;
    public bool playingConsumableAnim = false;
    private SoundManager soundManager;


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
         showHideButton.onClick.AddListener(SwitchInventory);  
        soundManager = FindObjectOfType<SoundManager>();
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
        if (inventoryButton != null)
        {
            inventoryButton.image.alphaHitTestMinimumThreshold = 0.1f;
        }
    }
    void UseItem(InventoryButton inventoryButton)
    {
        string itemName = inventoryButton.image.sprite.name;
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
        SpriteRenderer effectSpriteRenderer = Effect.GetComponent<SpriteRenderer>();

        if (effectSpriteRenderer == null)
        {
            Debug.LogError("Effect game object does not have a SpriteRenderer component.");
            return;
        }

        // 生成新的VFX名字
        string vfxName = itemName + "VFX";

        Sprite vfxSprite = null;
        foreach (var sprite in VFX)
        {
            if (sprite.name == vfxName)
            {
                vfxSprite = sprite;
                break;
            }
        }

        if (vfxSprite != null)
        {
            StartCoroutine(FlashEffect(effectSpriteRenderer, vfxSprite));
        }
    }

    IEnumerator FlashEffect(SpriteRenderer effectSpriteRenderer, Sprite vfxSprite)
    {
        Sprite originalSprite = effectSpriteRenderer.sprite;
        float flashDuration = 0.8f;
        float flashInterval = 0.1f;
        float elapsedTime = 0f;
        bool isFlashing = true;

        effectSpriteRenderer.sprite = vfxSprite;

        while (elapsedTime < flashDuration)
        {
            effectSpriteRenderer.enabled = isFlashing;
            isFlashing = !isFlashing;
            elapsedTime += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        effectSpriteRenderer.sprite = emptySprite;
        effectSpriteRenderer.enabled = true;
    }

    void OnButtonClick(InventoryButton inventoryButton)
    {
        if (battleManager.DisablePlayerInput == true || enable==false || playingConsumableAnim==true) return;
        string itemName = inventoryButton.image.sprite.name;

        if (inventory.ContainsKey(itemName))
        {
            soundManager.PlaySound("UseItem");
            playingConsumableAnim = true;

            StartCoroutine(AnimateButton(inventoryButton.button.GetComponent<RectTransform>(),inventoryButton));

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
            Animator.Play("ShowInventory");
        }
        else
        {
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


    IEnumerator AnimateButton(RectTransform rectTransform, InventoryButton inventorybutton)
    {
        Vector3 originalPosition = rectTransform.anchoredPosition3D;
        Quaternion originalRotation = rectTransform.localRotation;

        Vector3 targetPosition1 = new Vector3(-990.8f, 145.5f, 0);
        Quaternion targetRotation1 = Quaternion.Euler(0, 0, 90);

        Vector3 targetPosition2 = new Vector3(-1046.6f, 145.5f, 0);

        // Move to targetPosition1 and targetRotation1 in 0.3 seconds
        yield return StartCoroutine(LerpRectTransform(rectTransform, targetPosition1, targetRotation1, 0.2f));


        yield return new WaitForSeconds(0.1f);

        // Move to targetPosition2 while keeping the rotation the same in 0.2 seconds
        yield return StartCoroutine(LerpRectTransform(rectTransform, targetPosition2, targetRotation1, 0.2f));

        yield return new WaitForSeconds(0.3f);
        UseItem(inventorybutton);
        // Move back to targetPosition1 in 0.15 seconds
        yield return StartCoroutine(LerpRectTransform(rectTransform, targetPosition1, targetRotation1, 0.2f));

        // Pause for 0.1 seconds
        yield return new WaitForSeconds(0.1f);

        // Move back to the original position and rotation in 0.15 seconds
        yield return StartCoroutine(LerpRectTransform(rectTransform, originalPosition, originalRotation, 0.2f));
        playingConsumableAnim = false;
    }

    IEnumerator LerpRectTransform(RectTransform rectTransform, Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Vector3 startPosition = rectTransform.anchoredPosition3D;
        Quaternion startRotation = rectTransform.localRotation;
        float time = 0;

        while (time < duration)
        {
            rectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, targetPosition, time / duration);
            rectTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition3D = targetPosition;
        rectTransform.localRotation = targetRotation;
    }
}


