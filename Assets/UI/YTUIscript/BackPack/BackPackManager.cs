using UnityEngine;
using UnityEngine.UI;

public class BackPackManager : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public Button stickerButton; // Reference to the Sticker button
    public Button consumableButton; // Reference to the Consumable button
    public Button importantItemButton; // Reference to the Important Item button

    void Start()
    {
        // Add listeners to the buttons
        stickerButton.onClick.AddListener(OnStickerButtonClick);
        consumableButton.onClick.AddListener(OnConsumableButtonClick);
        importantItemButton.onClick.AddListener(OnImportantItemButtonClick);
    }

    void OnStickerButtonClick()
    {
        // Change the Animator state to ToSticker
        animator.SetTrigger("ToSticker");
    }

    void OnConsumableButtonClick()
    {
        // Handle Consumable button click
        // animator.SetTrigger("ToConsumable"); // Example if you have another state
    }

    void OnImportantItemButtonClick()
    {
        // Handle Important Item button click
        // animator.SetTrigger("ToImportantItem"); // Example if you have another state
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("ToSticker");
        }
    }
}