using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumablesManager : MonoBehaviour
{

    public static Dictionary<string, int> ConsumablesInventory = new Dictionary<string, int>();
    public static Dictionary<string, int> TestConsumablesInventory = new Dictionary<string, int>()
    {
        { "MedKit", 2 },
        { "SprayCan", 2 },
        { "Mint", 2 },
        { "PaperCutter", 2 },
        { "FracturedPocketWatch", 2 }
    };//temperay inventory for testing



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
