using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconShow : MonoBehaviour
{
    public GameObject icon;
    // Start is called before the first frame update
  public void Show()
    {
        icon.SetActive(true);
    }

    public void Hide()
    {
        icon.SetActive(false);

    }
}
