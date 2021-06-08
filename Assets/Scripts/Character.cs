using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    //[SerializeField] TestSystem testSystem = null;
    //[SerializeField] Maps map = null;
    //Tile current = Tile.room;
    public abstract Maps Map { get; set; }
    protected abstract Tile Current { get; set; }


    protected virtual void StageStart()
    {

    }

    /// <summary>
    /// ランダムな部屋の位置に配置する
    /// </summary>
    public virtual void RandomPlacement()
    {
        Current = Tile.room;
        Vector2 point = Map.RandomRoom();
        transform.position = point;
        Map.TileStateChange(transform.position, Tile.other);
    }


    /// <summary>
    /// 移動先を受け取り移動できるなら移動する
    /// </summary>
    /// <param name="move">移動先</param>
    protected virtual void MoveCheck(Vector2Int move)
    {
        if (move.x * move.y != 0)
        {
            if (Map.Tiles[move.x, move.y] == Tile.wall || Map.Tiles[move.x, move.y] == Tile.wall)
            {
                return;
            }
        }
        Tile tileState = Map.TileStateCheck(move);
        if (tileState == Tile.road || tileState == Tile.room)
        {
            Map.TileStateChange(transform.position, Current);
            Map.TileStateChange(move, Tile.other);

            Current = tileState;
            Vector2 dir = move - (Vector2)transform.position;
            this.transform.Translate(dir.x, dir.y, 0, Space.World);
        }
    }

}
