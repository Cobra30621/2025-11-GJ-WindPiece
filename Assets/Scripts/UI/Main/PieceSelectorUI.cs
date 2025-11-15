using System.Collections.Generic;
using Core.Input;
using Game.Core.Pieces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Main
{
    public class PieceSelectorUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private Button deselectButton; // 取消選擇按鈕

        [Header("Game Config")]
        [SerializeField] private List<PieceConfig> pieceConfigs;

        private Dictionary<PieceConfig, Button> buttonMap = new Dictionary<PieceConfig, Button>();

        private void Awake()
        {
            GenerateButtons();

            if (deselectButton != null)
            {
                deselectButton.onClick.AddListener(() =>
                {
                    PieceSelectionManager.Instance.DeselectPiece();
                });
            }

            
        }

        void Start()
        {
            // 訂閱選擇變更事件
            PieceSelectionManager.Instance.OnSelectionChanged += UpdateButtonHighlights;
        }

        private void GenerateButtons()
        {
            if (buttonPrefab == null || buttonContainer == null) return;

            foreach (var config in pieceConfigs)
            {
                var btn = Instantiate(buttonPrefab, buttonContainer);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = config.pieceName;
                btn.transform.Find("image").GetComponentInChildren<Image>().sprite = config.image;
                btn.onClick.AddListener(() => PieceSelectionManager.Instance.SelectPiece(config));
                buttonMap[config] = btn;
            }
        }

        private void UpdateButtonHighlights(PieceConfig selected)
        {
            foreach (var kv in buttonMap)
            {
                var colors = kv.Value.colors;
                if (kv.Key == selected)
                    colors.normalColor = Color.green; // 高亮
                else
                    colors.normalColor = Color.white;
                kv.Value.colors = colors;
            }
        }
    }
}