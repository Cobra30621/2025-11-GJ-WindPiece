using System.Collections.Generic;
using Core.Input;
using Core.Pieces;
using Core.Stage;
using Core.Utils;
using Game.Core.Pieces;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Main
{
    public class PieceSelectorUI : MonoBehaviour
    {
        public static PieceSelectorUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Button buttonPrefab;
        // [SerializeField] private Button deselectButton;

        [SerializeField]
        [LabelText("所有的棋子(給 debug 用)")]
        private List<PieceConfig> allPieceList;
        
        
        [ShowInInspector]
        private List<PieceUsageConfig> pieceUsageList;

        private List<PieceButtonSlot> buttonSlots = new List<PieceButtonSlot>();

        private class PieceButtonSlot
        {
            public PieceConfig config;
            public Button button;
        }

        [ShowInInspector]
        private PieceButtonSlot currentSelectedSlot = null;

        public bool IsPieceEmpty => buttonSlots.Count == 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            pieceUsageList = StageManager.Instance.currentStageInstance.pieceUsageList;

            GenerateButtonsFromUsageList(pieceUsageList);

  

            GameEventBus.OnPiecePlaced += OnPieceUsed;
        }

        #region 生成按鈕

        private void GenerateButtonsFromUsageList(List<PieceUsageConfig> usageList)
        {
            foreach (var usage in usageList)
            {
                for (int i = 0; i < usage.count; i++)
                {
                    CreateButtonSlot(usage.config);
                }
            }
        }

        /// <summary>
        /// 核心按鈕生成方法
        /// </summary>
        private void CreateButtonSlot(PieceConfig config)
        {
            var btn = Instantiate(buttonPrefab, buttonContainer);

            // 設定圖片與名稱
            var image = btn.transform.Find("image")?.GetComponent<Image>();
            if (image != null) image.sprite = config.image;

            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = config.pieceName;
            
            // 設定 Tool Tip
            var tooltipTrigger = btn.GetComponent<TooltipTrigger>();
            if (tooltipTrigger != null) tooltipTrigger.message = config.description;

            var slot = new PieceButtonSlot
            {
                config = config,
                button = btn
            };

            buttonSlots.Add(slot);

            btn.onClick.AddListener(() => OnClickedSlot(slot));
        }

        #endregion

        #region 外部新增按鈕方法

        /// <summary>
        /// 將指定的 PieceConfig List 全部加入 buttonSlots
        /// </summary>
        [Button]
        public void AddAllPiece()
        {
            var pieces = allPieceList;
            if (pieces == null || pieces.Count == 0) return;

            foreach (var piece in pieces)
            {
                CreateButtonSlot(piece);
            }
        }

        #endregion

        private void OnClickedSlot(PieceButtonSlot slot)
        {
            if (!PieceSelectionManager.Instance.CanSelectPiece()) return;

            currentSelectedSlot = slot;

            // 通知 SelectionManager
            PieceSelectionManager.Instance.SelectPiece(slot.config);
            HighlightSelectedButton();
        }

        private void HighlightSelectedButton()
        {
            foreach (var slot in buttonSlots)
            {
                if (slot.button == null) continue;                // null 保護
                if (slot.button.Equals(null)) continue;          // Unity Destroy 保護 (必加)
                
                var colors = slot.button.colors;
                colors.normalColor = (slot == currentSelectedSlot) ? Color.green : Color.white;
                slot.button.colors = colors;
            }
        }

        private void DeselectCurrentButton()
        {
            currentSelectedSlot = null;
            HighlightSelectedButton();
        }

        private void OnPieceUsed(Piece piece)
        {
            if (piece == null) return;
            if (currentSelectedSlot == null) return;
            if (currentSelectedSlot.config != piece.Config) return;

            // 1. 拿出按鈕引用
            var btn = currentSelectedSlot.button;

            // 2. 從列表移除
            buttonSlots.Remove(currentSelectedSlot);

            // 3.馬上清空引用，避免後面被 iterate
            currentSelectedSlot.button = null;

            // 4. Destroy 按鈕
            Destroy(btn.gameObject);

            currentSelectedSlot = null;
            PieceSelectionManager.Instance.DeselectPiece();
        }
    }
}
