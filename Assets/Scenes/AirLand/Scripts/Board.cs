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
    public List<Vector3> exisitingBlocks; //���������ɵ�ʱ���Ŀǰ���������е�Block�����ǿ�����MainBlock��������꣬��������������Exit��ʱ��ֻ������Щ��cube����������
    public List<Vector3> outputBlocksForBridge; //��������Ľű�֪����Ҫ���������Cubes

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
        Clear(activePiece); //�������û��clear��
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
                if (filled)//�Ǳ������������
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
                else //û���ȱ��������
                {
                    if (!exisitingBlocks.Contains(pos) && !activePieceTilePos.Contains(pos))
                    {
                        outputBlocksForBridge.Add(worldPos); //��������block��worldpos�������������ǾͿ���������
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
        // ��ȡNotMoveMap������Tile��λ��
        BoundsInt bounds = notMoveMap.cellBounds;
        tilemap.ClearAllTiles();
        // ��������λ�ò���Tile���Ƶ�mainTilemap��ͬʱȷ������λ�ò���
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                TileBase tile = notMoveMap.GetTile(localPlace);
                if (tile != null)
                {
                    // ��ȡ��������
                    Vector3 worldPosition = notMoveMap.CellToWorld(localPlace);
                    worldPosition = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
                    //Debug.Log("the local positon in not move map is: "+ localPlace);
                    //Debug.Log("the world positon in not move map is: "+ worldPosition);
                    // ����������ת��ΪmainTilemap�ĵ�Ԫ������
                    Vector3Int mainTilemapPosition = tilemap.WorldToCell(worldPosition);
                   //Debug.Log("the positon in main is: "+mainTilemapPosition);
                    exisitingBlocks.Add(mainTilemapPosition);
                    // ��mainTilemap����ȷλ������Tile
                    mainTilemapPosition.y = mainTilemapPosition.y + 1; //�������һ��bug����Դ
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
