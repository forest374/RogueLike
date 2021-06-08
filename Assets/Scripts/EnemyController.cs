using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Chara
{
    string charaName;
    Maps map;
    Manager manager;
    Tile current;

    List<Vector2Int> route = new List<Vector2Int>();
    [SerializeField] GameObject targetObj = null;
    Vector2Int targetP;
    int count = 0;
    public override string Name { get => charaName; set { charaName = value; } }
    public override Maps Map { get => map; set { map = value; } }
    public override Manager Manager { get => manager; set { manager = value; } }
    protected override Tile Current { get => current; set { current = value; } }

    private void Start()
    {
        targetObj = GameObject.FindGameObjectWithTag("Player");
    }

    public void Action()
    {
        if (targetObj)
        {
            Vector2 taget = targetObj.transform.position;
            targetP = new Vector2Int((int)taget.x, (int)taget.y);
            Vector2 myPosition = transform.position;

            float distX = targetP.x - myPosition.x;
            float distY = targetP.y - myPosition.y;

            if (distX <= 1 && distX >= -1 && distY <= 1 && distY >= -1)
            {
                Debug.Log("attack");
                manager.EnemyEnd();
                return;
            }
            OnMove();
        }
        manager.EnemyEnd();
    }

    /// <summary>
    /// 移動する
    /// </summary>
    public void OnMove()
    {
        // AStarは毎回やると重い　ターゲットが変わる、いなくなる、一定回数動く
        if (count > 4 || count >= route.Count - 1 )
        {
            Debug.Log("route");
            count = 0;
            route.Clear();
            RouteSet();
        }
        count++;

        Vector2 position = new Vector2(this.transform.position.x, this.transform.position.y);
        Vector2Int movePoint = route[count];
        movePoint -= new Vector2Int((int)position.x, (int)position.y);
        //Debug.Log(route[count].x + ", " + route[count].y);
        if (MoveingCheck(route[count]))
        {
            Vector2Int p = new Vector2Int((int)position.x, (int)position.y);

            //移動したタイルの情報を変える
            manager.MoveStateChange(p, Current);
            manager.MoveStateChange(route[count], Tile.other);
            //Debug.Log(movePoint.x + ", " + movePoint.y);
            this.transform.Translate(movePoint.x, movePoint.y, 0, Space.World);
            manager.EnemyEnd();
        }
    }


    void RouteSet()
    {
        Vector2 p = transform.position;
        Vector2Int position = new Vector2Int((int)p.x, (int)p.y);
        route = manager.OnAstar(position, targetP);
    }

}
