using System;
using Game.Core.Pieces;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Input
{
    public class PieceSelectionManager : MonoBehaviour
    {
        public static PieceSelectionManager Instance { get; private set; }
        
        public PieceConfig SelectedPiece { get; private set; }
        public event Action<PieceConfig> OnSelectionChanged;
        

        public Camera mainCamera;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SelectPiece(PieceConfig config)
        {
            SelectedPiece = config;
            OnSelectionChanged?.Invoke(SelectedPiece);
            Debug.Log($"Selected piece: {config.pieceName}");

            CreatePreview();
        }

        public void DeselectPiece()
        {
            SelectedPiece = null;
            OnSelectionChanged?.Invoke(SelectedPiece);
            Debug.Log("Deselected piece");

            DestroyPreview();
        }

        private void CreatePreview()
        {
            DestroyPreview(); // 保證不會重複

            PlacementPreviewManager.Instance.StartPreview(SelectedPiece);
        }

        public void DestroyPreview()
        {
            PlacementPreviewManager.Instance.StopPreview();
        }
    }
}