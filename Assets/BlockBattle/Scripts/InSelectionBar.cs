using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InSelectionBar : MonoBehaviour
{
    public int Shapeindex;
    public bool inStorage;
    private SelectionTool selectionToolProcessor;
    public BattleManager battleManager;
    public SelectionToolUI SelectionUI;
    public IntTranslator Translator;
    public StickerInfo StickerInfo;
    public GameObject BlockTips;
    private TipsInfo tipsInfo;

    public Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        Translator = FindObjectOfType<IntTranslator>();
        SelectionUI = FindObjectOfType<SelectionToolUI>();
        selectionToolProcessor = FindObjectOfType<SelectionTool>();
        Shapeindex = gameObject.GetComponent<BlockStageController>().index;
        battleManager = FindObjectOfType<BattleManager>();
        StickerInfo= FindObjectOfType<StickerInfo>();
        CheckandAttachSticker();
        tipsInfo = FindObjectOfType<TipsInfo>();

    }

    public void CheckandAttachSticker()
    {
        foreach (var sticker in StickerInfo.dataList)
        {

            if (sticker.IndexShape == Shapeindex)
            {
                AttachSpriteToChild(sticker.position1, sticker.StickerName);
                break;
            }
        }
    }
    void AttachSpriteToChild(Vector3 localPosition, string spriteName)
    {
        foreach (Transform child in transform)
        {
            if (child.localPosition == localPosition)
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = child.gameObject.AddComponent<SpriteRenderer>();
                }
                spriteRenderer.sprite = GetSpriteByName(spriteName);

                break;
            }
        }

    }

    Sprite GetSpriteByName(string spriteName)
    {
        if (sprites == null)
        {
            Debug.LogError("Sprites array is null.");
            return null;
        }

        foreach (var sprite in sprites)
        {
            if (sprite.name == spriteName)
            {
                return sprite;
            }
        }
        Debug.LogError($"Sprite with name {spriteName} not found in sprites array.");
        return null; 
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (battleManager.DisablePlayerInput == true) return; // 1 is the right mouse button
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the raycast hit this GameObject
                    if (hit.transform == transform)
                    {
                        if (SelectionUI.previousGeneratedStorageObject != null)
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
        }
        if (Input.GetMouseButtonDown(0)) // 1 is the right mouse button
        {
            if (battleManager.DisablePlayerInput == true) return;
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
                            int position = selectionToolProcessor.GetComponent<SelectionTool>().threeBlockList.IndexOf(Shapeindex);
                            if (position < 0 || position >= selectionToolProcessor.GetComponent<SelectionTool>().threeBlockList.Count)
                            {
                                Debug.Log("Index out of range in InSelectionBar: " + position);
                                return;
                            }

                            selectionToolProcessor.GetComponent<SelectionTool>().addToFall(Shapeindex, false);
                            Destroy(gameObject);
                        }
                        else if (inStorage)
                        {
                            selectionToolProcessor.GetComponent<SelectionTool>().addToFall(Shapeindex, true);
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }
    private void OnMouseEnter()
    {
        if (selectionToolProcessor != null && tipsInfo != null)
        {
            int position = selectionToolProcessor.GetComponent<SelectionTool>().threeBlockList.IndexOf(Shapeindex);
            if (position >= 0 && position < selectionToolProcessor.GetComponent<SelectionTool>().threeColorList.Count)
            {
                int colorIndex = selectionToolProcessor.GetComponent<SelectionTool>().threeColorList[position];
                tipsInfo.FindBlockTipsContext(colorIndex);
            }
            else
            {
                Debug.LogWarning("Index out of range in threeBlockList or threeColorList.");
            }
        }
    }


    private void OnMouseExit()
    {

        TTooltipSystem.Hide();
    }
}
