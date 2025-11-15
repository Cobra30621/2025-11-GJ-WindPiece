using System.Collections.Generic;
using UnityEngine;

namespace Core.Board
{
    [CreateAssetMenu(fileName = "Tile Type Data", menuName = "Tile Type Data")]
    public class TileTypeData : ScriptableObject
    {
        public List<TileTypePair> TileTypePairs;
    }
}