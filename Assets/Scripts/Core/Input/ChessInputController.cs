using Core.GameFlow;
using Core.Input;
using Game.Core.Pieces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ChessInputController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    public Tilemap boardTilemap;
    
    public static ChessInputController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        HandleHover();
        HandleClick();
    }

    private void HandleHover()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 world = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        world.z = 0;
        Vector3Int cell = boardTilemap.WorldToCell(world);

        if (!boardTilemap.HasTile(cell)) return;

        // 可加入 WindSimulation 預覽
        // GameManager.Instance.PreviewPlacement(cell);
    }
    
    private void HandleClick()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 world = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        world.z = 0;
        Vector3Int cell = boardTilemap.WorldToCell(world);
        if (!boardTilemap.HasTile(cell)) return;

        var selectedPiece = PieceSelectionManager.Instance.SelectedPiece;
        if (selectedPiece == null) return;

        bool success = GameManager.Instance.PlacePiece(selectedPiece, new Vector2Int(cell.x, cell.y));

        if (success)
        {
            // 下成功 → 刪除 preview & 取消選擇
            PieceSelectionManager.Instance.DestroyPreview();
            PieceSelectionManager.Instance.DeselectPiece();
        }
    }
}