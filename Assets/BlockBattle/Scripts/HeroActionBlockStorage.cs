using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroActionBlockStorage : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject SelectionTool;
    private Dictionary<int, int> actionBlockDictionary = new Dictionary<int, int>();
    void Start()
    {
        actionBlockDictionary = new Dictionary<int, int>
        {
            { 0, 0 },
            { 1, 1 },
            { 2, 2 },
            { 3, 3 },
            { 4, 4 },
            { 5, 5 },
            { 6, 6 },
        };
        
        SelectionTool.GetComponent<SelectionTool>().actionBlockDictionary = actionBlockDictionary;
        SelectionTool.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
