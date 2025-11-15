using System;
using Game.Core.Pieces;
using UnityEngine;

namespace Core.Input
{
    public class PieceSelectionManager : MonoBehaviour
    {
        public static PieceSelectionManager Instance { get; private set; }

        public PieceConfig SelectedPiece { get; private set; }
        public event Action<PieceConfig> OnSelectionChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// 選擇一個棋子
        /// </summary>
        public void SelectPiece(PieceConfig config)
        {
            SelectedPiece = config;
            OnSelectionChanged?.Invoke(SelectedPiece);
            Debug.Log($"Selected piece: {config.pieceName}");
        }

        /// <summary>
        /// 取消選擇
        /// </summary>
        public void DeselectPiece()
        {
            SelectedPiece = null;
            OnSelectionChanged?.Invoke(SelectedPiece);
            Debug.Log("Deselected piece");
        }
    }
}