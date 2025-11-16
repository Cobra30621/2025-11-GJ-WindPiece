using System.Collections;
using System.Collections.Generic;
using Core.Audio;
using Core.Board;
using Core.GameFlow;
using Core.Pieces;
using Core.Utils;
using UnityEngine;

namespace Core.Wind
{
    public class PieceAnimator : MonoBehaviour
    {
        public Transform tileOrigin;
        private BoardManager board => BoardManager.Instance;

        public Vector3 CellToWorld(Vector2Int p)
        {
            return board.GridToWorld(p);
        }

        /// <summary>
        /// 接受 MovementEvent 並播放每個事件內的 moves
        /// </summary>
        public IEnumerator PlayMoves(List<MovementEvent> events, float durationPerMove = 0.12f)
        {
            foreach (var ev in events)
            {
                if (ev.moves == null) continue;
                SFXManager.Instance.PlaySFX(SFXType.Wind);
                
                yield return new WaitForSeconds(0.2f);

                foreach (var m in ev.moves)
                {
                    if (m.piece == null) continue;

                    Vector3 from = CellToWorld(m.from);
                    Vector3 to = CellToWorld(m.to);
                    float t = 0;

                    SFXManager.Instance.PlaySFX(SFXType.Move);

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
                        GameEventBus.OnPieceRemoved?.Invoke(m.piece);
                        Object.Destroy(m.piece.gameObject);
                        BoardManager.Instance.RemovePiece(m.piece);

                        if (m.piece is EnemyPiece)
                        {
                            SFXManager.Instance.PlaySFX(SFXType.EnemyDeath);
                        }
                    }

                    yield return null;
                }

                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}