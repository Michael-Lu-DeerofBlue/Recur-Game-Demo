using UnityEngine;
using UnityEngine.Tilemaps;

public enum BridBolck
{
    I, J, L, O, S, T, Z
}

[System.Serializable]
public struct TetrominoData
{
    public Tile tile;
    public BridBolck BridgeBlock;

    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }

    public void Initialize()
    {
        cells = Data.Cells[BridgeBlock];
        wallKicks = Data.WallKicks[BridgeBlock];
    }

}
