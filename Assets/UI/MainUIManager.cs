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
               PuaseMenuCanvas.gameObject.SetActive(true);
                CurrentAnimator = PuaseMenuCanvas.GetComponent<Animator>();
                CurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                CurrentAnimator.Play("EnterPuaseMenu");
                currentState = GameState.PauseMenu;
                return;
            case GameState.PauseMenu:
                CurrentAnimator.SetTrigger("ExitPauseMenu");
                currentState = GameState.Gaming;
                return;

        }

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

        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
