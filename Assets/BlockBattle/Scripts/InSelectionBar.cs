using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InSelectionBar : MonoBehaviour
{
    public int Shapeindex;
    public bool inStorage;
    private SelectionTool selectionToolProcessor;
    public BattleManager battleManager;
    public SelectionToolUI SelectionUI;
    public IntTranslator Translator;
    // Start is called before the first frame update
    void Start()
    {
        Translator = FindObjectOfType<IntTranslator>();
        SelectionUI = FindObjectOfType<SelectionToolUI>();
        selectionToolProcessor = FindObjectOfType<SelectionTool>();
        Shapeindex = gameObject.GetComponent<BlockStageController>().index;
        battleManager= FindObjectOfType<BattleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        if (battleManager.DisablePlayerInput == true) return;// 1 is the right mouse button
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
                    if(SelectionUI.previousGeneratedStorageObject != null)
                    {
                        Renderer renderer = SelectionUI.previousGeneratedStorageObject.GetComponentInChildren<Renderer>();
                        if (renderer != null)
                        {
                            // Get the color of the previous generated storage object
                            Color storedColor = renderer.material.color;

                            // Find the color index
                            int colorIndex = System.Array.IndexOf(Translator.GetComponent<IntTranslator>().Colors, storedColor);

                            // Check if the color index is greater than 7
                            if (colorIndex > 7)
                            {
                                Debug.Log("Color index is greater than 7. Doing nothing.");
                                return; // Do nothing
                            }
                        }
                    }
                    selectionToolProcessor.GetComponent<SelectionTool>().addToStorage(Shapeindex);
                    Destroy(gameObject);
                }
            }
        }
        if (Input.GetMouseButtonDown(0)) // 1 is the right mouse button
        {
            if(battleManager.DisablePlayerInput == true)return;
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
                        if (!inStorage)
                        {
                            selectionToolProcessor.GetComponent<SelectionTool>().addToFall(Shapeindex,false);
                            Destroy(gameObject);
                        }else if (inStorage)
                        {
                            selectionToolProcessor.GetComponent<SelectionTool>().addToFall(Shapeindex,true);
                            Destroy(gameObject);
                        }
                    }
                }
            }
            
        }
    }
}
