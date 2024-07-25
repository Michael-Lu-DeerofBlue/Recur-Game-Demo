using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintKeyTracker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int number = gameObject.GetComponent<HintController>().currentIndex;
        if (number == 0)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                gameObject.GetComponent<HintController>().SwitchTip();
            }
        }
        else if (number == 1)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.A))
            {
                gameObject.GetComponent<HintController>().SwitchTip();
            }
        }
        else if (number == 2)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                gameObject.GetComponent<HintController>().SwitchTip();
            }
        }
        else if (number == 3 || number == 4 || number == 5 || number == 6 || number == 7)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space))
            {
                gameObject.GetComponent<HintController>().SwitchTip();
            }
        }
    }
}
