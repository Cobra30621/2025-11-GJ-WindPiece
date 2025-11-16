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
        [SerializeField] private Button deselectButton;

        [ShowInInspector]
        private List<PieceUsageConfig> pieceUsageList;

        /// <summary> 存放所有按鈕（1 config 對應多個按鈕） </summary>
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
            
            GenerateButtons();

            if (deselectButton != null)
            {
                deselectButton.onClick.AddListener(() =>
                {
                    DeselectCurrentButton();
                    PieceSelectionManager.Instance.DeselectPiece();
                });
            }
            
            GameEventBus.OnPiecePlaced += OnPieceUsed;
        }

        private void GenerateButtons()
        {
            foreach (var usage in pieceUsageList)
            {
                for (int i = 0; i < usage.count; i++)
                {
                    var btn = Instantiate(buttonPrefab, buttonContainer);

                    btn.transform.Find("image")
                        .GetComponent<Image>().sprite = usage.config.image;

                    btn.GetComponentInChildren<TextMeshProUGUI>().text = usage.config.pieceName;

                    var slot = new PieceButtonSlot
                    {
                        config = usage.config,
                        button = btn
                    };

                    buttonSlots.Add(slot);

                    btn.onClick.AddListener(() => OnClickedSlot(slot));
                }
            }
        }

        private void OnClickedSlot(PieceButtonSlot slot)
        {
            if(!PieceSelectionManager.Instance.CanSelectPiece()) return;
            
            currentSelectedSlot = slot;

            // 通知 SelectionManager
            PieceSelectionManager.Instance.SelectPiece(slot.config);
            HighlightSelectedButton();
        }

        private void HighlightSelectedButton()
        {
            foreach (var slot in buttonSlots)
            {
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

        /// <summary>
        /// 當棋子放下後，由 SelectionManager 發出事件
        /// </summary>
        private void OnPieceUsed(Piece piece)
        {
            if(piece == null) return;
            if (currentSelectedSlot == null) return;
            if (currentSelectedSlot.config !=piece.Config) return;

            // 移除使用掉的按鈕
            buttonSlots.Remove(currentSelectedSlot);
            Destroy(currentSelectedSlot.button.gameObject);

            currentSelectedSlot = null;
            PieceSelectionManager.Instance.DeselectPiece();
        }
    }
}
