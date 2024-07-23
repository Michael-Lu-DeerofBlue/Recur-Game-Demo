using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Fungus;


public class Board : MonoBehaviour
{
    public GameObject levelController;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Tilemap notMoveMap;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Tile FillTiles;
    private string saveFilePath;

    public float offset;
    public List<Vector3> exisitingBlocks; //用来在生成的时候存目前现在里面有的Block，但是看的是MainBlock里面的坐标，这样方便我们在Exit的时候只生成那些让cube出来的坐标
    public List<Vector3> outputBlocksForBridge; //用来给搭建的脚本知道我要在哪里搭上Cubes

    private Dictionary<string, TileBase> tileDictionary;

    public float xOffset;
    public float yOffset;
    public float zOffset;
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
        saveFilePath = Path.Combine(Application.persistentDataPath, "tiledata.json");
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveTileMap();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Exit(false);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadMap();
        }
    }
    [System.Serializable]
    public class TileDataContainer
    {
        public List<Vector3> positions;

        public TileDataContainer(List<Vector3> tilePositions)
        {
            positions = tilePositions;
        }
    }

    public void SaveTileMap()
    {
        List<Vector3> tilePositions = new List<Vector3>();
        Clear(activePiece); //这个好像没有clear掉
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                notMoveMap.gameObject.SetActive(true);
                Vector3 worldPos = tilemap.CellToWorld(pos);
                notMoveMap.SetTile(notMoveMap.WorldToCell(worldPos), tilemap.GetTile(pos));
                tilePositions.Add(worldPos);
                //Debug.Log("the saved worldPoses are: " + worldPos);
            }
        }

        notMoveMap.gameObject.SetActive(false);
        string json = JsonUtility.ToJson(new TileDataContainer(tilePositions));
        File.WriteAllText(saveFilePath, json);
    }

    // Load the tilemap data
    public void LoadMap()
    {
        if (!Level3.firstAccess)
        {
            //Debug.Log("here");

            notMoveMap.gameObject.SetActive(true);
            tilemap.ClearAllTiles();
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                TileDataContainer tileDataContainer = JsonUtility.FromJson<TileDataContainer>(json);

                foreach (var position in tileDataContainer.positions)
                {
                    Vector3Int cellPos = notMoveMap.WorldToCell(position);
                    //Debug.Log("the reloaded cellPos is:" + cellPos);
                    notMoveMap.SetTile(cellPos, FillTiles);
                }
            }
        }

        // Call MergeTilemaps() after loading
        MergeTilemaps();
    }

    private void Exit(bool filled)
    {
        List<Vector3Int> activePieceTilePos = GetActivePieceTilePosition(activePiece);
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Vector3 worldPos = tilemap.CellToWorld(pos);
                worldPos = new Vector3(worldPos.x + xOffset, worldPos.y + yOffset, worldPos.z + zOffset);
                if (filled)//是被迫填满的情况
                {
                    if (!exisitingBlocks.Contains(pos))
                    {
                         //Debug.Log("added to the output is:" + pos);
                        outputBlocksForBridge.Add(worldPos);
                    }
                    else
                    {
                        //Debug.Log("excluded from the output is:" + pos);
                    }
                }
                else //没被迫逼满的情况
                {
                    if (!exisitingBlocks.Contains(pos) && !activePieceTilePos.Contains(pos))
                    {
                        outputBlocksForBridge.Add(worldPos); //将新增的block的worldpos导出，这样我们就可以造桥了
                    }
                }

            }
        }
        if (filled)
        {
            foreach (Vector3Int pos in activePieceTilePos)
            {
                Vector3 worldPos = tilemap.CellToWorld(pos);
                worldPos = new Vector3(worldPos.x + xOffset, worldPos.y + yOffset, worldPos.z +zOffset);
                outputBlocksForBridge.Add(worldPos);
            }
        }

        activePiece.stopped = true;
        Clear(activePiece);
        levelController.GetComponent<Level3>().BridgeCubePositions = outputBlocksForBridge;
        levelController.GetComponent<Level3>().ExitBoard();
    }

    private void OnEnable()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        setMyPositionForPlayer();
        setSpawnPosition();
        SpawnPiece();
    }

    void MergeTilemaps()
    {
        // 获取NotMoveMap的所有Tile的位置
        BoundsInt bounds = notMoveMap.cellBounds;
        tilemap.ClearAllTiles();
        // 遍历所有位置并将Tile复制到mainTilemap，同时确保世界位置不变
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                TileBase tile = notMoveMap.GetTile(localPlace);
                if (tile != null)
                {
                    // 获取世界坐标
                    Vector3 worldPosition = notMoveMap.CellToWorld(localPlace);
                    worldPosition = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
                    //Debug.Log("the local positon in not move map is: "+ localPlace);
                    //Debug.Log("the world positon in not move map is: "+ worldPosition);
                    // 将世界坐标转换为mainTilemap的单元格坐标
                    Vector3Int mainTilemapPosition = tilemap.WorldToCell(worldPosition);
                   //Debug.Log("the positon in main is: "+mainTilemapPosition);
                    exisitingBlocks.Add(mainTilemapPosition);
                    // 在mainTilemap的正确位置设置Tile
                    mainTilemapPosition.y = mainTilemapPosition.y + 1; //这好像是一个bug的来源
                    tilemap.SetTile(mainTilemapPosition, tile);
                }
            }
        }
        notMoveMap.gameObject.SetActive(false);
        Level3.firstAccess = false;
    }
    public void setSpawnPosition()
    {
        Vector3 playerPosition = levelController.GetComponent<Level3>().GrabNearestMarkerLocation();
        int playerXIntRound = Mathf.RoundToInt(-playerPosition.x / 3.5f);
        spawnPosition.x = playerXIntRound;
    }

    public void setMyPositionForPlayer()
    {
        Vector3 playerPosition = levelController.GetComponent<Level3>().GrabNearestMarkerLocation();
        float playerZ = playerPosition.z;
        Vector3 newPosition = transform.position;
        newPosition.z = Mathf.RoundToInt(playerZ + offset);
        tilemap.transform.position = newPosition;
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);
        //Debug.Log(spawnPosition.y);
        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public List<Vector3Int> GetActivePieceTilePosition(Piece piece)
    {
        List<Vector3Int> activePieceTilePosition = new List<Vector3Int>();
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            activePieceTilePosition.Add(tilePosition);
        }
        return activePieceTilePosition;
    }

    public void GameOver()
    {
        Exit(true);
        // Do anything else you want on game over here..
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;
        //Debug.Log(bounds);
        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
            //Debug.Log(tilePosition);
            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition))
            {
                //Debug.Log(tilePosition);
                return false;
            }
        }

        return true;
    }


}
