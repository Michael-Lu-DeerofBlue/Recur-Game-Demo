using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerToUI : MonoBehaviour
{
    public TextMeshProUGUI MagneticBoots;
    public TextMeshProUGUI FPS;
    public TextMeshProUGUI Camera;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void MagneticBootsUI(string value)
    {
        string text = "Magnetic Boots: ";
        MagneticBoots.text = text + value;
    }

    public void CameraUI(string value)
    {
        string text = "Camera: ";
        Camera.text = text + value;
    }

    // Update is called once per frame
    void Update()
    {
        string text = "FPS: ";
        FPS.text = text + (1.0f / Time.unscaledDeltaTime).ToString("#0.0");
    }
}
