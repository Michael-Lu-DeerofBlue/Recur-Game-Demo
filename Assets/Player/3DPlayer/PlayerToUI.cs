using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerToUI : MonoBehaviour
{
    public TextMeshProUGUI MagneticBoots;
    public TextMeshProUGUI FPS;
    public TextMeshProUGUI Camera;
    public TextMeshProUGUI HP;

    [SerializeField] private TextMeshProUGUI promptText;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void MagneticBootsUI(string value)
    {
        //string text = "Magnetic Boots: ";
        //MagneticBoots.text = text + value;
    }

    public void CameraUI(string value)
    {
        //string text = "Camera: ";
        //Camera.text = text + value;
    }

    public void UpdateHP(int value)
    {
        //string text = "HP: ";
        //HP.text = text + value;
    }

    public void UpdateText(string promptMessage)
    {
       // promptText.text = promptMessage;
    }

    // Update is called once per frame
    void Update()
    {
        string text = "FPS: ";
        FPS.text = text + (1.0f / Time.unscaledDeltaTime).ToString("#0.0");
    }
}
