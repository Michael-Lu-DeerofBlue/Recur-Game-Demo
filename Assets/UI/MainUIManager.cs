using Pathfinding.RVO;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    public Animator SystemMenuAnimator;
    public GameObject SettingMenu;
    public Canvas BackPackCanvas;
    public Canvas PuaseMenuCanvas;
    public Canvas EquipCanvas;
    public Canvas CollectionCanvas;
    public Animator CurrentAnimator;
    public GameObject stickerManager;
    public static string gameStateString = "gaming";
    public enum GameState
    {
        Gaming,
        PauseMenu,
        Backpack,
        Equip,
        Collection,
        Setting
    }
    public GameState currentState=GameState.Gaming;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnEsc()
    {
        switch (currentState)
        {
            case GameState.Gaming:
                UnlockCursor();
                PauseGame();
                PuaseMenuCanvas.gameObject.SetActive(true);
                CurrentAnimator = PuaseMenuCanvas.GetComponent<Animator>();
                CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                CurrentAnimator.Play("EnterPuaseMenu");
                currentState = GameState.PauseMenu;
                gameStateString = "pauseMenu";
                return;
            case GameState.PauseMenu:
                LockCursor();
                ResumeGame();
                CurrentAnimator.SetTrigger("ExitPauseMenu");
                currentState = GameState.Gaming;
                gameStateString = "gaming";
                return;
            case GameState.Backpack:
                CurrentAnimator.Play("ExitBackPack");
                PuaseMenuCanvas.gameObject.SetActive(true);
                CurrentAnimator = PuaseMenuCanvas.GetComponent<Animator>();
                CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                CurrentAnimator.Play("EnterPuaseMenu");
                currentState = GameState.PauseMenu;
                gameStateString = "pauseMenu";
                return;
            case GameState.Equip:
                if (AddStickerManager.inStickerSelection)
                {
                    stickerManager.GetComponent<AddStickerManager>().ExitSecletion();
                    return;
                }
                else
                {
                    CurrentAnimator.Play("ExitEquip");
                    PuaseMenuCanvas.gameObject.SetActive(true);
                    CurrentAnimator = PuaseMenuCanvas.GetComponent<Animator>();
                    CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                    CurrentAnimator.Play("EnterPuaseMenu");
                    currentState = GameState.PauseMenu;
                    gameStateString = "pauseMenu";
                    return;
                }
            case GameState.Collection:
                CurrentAnimator.Play("ExitCollection");
                PuaseMenuCanvas.gameObject.SetActive(true);
                CurrentAnimator = PuaseMenuCanvas.GetComponent<Animator>();
                CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                CurrentAnimator.Play("EnterPuaseMenu");
                currentState = GameState.PauseMenu;
                gameStateString = "pauseMenu";
                return;
            case GameState.Setting:
                CurrentAnimator.Play("ExitSystem");
                SettingMenu.gameObject.SetActive(false);
                PuaseMenuCanvas.gameObject.SetActive(true);
                CurrentAnimator = PuaseMenuCanvas.GetComponent<Animator>();
                CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                CurrentAnimator.Play("EnterPuaseMenu");
                currentState = GameState.PauseMenu;
                gameStateString = "pauseMenu";
                return;
        }

    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        if (DialogueManager.isConversationActive)
        {
            DialogueManager.Pause();
        }
        Time.timeScale = 0;
    }

    public void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (DialogueManager.isConversationActive)
        {
            DialogueManager.Unpause();
        }
        Time.timeScale = 1;
    }

    public void ButtonClicked(string button)
    {
        switch (button)
        {
            case "Backpack":
                CurrentAnimator.SetTrigger("ExitPauseMenu");
                currentState = GameState.Backpack;
                BackPackCanvas.gameObject.SetActive(true);
                CurrentAnimator= BackPackCanvas.GetComponent<Animator>();
                CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                return;
            case "Equip":
                CurrentAnimator.SetTrigger("ExitPauseMenu");
                currentState = GameState.Equip;
                EquipCanvas.gameObject.SetActive(true);
                CurrentAnimator = EquipCanvas.GetComponent<Animator>();
                CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                return;
            case "Collection":
                CurrentAnimator.SetTrigger("ExitPauseMenu");
                currentState = GameState.Collection;
                CollectionCanvas.gameObject.SetActive(true);
                CurrentAnimator = CollectionCanvas.GetComponent<Animator>();
                CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                return;
            case "System":
                CurrentAnimator.SetTrigger("ExitPauseMenu");
                currentState = GameState.Setting; 
                CurrentAnimator = SystemMenuAnimator.GetComponent<Animator>();
                CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                CurrentAnimator.Play("EnterSystem");
                SettingMenu.gameObject.SetActive(true);
                return;

        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
