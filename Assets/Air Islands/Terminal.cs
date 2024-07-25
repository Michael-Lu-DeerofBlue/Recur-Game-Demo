using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

public class Terminal : MonoBehaviour
{
    public GameObject prompt;
    public string text;
    public bool pointed;
    private float holdTime = 2.0f;
    private float timer = 0.0f;
    public GameObject levelController;
    public Image fillingBar;
    public string type;
    public GameObject player;
    public float addedHP = -10;
    public string[] consumables;
    public int amount;
    public bool breaked;
    public GameObject bird;
    // Start is called before the first frame update
    void Start()
    {
        levelController = GameObject.Find("Level Script");
        player = GameObject.Find("Player");
        if (fillingBar != null)
        {
            fillingBar.fillAmount = 0.0f;
        }
    }

    public void ShowPrompt()
    {
        pointed = true;
        prompt.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        pointed = false;
        prompt.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool birdAlive = false;
        if (bird != null)
        {
            birdAlive = false;
        }
        if (pointed && !breaked && !birdAlive)
        {
            if (Input.GetKey(KeyCode.F))
            {
                timer += Time.deltaTime;
                if (timer >= holdTime)
                {
                    levelController.GetComponent<Level3>().TerminalEnterBoard();
                    timer = 0.0f; // Reset timer after the action
                }
                UpdateFillingBar();
            }
            else if (Input.GetKey(KeyCode.H) && type == "Heal")
            {
                timer += Time.deltaTime;
                if (timer >= holdTime)
                {
                    Debug.Log(addedHP);
                    player.GetComponent<InventoryManager>().AddHealth(addedHP);
                    timer = 0.0f; // Reset timer after the action
                }
                UpdateFillingBar();
            }
            else if (Input.GetKey(KeyCode.I) && type == "Chest")
            {
                timer += Time.deltaTime;
                if (timer >= holdTime)
                {
                    foreach (string con in consumables)
                    {
                        player.GetComponent<InventoryManager>().AddConsumables(con, amount);
                    }
                    timer = 0.0f; // Reset timer after the action
                }
                UpdateFillingBar();
            }
            else
            {
                timer = 0.0f; // Reset timer if the key is released
                ResetFillingBar();
            }
        }
        else
        {
            timer = 0.0f; // Reset timer if not pointed
            ResetFillingBar();
        }
    }

    private void UpdateFillingBar()
    {
        if (fillingBar != null)
        {
            fillingBar.fillAmount = timer / holdTime;
        }
    }

    private void ResetFillingBar()
    {
        if (fillingBar != null)
        {
            fillingBar.fillAmount = 0.0f;
        }
    }
    public void Break()
    {
        breaked = true;
        StartCoroutine(BreakTimerStart(3f));
    }

    IEnumerator BreakTimerStart(float delay)
    {
        yield return new WaitForSeconds(delay);
       breaked = false;

    }
}
