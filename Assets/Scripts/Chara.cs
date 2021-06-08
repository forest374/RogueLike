using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chara : MonoBehaviour
{
    public abstract string Name { get; set; }
    public abstract Maps Map { get; }
    public abstract Manager Manager { get; }

    /// <summary>
    /// 移動先を受け取り移動できるならtrueを返す
    /// </summary>
    /// <param name="point">座標</param>
    /// <returns>可否</returns>
    protected virtual bool MoveingCheck(Vector2Int point)
    {
        Tile tile = Map.TileStateCheck(point);
        if (tile == Tile.road || tile == Tile.room)
        {
            return true;
        }
        return false;
    }
}
