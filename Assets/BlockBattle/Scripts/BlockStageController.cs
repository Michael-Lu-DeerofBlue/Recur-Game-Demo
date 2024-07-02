using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockStageController : MonoBehaviour
{
    public bool inSelection;
    public bool inFall;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        if (inSelection) { gameObject.GetComponent<InSelectionBar>().enabled = true; }
        if (inFall) { gameObject.GetComponent<BlockManager>().enabled = true; }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
