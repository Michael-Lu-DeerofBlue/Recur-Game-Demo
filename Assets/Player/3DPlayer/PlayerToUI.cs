using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerToUI : MonoBehaviour
{
    public TextMeshProUGUI MagneticBoots;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MagneticBootsUI(string value)
    {
        string text = "Magnetic Boots: ";
        MagneticBoots.text = text + value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
