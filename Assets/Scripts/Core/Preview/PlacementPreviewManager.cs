using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Core.Board;
using Core.Input;
using Core.Pieces;
using Core.Utils;
using Game.Core.Pieces;

public class PlacementPreviewManager : MonoBehaviour
{
    public static PlacementPreviewManager Instance { get; private set; }

    [Header("Tilemaps")]
    public Tilemap overlayTilemap;  // 是否可放置
    public Tilemap effectTilemap;   // 棋子效果範圍
    public TileBase canPlaceTile;
    public TileBase cannotPlaceTile;
    public TileBase windTile;

    [Header("Preview Piece")]
    public GameObject previewPieceRoot;
    public float previewAlpha = 0.5f;

    private PieceConfig currentConfig;
    private BoardManager Board => BoardManager.Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        previewPieceRoot.SetActive(false);
    }

    private void Update()
    {
        if (currentConfig == null)
            return;

        if (PieceSelectionManager.Instance.LockMode == InputLockMode.Unlock)
        {
            UpdateHoverPreview();
        }
    }

    // ======================================================
    // API
    // ======================================================
    public void StartPreview(PieceConfig config)
    {
        currentConfig = config;

        previewPieceRoot.SetActive(true);
        ApplyPreviewSprite(config);
        ApplyPreviewAlpha();
    }

    public void StopPreview()
    {
        currentConfig = null;

        previewPieceRoot.SetActive(false);
        overlayTilemap.ClearAllTiles();
        effectTilemap.ClearAllTiles();
    }

    // ======================================================
    // Hover + Tile 顯示
    // ======================================================
    private void UpdateHoverPreview()
    {
        if (!TryGetHoverCell(out Vector2Int pos))
        {
            previewPieceRoot.SetActive(false);
            return;
        }

        previewPieceRoot.SetActive(true);
        previewPieceRoot.transform.position = Board.GridToWorld(pos);

        UpdatePlacementTiles();
        UpdateEffectRangeTiles(pos);
    }

    // ======================================================
    // 顯示可放置 / 不可放置 TileMap
    // ======================================================
    private void UpdatePlacementTiles()
    {
        overlayTilemap.ClearAllTiles();

        foreach (TileCell cell in Board.AllCells())
        {
            var tilePos = new Vector3Int(cell.Pos.x, cell.Pos.y, 0);
            var tile = cell.CanAddPiece() ? canPlaceTile : cannotPlaceTile;
            // 如果鎖定中，無法放置
            if (!PieceSelectionManager.Instance.CanPlacePiece())
            {
                tile = cannotPlaceTile;
            }
            
            overlayTilemap.SetTile(tilePos, tile);
        }
    }

    // ======================================================
    // 顯示棋子風向範圍
    // ======================================================
    private void UpdateEffectRangeTiles(Vector2Int origin)
    {
        effectTilemap.ClearAllTiles();

        Vector2Int dir = UtilsTool.DirectionToVector2Int(currentConfig.windDirection); // 假設 PieceConfig 有方向

        foreach (TileCell cell in Board.AllCells())
        {
            // 排除 origin
            if (cell.Pos == origin)
                continue;
            
            if (IsInDirectionRange(cell.Pos, origin, dir))
            {
                var tilePos = new Vector3Int(cell.Pos.x, cell.Pos.y, 0);
                effectTilemap.SetTile(tilePos, windTile);
            }
        }
    }

    // 判斷某格子是否在方向作用範圍內
    private bool IsInDirectionRange(Vector2Int pos, Vector2Int origin, Vector2Int dir)
    {
        if (dir == Vector2Int.right) return pos.x >= origin.x;
        if (dir == Vector2Int.left)  return pos.x <= origin.x;
        if (dir == Vector2Int.up)    return pos.y >= origin.y;
        if (dir == Vector2Int.down)  return pos.y <= origin.y;
        return false;
    }

    // ======================================================
    // 工具：滑鼠轉格子
    // ======================================================
    private bool TryGetHoverCell(out Vector2Int gridPos)
    {
        gridPos = default;

        if (Camera.main == null)
            return false;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector3 world = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z)
        );

        // 修正 pivot 偏移
        world.z = 0;
        Vector3 adjustPos = world - new Vector3(0.5f, 0.5f, 0);

        return Board.TryWorldToGrid(adjustPos, out gridPos) &&
               Board.IsInside(gridPos);
    }

    // ======================================================
    // 工具：設定透明棋子
    // ======================================================
    private void ApplyPreviewSprite(PieceConfig config)
    {
        var sr = previewPieceRoot.GetComponent<SpriteRenderer>();
        sr.sprite = config.image;
    }

    private void ApplyPreviewAlpha()
    {
        foreach (var sr in previewPieceRoot.GetComponentsInChildren<SpriteRenderer>())
        {
            var c = sr.color;
            c.a = previewAlpha;
            sr.color = c;
        }
    }
}
