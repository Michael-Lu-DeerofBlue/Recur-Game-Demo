using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Fungus;


public class Board : MonoBehaviour
{
    public GameObject levelController;
    public Camera BlockGameCamera;
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
    public Vector2Int TopRightCornerPos;
    private Dictionary<string, TileBase> tileDictionary;

    public float xOffset;
    public float yOffset;
    public float zOffset;
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(boardSize.x / 2, spawnPosition.y+3);
            TopRightCornerPos = position;
            SetCameraPosition();
            return new RectInt(position, -boardSize);

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
            ClearBlock(activePiece);
            SpawnPiece();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Exit(false);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
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


    // Load the tilemap data


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

        ClearBlock(activePiece);
        levelController.GetComponent<Level3>().BridgeCubePositions = outputBlocksForBridge;
        levelController.GetComponent<Level3>().ExitBoard();
    }

    private void OnEnable()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        setSpawnPosition();
       SetCameraPosition();
        SpawnPiece();
    }

    public void setSpawnPosition()
    {
       spawnPosition= levelController.GetComponent<Level3>().GrabNearestMarker();

    }

    public void SetCameraPosition()
    {
        Vector3 localPosition = BlockGameCamera.transform.localPosition;
        localPosition.y = spawnPosition.y - 8;
        BlockGameCamera.transform.localPosition = localPosition;
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);
        activePiece.Stopped = false;
        activePiece.cleared = false;
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


    public void ClearBlock(Piece piece)
    {
        piece.Stopped = true;
        piece.cleared = true;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }


    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
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
