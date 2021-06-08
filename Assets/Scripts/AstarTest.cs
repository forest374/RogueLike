using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AstarTest : MonoBehaviour
{
    [SerializeField] GameObject obj = null;
    [SerializeField] GameObject objC = null;
    [SerializeField] GameObject te = null;
    [SerializeField] GameObject ca = null;
    [SerializeField] GameObject S = null;
    [SerializeField] GameObject G = null;
    [SerializeField] Maps map = null;
    TileInformation[,] tilesI;

    int mapSizeX;
    int mapSizeY;

    int score = 0;
    Vector2Int startP;
    Vector2Int goalP;

    List<Vector2Int> movePoint = new List<Vector2Int>();
    Vector2Int halfwayPoint;

    Tile[,] tiles;
    void Start()
    {
        mapSizeX = map.MapSizeX;
        mapSizeY = map.MapSizeY;
        tilesI = new TileInformation[mapSizeX, mapSizeY];
        halfwayPoint = new Vector2Int(-1, -1);
    }

    public void On()
    {
        //Debug.Log("ゴールまでの距離が同じ場合の優先順位（移動した距離の大きさ）で回数が変わるかをで調べている");
        tiles = map.Tiles;

        for (int i = 0; i < 2;)
        {
            int x = Random.Range(0, mapSizeX);
            int y = Random.Range(0, mapSizeY);

            if (tiles[x, y] == Tile.room)
            {
                if (i == 0)
                {
                    startP = new Vector2Int(x, y);
                    Instantiate(S, (Vector2)startP, Quaternion.identity);
                    //Debug.Log(startP);
                    i++;
                }
                else
                {
                    goalP = new Vector2Int(x, y);
                    Instantiate(G, (Vector2)goalP, Quaternion.identity);
                    //Debug.Log(goalP);
                    i++;
                }
            }
        }

        OnAStar(startP, goalP);

    }

    void Route(Vector2Int route)
    {
        Vector2Int aa = tilesI[route.x, route.y].parent;
        if (aa == Vector2Int.zero)
        {
            return;
        }

        movePoint.Add(tilesI[route.x, route.y].parent);
        Route(aa);
    }

    public void OnAStar(Vector2Int startP, Vector2Int goalP)
    {
        this.startP = startP;
        this.goalP = goalP;
        int x = startP.x;
        int y = startP.y;
         tilesI[x, y].tileState = TileState.close;
        tilesI[x, y].cost = 0;
        tilesI[x, y].parent = Vector2Int.zero;
        EightDirections(x, y);

        Route(this.goalP);

        movePoint.Reverse();
        Tile tile = Tile.other;
        foreach (var item in movePoint)
        {
            if (tile == Tile.room && map.Tiles[item.x, item.y] == Tile.road)
            {
                halfwayPoint = new Vector2Int(item.x, item.y);
                this.goalP = halfwayPoint;
                break;
            }
            tile = map.Tiles[item.x, item.y];
        }

        if (halfwayPoint != new Vector2Int(-1, -1))
        {
            for (int yy = 0; yy < mapSizeY; yy++)
            {
                for (int xx = 0; xx < mapSizeX; xx++)
                {
                    tilesI[xx, yy].tileState = TileState.yetOpen;
                    tilesI[xx, yy].parent = Vector2Int.zero;
                }
            }
            tilesI[x, y].tileState = TileState.close;
            tilesI[x, y].cost = 0;
            tilesI[x, y].parent = Vector2Int.zero;
            EightDirections(x, y);

            movePoint.Clear();
            Route(this.goalP);
            movePoint.Reverse();
        }

        Debug.Log(movePoint.Count);
        movePoint.RemoveAt(0);
        foreach (var item in movePoint)
        {
            Instantiate(obj, (Vector2)item, Quaternion.identity, transform);
        }
    }
    void EightDirections(int tileX, int tileY)
    {
        Vector2Int aaaaa = new Vector2Int(tileX, tileY);
        if (aaaaa == goalP)
        {
            return;
        }

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                int xx = tileX + x;
                int yy = tileY + y;

                if (xx > 0 && xx < mapSizeX && yy > 0 && yy < mapSizeY &&
                    tilesI[xx, yy].tileState == TileState.close)
                {
                    continue;
                }

                if (tilesI[xx, yy].tileState == TileState.yetOpen || tilesI[xx, yy].tileState == TileState.open)
                {
                    if (tiles[xx, yy] == Tile.wall)
                    {
                        tilesI[xx, yy].tileState = TileState.none;
                        continue;
                    }

                    float cost;
                    if (xx * yy != 0) // 斜め移動のとき横が壁ならできない
                    {
                        cost = tilesI[tileX, tileY].cost + 1.4f;
                        if (tiles[xx, tileY] == Tile.wall || tiles[tileX, yy] == Tile.wall)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        cost = tilesI[tileX, tileY].cost + 1;

                    }
                    if (tilesI[xx, yy].tileState == TileState.open && tilesI[xx, yy].cost < cost)
                    {
                        continue;
                    }

                    Instantiate(objC, new Vector2(xx, yy), Quaternion.identity, transform);

                    //　簡易版
                    int dX = goalP.x - xx;
                    int dY = goalP.y - yy;

                    int distG = dX * dX + dY * dY;
                    float dist = cost + distG;

                    //　精度はこっちのほうが高い　けどまだ完全じゃないし計算量が多い
                    //int distG = dX * dX + (dY * dY);
                    //float dist = Mathf.Sqrt(distG);

                    //dX = startP.x - xx;
                    //dY = startP.y - yy;
                    //distG = dX * dX + (dY * dY);
                    //dist += Mathf.Sqrt(distG);

                    tilesI[xx, yy].cost = cost;
                    tilesI[xx, yy].dist = dist;

                    tilesI[xx, yy].parent = new Vector2Int(tileX, tileY);
                    tilesI[xx, yy].tileState = TileState.open;

                }
            }
        }

        MinDist();
    }

    void MinDist()
    {
        float min = float.MaxValue;
        int xx = -1;
        int yy = -1;
        for (int y = 0; y < mapSizeY; y++)
        {
            for (int x = 0; x < mapSizeX; x++)
            {
                if (tilesI[x, y].tileState == TileState.open)
                {
                    float dist = tilesI[x, y].dist;
                    if (min > dist) 
                    {
                        min = dist;
                        xx = x;
                        yy = y;
                    }
                    else if (min == dist && xx != -1 )// 距離が同じ場合
                    {
                        //実際に移動した距離が大きい方を先に行う
                        if (tilesI[xx, yy].cost < tilesI[x, y].cost)
                        {
                            xx = x;
                            yy = y;
                        }
                    }
                }
            }
        }
        if (xx >= 0 && yy >= 0)
        {
            //tilesI[xx, yy].obj = Instantiate(obj, new Vector2(xx, yy), Quaternion.identity, transform);
            //tilesI[xx, yy].obj.name = tilesI[xx, yy].cost + " :" + tilesI[xx, yy].dist;
            tilesI[xx, yy].tileState = TileState.close;
            EightDirections(xx, yy);
        }
    }

}
