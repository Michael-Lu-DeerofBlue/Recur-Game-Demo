using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using UnityEngine.SceneManagement;

public abstract class LevelController : MonoBehaviour
{
    public Flowchart flowchart;
    private GameObject player;
    // Start is called before the first frame update

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public virtual void GoToBattle()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ThreeDTo2DData.ThreeDScene = SceneManager.GetActiveScene().name; ;
        flowchart.ExecuteBlock("WhiteScreen");
        StartCoroutine(LoadBattle());
    }

    public IEnumerator LoadBattle()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("BattleLevel");
    }

    public void LoadData()
    {
        Vector3 location = player.transform.position;
        string currentScene = SceneManager.GetActiveScene().name;
        ES3.Load("location", location);
        ES3.Load("Weapons", WeaponManager.WeaponInventory);
        ES3.Load("CurrentWeapon", WeaponManager.CurrentWeapon);
        ES3.Load("Consumables", ConsumablesManager.ConsumablesInventory);
        ES3.Load("CurrentScene", currentScene);
    }

    public void SaveData()
    {
        Vector3 location = player.transform.position;
        string currentScene = SceneManager.GetActiveScene().name;
        ES3.Save("location", location);
        ES3.Save("Weapons", WeaponManager.WeaponInventory);
        ES3.Save("CurrentWeapon", WeaponManager.CurrentWeapon);
        ES3.Save("Consumables", ConsumablesManager.ConsumablesInventory);
        ES3.Save("CurrentScene", currentScene);
    }
}
