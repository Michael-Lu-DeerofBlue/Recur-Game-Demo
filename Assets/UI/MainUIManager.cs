using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    public Animator SystemMenuAnimator;
    public GameObject SettingMenu;
    public Canvas BackPackCanvas;
    public Canvas PuaseMenuCanvas;
    public Canvas EquipCanvas;
    public Canvas CollectionCanvas;
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
                currentState= GameState.PauseMenu;
                Debug.Log("esc escesc");
                return;
            case GameState.PauseMenu:
                Animator currentAnimator = PuaseMenuCanvas.GetComponent<Animator>();
                currentAnimator.SetTrigger("ExitPauseMenu");
                PuaseMenuCanvas.gameObject.SetActive(false);

                currentState = GameState.Gaming;
                return;

        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
