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
        flowchart.ExecuteBlock("WhiteScreen");
        StartCoroutine(LoadBattle());
    }

    public IEnumerator LoadBattle()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("BattleLevel");
    }

    public void SaveData()
    {
        Vector3 location = player.transform.position;
        ES3.Save("location", location);

    }
}
