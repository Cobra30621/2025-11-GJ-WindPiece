using UnityEngine;
using UnityEngine.Tilemaps;
using Core.Board;
using Core.Pieces;
using Game.Core.Pieces;
using UnityEngine.InputSystem;

public class PlacementPreviewManager : MonoBehaviour
{
    public static PlacementPreviewManager Instance { get; private set; }

    private BoardManager board => BoardManager.Instance;
    public Tilemap overlayTilemap;
    public TileBase canPlaceTile;
    public TileBase cannotPlaceTile;

    [Header("透明棋子")]
    public GameObject previewPieceRoot;
    public float previewAlpha = 0.5f;

    private PieceConfig currentConfig;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (currentConfig == null) return;

        UpdateHoverPreview();
    }

    // ======================================================
    // API — 開始顯示預覽（由 UI 按下某個 PieceConfig 時觸發）
    // ======================================================

    public void StartPreview(PieceConfig config)
    {
        currentConfig = config;
        previewPieceRoot.SetActive(true);
        previewPieceRoot.GetComponent<SpriteRenderer>().sprite = config.image;

        // 設透明度
        foreach (var sr in previewPieceRoot.GetComponentsInChildren<SpriteRenderer>())
        {
            var c = sr.color;
            c.a = previewAlpha;
            sr.color = c;
        }
    }

    public void StopPreview()
    {
        currentConfig = null;
        previewPieceRoot.SetActive(false);
        overlayTilemap.ClearAllTiles();
    }

    // ======================================================
    // 滑鼠追蹤 + 更新顏色顯示
    // ======================================================

    private void UpdateHoverPreview()
    {
        if (!TryGetHoverCell(out Vector2Int cellPos))
        {
            previewPieceRoot.SetActive(false);
            return;
        }
        else
        {
            previewPieceRoot.SetActive(true);
        }
        
        // 移動透明棋子
        previewPieceRoot.transform.position = board.GridToWorld(cellPos);
        
        overlayTilemap.ClearAllTiles();

        foreach (TileCell cell in board.AllCells())
        {
            Vector3Int tilePos = new Vector3Int(cell.Pos.x, cell.Pos.y, 0);

            if (cell.CanAddPiece())
            {
                overlayTilemap.SetTile(tilePos, canPlaceTile);
            }
            else
            {
                overlayTilemap.SetTile(tilePos, cannotPlaceTile);
            }
        }
    }

    // ======================================================
    // 工具：取得滑鼠 hover 位置的 Cell
    // ======================================================

    private bool TryGetHoverCell(out Vector2Int gridPos)
    {
        gridPos = default;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));

        world.z = 0;
        
        var offestPos = world - new Vector3(0.5f, 0.5f, 0);

        return board.TryWorldToGrid(offestPos, out gridPos) &&
               board.IsInside(gridPos);
    }
}
