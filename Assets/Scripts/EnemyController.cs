using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Chara
{
    string charaName;
    Maps map;
    Manager manager;
    Astar astar;
    List<Vector2Int> route;
    GameObject targetObj;
    Vector2Int targetP;
    int count;
    public override string Name { get => charaName; set { charaName = value; } }
    public override Maps Map => map;
    public override Manager Manager => manager;


    public void Action()
    {
        if (targetObj)
        {

        }
    }

    /// <summary>
    /// 移動する
    /// </summary>
    public void OnMove()
    {
        // AStarは毎回やると重い　ターゲットが変わる、いなくなる、一定回数動く
        if (count > 4 || count >= route.Count )
        {
            RouteSet();
        }

        MoveingCheck(route[count]);
    }


    void RouteSet()
    {
        Vector2 p = transform.position;
        Vector2Int position = new Vector2Int((int)p.x, (int)p.y);
        route = astar.OnAStar(position, targetP);
    }

}
