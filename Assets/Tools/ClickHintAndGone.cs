using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHintAndGone : MonoBehaviour
{
    public GameObject hintManager;
    // Start is called before the first frame update
    private void OnMouseDown()
    {
        hintManager.GetComponent<HintController>().SwitchTip();
    }
}
