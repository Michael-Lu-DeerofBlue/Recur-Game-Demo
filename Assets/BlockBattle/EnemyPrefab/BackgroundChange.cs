using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackgroundChange : MonoBehaviour
{
    public int levelNumber;
    public GameObject background;
    public Sprite hallway;
    public Sprite church;
    public Sprite airIsland;
    // Start is called before the first frame update
    void Start()
    {
        if (ES3.KeyExists("Level"))
        {
            levelNumber = ES3.Load<int>("Level");
        }
        switch (levelNumber)
        {
            case 1:
                background.GetComponent<SpriteRenderer>().sprite = hallway;
                break;
            case 2:
                background.GetComponent<SpriteRenderer>().sprite = church;
                break;
            case 3:
                background.GetComponent<SpriteRenderer>().sprite = airIsland;
                break;
            default:
                background.GetComponent<SpriteRenderer>().sprite = airIsland;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
