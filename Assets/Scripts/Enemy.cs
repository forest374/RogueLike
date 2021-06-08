using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] GameObject player = null;
    Tile current = Tile.room;
    Maps map = null;

    protected override Tile Current { get => current; set { current = value; } }
    public override Maps Map { get => map; set {map = value; } }

    protected override void StageStart()
    {
        base.StageStart();
    }

    /// <summary>
    /// 行動する
    /// </summary>
    public void Action()
    {
        Vector2 myPosition = transform.position;

        float distX = player.transform.position.x - myPosition.x;
        float distY = player.transform.position.y - myPosition.y;

        if (distX <= 1 && distX >= -1 && distY <= 1 && distY >= -1)
        {
            Debug.Log("attack");
            return;
        }

        TargetMove(player.transform.position);
    }

    /// <summary>
    /// 目的地へ移動する
    /// </summary>
    /// <param name="destination">目的地</param>
    void TargetMove(Vector2 destination)
    {
        Vector2 point = transform.position;

        float distX = destination.x - point.x;
        float distY = destination.y - point.y;


        if (distX * distX > distY * distY)
        {
            if (distX > 0)
            {
                point = new Vector2(point.x + 1, point.y);
            }
            else
            {
                point = new Vector2(point.x - 1, point.y);
            }
        }
        else if (distY * distY > distX * distX)
        {
            if (distY > 0)
                point = new Vector2(point.x, point.y + 1);
            else
                point = new Vector2(point.x, point.y - 1);
        }
        else
        {
            if (distX > 0 && distY > 0)
                point = new Vector2(point.x + 1, point.y + 1);
            else if (distX < 0 && distY < 0)
                point = new Vector2(point.x - 1, point.y - 1);
            else if (distX > 0 && distY < 0)
                point = new Vector2(point.x + 1, point.y - 1);
            else
                point = new Vector2(point.x - 1, point.y + 1);

        }
        //MoveCheck((Vector2Int)point);
    }
    protected override void MoveCheck(Vector2Int move)
    {
        base.MoveCheck(move);
    }

}
