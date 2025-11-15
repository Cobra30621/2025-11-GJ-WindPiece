using System.Collections;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace Core.Wind
{
    public class PieceAnimator : MonoBehaviour
    {
        // World position conversion helper
        public Transform tileOrigin;
        public Vector3 CellToWorld(Vector2Int p)
        {
            // 假設每格 1 單位
            return new Vector3(p.x, p.y, 0) + (tileOrigin ? tileOrigin.position : Vector3.zero);
        }

        // 播放 move list（每個 move 會做平滑插值）
        public IEnumerator PlayMoves(List<PieceMoveResult> moves, float durationPerMove = 0.12f)
        {
            foreach (var m in moves)
            {
                if (m.piece == null) continue;
                Vector3 from = CellToWorld(m.from);
                Vector3 to = CellToWorld(m.to);
                float t = 0;
                while (t < durationPerMove)
                {
                    t += Time.deltaTime;
                    float f = Mathf.Clamp01(t / durationPerMove);
                    m.piece.transform.position = Vector3.Lerp(from, to, f);
                    yield return null;
                }
                m.piece.MoveToWorld(to);

                if (m.isFalling)
                {
                    // 簡單處理：destroy object
                    GameEventBus.OnPieceRemoved?.Invoke(m.piece);
                    Object.Destroy(m.piece.gameObject);
                }

                yield return null;
            }
        }
    }
}