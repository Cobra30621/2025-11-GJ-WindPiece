using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Main;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Stage
{
    public class Stage : MonoBehaviour
    {
        [LabelText("本關卡可以使用的棋子")]
        public List<PieceUsageConfig> pieceUsageList;

        
        public Tilemap boardTilemap;
        public Tilemap obstacleTilemap;

        public TilemapSpawner TilemapSpawner;
    }
}