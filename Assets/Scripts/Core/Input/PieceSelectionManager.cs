using System;
using Game.Core.Pieces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Input
{
    public enum InputLockMode
    {
        Unlock,         // 完全可操作
        LockPlacement,  // 可以選，但不能下
        LockAll         // 不能選也不能下
    }

    public class PieceSelectionManager : MonoBehaviour
    {
        public static PieceSelectionManager Instance { get; private set; }
        
        public PieceConfig SelectedPiece { get; private set; }
        public event Action<PieceConfig> OnSelectionChanged;


        /// <summary>
        /// 玩家操作鎖定模式
        /// </summary>
        [ShowInInspector]
        public InputLockMode LockMode { get; private set; } = InputLockMode.Unlock;

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
        /// 設定玩家操作鎖定模式
        /// </summary>
        public void SetLockMode(InputLockMode mode)
        {
            LockMode = mode;

            // 如果完全鎖定且當前有選中棋子 → 取消選擇
            if (mode == InputLockMode.LockAll && SelectedPiece != null)
            {
                DeselectPiece(force: true);
            }
        }

        public void SelectPiece(PieceConfig config)
        {
            // LockAll: 不能選
            if (LockMode == InputLockMode.LockAll)
            {
                Debug.Log("Input locked: cannot select piece.");
                return;
            }

            SelectedPiece = config;
            OnSelectionChanged?.Invoke(SelectedPiece);
            Debug.Log($"Selected piece: {config.pieceName}");

            CreatePreview();
        }

        public void DeselectPiece(bool force = false)
        {
            // LockAll: 不可手動取消選擇（除非強制）
            if (!force && LockMode == InputLockMode.LockAll)
            {
                Debug.Log("Input locked: cannot deselect piece.");
                return;
            }

            SelectedPiece = null;
            OnSelectionChanged?.Invoke(null);
            Debug.Log("Deselected piece");

            DestroyPreview();
        }

        private void CreatePreview()
        {
            if (SelectedPiece == null)
                return;

            // LockAll: 不能下，也不能預覽
            // LockPlacement: 不能下，但「可以預覽」你可自行選擇是否要禁止預覽
            if (LockMode == InputLockMode.LockAll)
                return;

            DestroyPreview();
            PlacementPreviewManager.Instance.StartPreview(SelectedPiece);
        }

        public void DestroyPreview()
        {
            PlacementPreviewManager.Instance.StopPreview();
        }

        public bool CanSelectPiece()
        {
            return LockMode != InputLockMode.LockAll;
        }
        
        /// <summary>
        /// 放置棋子前可呼叫這個方法檢查是否允許放置
        /// </summary>
        public bool CanPlacePiece()
        {
            return LockMode == InputLockMode.Unlock;
        }
    }
}
