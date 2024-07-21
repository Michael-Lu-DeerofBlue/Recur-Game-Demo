using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public GameObject playerLocation;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Tile FillTiles;
    private string saveFilePath;
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

        for (int i = 0; i < tetrominoes.Length; i++) {
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



private void Start()
    {
        setMyPositionForPlayer();
        setSpawnPosition();
        SpawnPiece();
    }
    public void setSpawnPosition()
    {
        Vector3 playerPosition = playerLocation.transform.position;
        int playerXIntRound = Mathf.RoundToInt(playerPosition.x);
        spawnPosition.x = playerXIntRound;

    }
    public void setMyPositionForPlayer()
    {
        Vector3 playerPosition = playerLocation.transform.position;
        float playerZ = playerPosition.z;
        Vector3 newPosition = transform.position;
        newPosition.z = playerZ - 10;
        transform.position = newPosition;
    }
    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition)) {
            Set(activePiece);
        } else {
            GameOver();
        }
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();

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
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }





}
