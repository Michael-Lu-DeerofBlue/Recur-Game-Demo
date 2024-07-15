using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public GameObject common;
    public GameObject elite;
    public GameObject epic;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Clear();
        common.SetActive(true);
    }

    void Clear()
    {
        common.SetActive(false);
        elite.SetActive(false);
        epic.SetActive(false);
    }

    // Update is called once per frame
    public void OnCommonClicked()
    {
        Clear();
        common.SetActive(true);
    }
    public void OnEliteClicked()
    {
        Clear();
        elite.SetActive(true);
    }
    public void OnEpicClicked()
    {
        Clear();
        epic.SetActive(true);
    }
}
