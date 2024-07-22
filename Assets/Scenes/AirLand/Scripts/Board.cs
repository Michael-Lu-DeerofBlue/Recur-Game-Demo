using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Fungus;

[DefaultExecutionOrder(-1)]
[System.Serializable]
public class TileDataContainer
{
    public List<TileData> tiles;
}

[System.Serializable]
public class TileData
{
    public Vector3Int position;
}

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
        // LoadTiles();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveTiles();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Exit(false);
        }
    }

    private void Exit(bool filled) {
        List<Vector3> activePieceTilePos = GetActivePieceTilePosition(activePiece); 
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                
                Vector3 worldPos = tilemap.CellToWorld(pos);
                worldPos = new Vector3(worldPos.x-3.57f, worldPos.y, worldPos.z+ 3.57f-5.76f);
                if (filled)//�Ǳ������������
                {
                    if (!exisitingBlocks.Contains(pos))
                    {
                        outputBlocksForBridge.Add(worldPos);
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
        activePiece.stopped = true;
        Clear(activePiece);
        levelController.GetComponent<Level3>().BridgeCubePositions = outputBlocksForBridge;
        levelController.GetComponent<Level3>().ExitBoard();
    }

    private void SaveTiles()
    {
        List<TileData> tileDataList = new List<TileData>();

        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                TileData tileData = new TileData { position = pos };
                tileDataList.Add(tileData);
            }
        }

        string json = JsonUtility.ToJson(new TileDataContainer { tiles = tileDataList }, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Tiles saved.");
    }

    private void LoadTiles()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            TileDataContainer tileDataContainer = JsonUtility.FromJson<TileDataContainer>(json);

            foreach (TileData tileData in tileDataContainer.tiles)
            {
                tilemap.SetTile(tileData.position, FillTiles);
            }

            Debug.Log("Tiles loaded.");
        }
    }

    private void OnEnable()
    {
        setMyPositionForPlayer();
        setSpawnPosition();
        MergeTilemaps();
        SpawnPiece();
    }
    void MergeTilemaps()
    {
        // ��ȡNotMoveMap������Tile��λ��
        BoundsInt bounds = notMoveMap.cellBounds;
        TileBase[] allTiles = notMoveMap.GetTilesBlock(bounds);

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
                    // ����������ת��ΪmainTilemap�ĵ�Ԫ������
                    Vector3Int mainTilemapPosition = tilemap.WorldToCell(worldPosition);
                    exisitingBlocks.Add(mainTilemapPosition);
                    // ��mainTilemap����ȷλ������Tile
                    tilemap.SetTile(mainTilemapPosition, tile);
                }
            }
        }
    }
    public void setSpawnPosition()
    {
        Vector3 playerPosition = levelController.GetComponent<Level3>().GrabNearestMarkerLocation();
        int playerXIntRound = Mathf.RoundToInt(playerPosition.x);
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

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public List<Vector3> GetActivePieceTilePosition(Piece piece)
    {
        List<Vector3> activePieceTilePosition = new List<Vector3>();
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

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

}
