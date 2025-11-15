using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Prefab Data", menuName = "Tile Prefab Data")]
public class TilePrefabData : ScriptableObject
{
    public List<TilePrefabPair> TilePrefabPairs;
}
