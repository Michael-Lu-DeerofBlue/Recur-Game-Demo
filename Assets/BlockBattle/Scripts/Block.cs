using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime = 0.8f;
    public static int height = 24;
    public static int width = 12;
    public static int extendedWidth = 17; // Extended width to display blocks moved to the right side
    public static Transform[,] grid = new Transform[extendedWidth, height];
    private SpawnBlock spawnblock;
    private BattleManager battleManager;
    public GameObject ghostBlock;
    public int id; //used to get a id for the block to check for homogenousty
    private static Dictionary<string, int> globalColorCount = new Dictionary<string, int>();
    private SelectionTool selectionToolProcessor;
    public int colorId;
    public bool lockedRotation;
    public string color;
    public TwoDto3D twoDto3D;
    public StickerInfo StickerInfo;
    public Sprite[] sprites;
    private int Shapeindex;
    private bool isClearingRightSideBlocks = false;
    private int movedLines = 0;
    void Start()
    {
        twoDto3D = FindObjectOfType<TwoDto3D>();
        selectionToolProcessor = FindObjectOfType<SelectionTool>();
        spawnblock = FindObjectOfType<SpawnBlock>();
        battleManager = FindObjectOfType<BattleManager>();
        id = spawnblock.blockIdCounter;
        color=GetBlockColor();
        StickerInfo = FindObjectOfType<StickerInfo>();

       Shapeindex=spawnblock.GetLastSpawnedIndex();
       CheckandAttachSticker();


    }


    void Update()
    {
        if (battleManager.TimeStop == false) {
            if (Input.GetKeyDown(KeyCode.Space))   // Drop the block
        {
                if (battleManager.DisablePlayerInput == true) return;
                while (true)
            {
                transform.position += new Vector3(0, -1, 0);
                if (!ValidMove())
                {
                    transform.position -= new Vector3(0, -1, 0);
                    AddToGrid();
                    CheckForLines();
                    this.enabled = false;
                    break;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (battleManager.DisablePlayerInput == true) return;
                transform.position += new Vector3(-1, 0, 0);
            if (!ValidMove())
                transform.position -= new Vector3(-1, 0, 0);
            FindObjectOfType<SpawnBlock>().SpawnGhostBlock();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
                if (battleManager.DisablePlayerInput == true) return;
                transform.position += new Vector3(1, 0, 0);
            if (!ValidMove())
                transform.position -= new Vector3(1, 0, 0);
            FindObjectOfType<SpawnBlock>().SpawnGhostBlock();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
                if (battleManager.DisablePlayerInput == true) return;
                if (lockedRotation == true) return;
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            if (!ValidMove())
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            FindObjectOfType<SpawnBlock>().SpawnGhostBlock();
        }

        // Handle block falling over time


            if (Time.time - previousTime > (Input.GetKey(KeyCode.S) ? fallTime / 10 : fallTime))
            {
                transform.position += new Vector3(0, -1, 0);
                if (!ValidMove())
                {
                    transform.position -= new Vector3(0, -1, 0);
                    AddToGrid();
                    CheckForLines();
                    this.enabled = false;
                }
                previousTime = Time.time;
            }
        }
    }

    public bool IsOccupied(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        if (x >= 0 && x < extendedWidth && y >= 0 && y < height)
        {
            return grid[x, y] != null;
        }
        return false;
    }

    public Vector3 RoundVector(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    // Check for complete lines and move them to the right side
    void CheckForLines()
    {

        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                MoveLineToRightSide(i);
                RowDown(i);
                battleManager.IntrruptBlockGame();
                battleManager.addclearedlineNumber();
            }
        }
    }

    // Check if a line is complete
    public bool HasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null)
                return false;
        }
        movedLines++;
        return true;

    }


    // Move a complete line to the right side and position it vertically
    void MoveLineToRightSide(int i)
    {
        int startX = extendedWidth - 1; // Display on the right side
        int startY = height - 1;

        
        List<Transform> blocks = new List<Transform>();
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] != null)
            {
                Transform block = grid[j, i];
                blocks.Add(block);
                grid[j, i] = null;
            }
        }



        for (int k = 0; k < blocks.Count; k++)
        {
            Transform block = blocks[k];
            Vector3 newPosition = new Vector3(startX, startY - k, 0);

            while (IsInsideExtendedGrid(newPosition) && grid[(int)newPosition.x, (int)newPosition.y] != null)
            {
                startX--;
                newPosition = new Vector3(startX, startY - k, 0);
            }

            if (IsInsideExtendedGrid(newPosition))
            {
                block.position = newPosition;
                grid[(int)newPosition.x, (int)newPosition.y] = block;
            }
        }
        StartClearBlock();
    }


    // Coroutine to clear connected blocks on the right side with the same color
    IEnumerator ClearRightSideBlocks()
    {
        isClearingRightSideBlocks = true;
        while (battleManager.TimeStop == true)
        {
            yield return new WaitForSeconds(0.3f);

            Transform upleftBlock = FindUpleftBlock();
            if (upleftBlock == null)
            {
                battleManager.ExecuteIconSkill();
                yield break;
            }
            string UpLeftColor = ColorUtility.ToHtmlStringRGBA(upleftBlock.GetComponent<Renderer>().material.color);
            int colorCode = upleftBlock.parent.GetComponent<BlockManager>().colorId;
            int passedInId = upleftBlock.parent.GetComponent<BlockManager>().id;
            List<Transform> blocksToClear = new List<Transform>();
            FindConnectedBlocks(upleftBlock, UpLeftColor, passedInId, blocksToClear);

            foreach (var block in blocksToClear)
            {
                grid[(int)block.position.x, (int)block.position.y] = null;
                Destroy(block.gameObject);
            }
            battleManager.ReceColorMessage(UpLeftColor, blocksToClear.Count);
        }
        isClearingRightSideBlocks = false;
    }
    public void StartClearBlock()
    {
        battleManager.TimeStop = true;
        if (!isClearingRightSideBlocks)
        {
            StartCoroutine(ClearRightSideBlocks());
        }
    }


    Transform FindUpleftBlock()
    {
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = width; x < extendedWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    return grid[x, y];
                }
            }
        }
        return null;
    }


    // Find all connected blocks with the same color and add them to the list
    // Find all connected blocks with the same color and add them to the list
    void FindConnectedBlocks(Transform block, string color, int checkId, List<Transform> blocksToClear)
    {

        if (block == null || blocksToClear.Contains(block)) return;

        string blockColor = ColorUtility.ToHtmlStringRGBA(block.GetComponent<Renderer>().material.color);
        if (blockColor != color) return;
        int otherID = block.parent.GetComponent<BlockManager>().id;
        if (otherID != checkId) return;

        // Check for "Sticker" sprite on block's children

        blocksToClear.Add(block);


        foreach (Transform child in block)
        {
            if (child.GetComponent<SpriteRenderer>().sprite.name == "Critical")
            {
                battleManager.HandleStickerEffect("Critical");
            }
            if (child.GetComponent<SpriteRenderer>().sprite.name == "Piercing")
            {
                battleManager.HandleStickerEffect("Piercing");
            }
            if (child.GetComponent<SpriteRenderer>().sprite.name == "Sober")
            {
                battleManager.HandleStickerEffect("Sober");
            }
            if (child.GetComponent<SpriteRenderer>().sprite.name == "Swordmaster")
            {
                battleManager.HandleStickerEffect("Swordmaster");
            }
            if (child.GetComponent<SpriteRenderer>().sprite.name == "Gunslinger")
            {
                battleManager.HandleStickerEffect("GunSlinger");
            }
        }


            int x = (int)block.position.x;
        int y = (int)block.position.y;

        if (IsInsideExtendedGrid(new Vector3(x + 1, y, 0)))
        {
            if (grid[x + 1, y] != null && !blocksToClear.Contains(grid[x + 1, y]))
            {
                FindConnectedBlocks(grid[x + 1, y], color, checkId, blocksToClear);
            }
        }
        if (IsInsideExtendedGrid(new Vector3(x - 1, y, 0)))
        {
            if (grid[x - 1, y] != null && !blocksToClear.Contains(grid[x - 1, y]))
            {
                FindConnectedBlocks(grid[x - 1, y], color, checkId, blocksToClear);
            }
        }
        if (IsInsideExtendedGrid(new Vector3(x, y + 1, 0)))
        {
            if (grid[x, y + 1] != null && !blocksToClear.Contains(grid[x, y + 1]))
            {
                FindConnectedBlocks(grid[x, y + 1], color, checkId, blocksToClear);
            }
        }
        if (IsInsideExtendedGrid(new Vector3(x, y - 1, 0)))
        {
            if (grid[x, y - 1] != null && !blocksToClear.Contains(grid[x, y - 1]))
            {
                FindConnectedBlocks(grid[x, y - 1], color, checkId, blocksToClear);
            }
        }
    }


    // Move rows down after clearing a line
    void RowDown(int i)
    {
        for (int y = i; y < height - 1; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y + 1] != null)
                {
                    grid[j, y] = grid[j, y + 1];
                    grid[j, y + 1] = null;
                    grid[j, y].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    // Add the  block to the grid
    void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if (IsInsideGrid(new Vector3(roundedX, roundedY, 0)))
            {
                grid[roundedX, roundedY] = children;
                if (roundedY >= height - 1)
                {
                    battleManager.TimeStop = true;
                    GameObject gameInstance = GameObject.Find("GameInstance");
                    if (gameInstance != null) { gameInstance.GetComponent<TwoDto3D>().TwoDGameOver(); }
                    return;
                }
            }
        }
        CheckOnLand();
        selectionToolProcessor.GetComponent<SelectionTool>().stillFalling = false;
        if (ghostBlock != null)
        {
            Destroy(ghostBlock);
        }
        //FindObjectOfType<SpawnBlock>().SpawnNewBlock(1);
    }

    public void CheckOnLand()
    {
        battleManager.CheckOnLand(color);
    }


    // Check if the current move is valid
    bool ValidMove()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
                return false;
        }
        return true;
    }

    // Check if a position is inside the main grid
    public bool IsInsideGrid(Vector3 position)
    {
        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
    }

    // Check if a position is inside the extended grid
    public bool IsInsideExtendedGrid(Vector3 position)
    {
        return position.x >= 0 && position.x < extendedWidth && position.y >= 0 && position.y < height;
    }


    public void DropDown()
    {
        while (true)
        {
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLines();
                this.enabled = false;
                break;
            }
        }
    }

    public string GetBlockColor()
    {
        foreach (Transform child in transform)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                return ColorUtility.ToHtmlStringRGBA(renderer.material.color);
            }
        }
        Debug.LogWarning("Renderer component not found on any of the block's children.");
        return "FFFFFF"; 
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
        GameObject thebigblock = this.gameObject;
        foreach (Transform child in thebigblock.transform)
        {
            if (child.localPosition == localPosition)
            {
                GameObject newSpriteObject = new GameObject(spriteName);
                newSpriteObject.transform.parent = child;
                newSpriteObject.transform.localPosition = Vector3.zero;
                SpriteRenderer spriteRenderer = newSpriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = GetSpriteByName(spriteName);
                Debug.Log("Parent GameObject: " + newSpriteObject.transform.parent.name);
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

}