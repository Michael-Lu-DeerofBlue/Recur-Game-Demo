using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackPackManager : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public Button ConsumableButton; // Reference to the Consumable button
    public Button StickerButton; // Reference to the Sticker button
    public Button ImportantButton; // Reference to the Important Item button


    private Button[] buttons;
    private int currentIndex = 0;
    private bool canAcceptInput = true;
    void Start()
    {
        // Add listeners to the buttons
        buttons = new Button[] { ConsumableButton, StickerButton, ImportantButton };


        StickerButton.onClick.AddListener(OnStickerButtonClick);
        ConsumableButton.onClick.AddListener(OnConsumableButtonClick);
        ImportantButton.onClick.AddListener(OnImportantItemButtonClick);


        currentIndex = 0; 
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