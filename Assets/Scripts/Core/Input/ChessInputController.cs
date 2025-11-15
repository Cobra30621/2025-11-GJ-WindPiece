using Core.GameFlow;
using Game.Core.Pieces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ChessInputController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap boardTilemap;

    [SerializeField]
    private PieceConfig selectedPieceConfig;

    [Button]
    public void SetSelectedPiece(PieceConfig config)
    {
        selectedPieceConfig = config;
    }
    
    private void Update()
    {
        HandleHover();
        HandleClick();
    }

    /// <summary>
    /// 滑鼠移動：更新預覽
    /// </summary>
    private void HandleHover()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 world = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        world.z = 0;

        Vector3Int cell = boardTilemap.WorldToCell(world);

        
        if (!boardTilemap.HasTile(cell))
            return;

        // gameManager.PreviewPlacement(cell);
    }

    /// <summary>
    /// 滑鼠左鍵：下棋
    /// </summary>
    private void HandleClick()
    {
        if (Mouse.current == null) return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 world = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        world.z = 0;

        Vector3Int cell = boardTilemap.WorldToCell(world);

        if (!boardTilemap.HasTile(cell))
            return;

        Debug.Log($"click: world {world}, cell: {cell}");
        
        GameManager.Instance.PlacePiece(selectedPieceConfig, new Vector2Int(cell.x, cell.y));
    }
}