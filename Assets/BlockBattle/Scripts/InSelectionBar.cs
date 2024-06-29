using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSelectionBar : MonoBehaviour
{
    public int index;
    public bool inStorage;
    private SelectionTool selectionToolProcessor;
    public BattleManager battleManager;
    // Start is called before the first frame update
    void Start()
    {
        selectionToolProcessor = FindObjectOfType<SelectionTool>();
        index = gameObject.GetComponent<BlockStageController>().index;
        battleManager= FindObjectOfType<BattleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 1 is the right mouse button
        {
            // Create a ray from the mouse cursor position in the screen space
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the raycast hit this GameObject
                if (hit.transform == transform)
                {
                    selectionToolProcessor.GetComponent<SelectionTool>().addToStorage(index);
                    Destroy(gameObject);
                }
            }
        }
        if (Input.GetMouseButtonDown(0)) // 1 is the right mouse button
        {
            if (battleManager.TimeStop == true)
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the raycast hit this GameObject
                if (hit.transform == transform)
                {
                    if (!selectionToolProcessor.GetComponent<SelectionTool>().stillFalling)
                    {
                        selectionToolProcessor.GetComponent<SelectionTool>().addToFall(index);
                        if (inStorage)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
            
        }
    }
}
