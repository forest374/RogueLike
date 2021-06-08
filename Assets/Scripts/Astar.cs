using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState
{
    yetOpen, open, close, none
}

public struct TileInformation
{
    public TileState tileState;
    public float dist;
    public float cost;
    public Vector2Int parent;
}
public class Astar : MonoBehaviour
{
    [SerializeField] Maps map = null;
    TileInformation[,] tilesI;

    int mapSizeX;
    int mapSizeY;

    Vector2Int startP;
    Vector2Int goalP;
    List<Vector2Int> route = new List<Vector2Int>();
    Vector2Int halfwayPoint = new Vector2Int(-1, -1);


    Tile[,] tiles;
    void Start()
    {
        mapSizeX = map.MapSizeX;
        mapSizeY = map.MapSizeY;
        tilesI = new TileInformation[mapSizeX, mapSizeY]; 
        tiles = map.Tiles;
    }

    /// <summary>
    /// A＊アルゴリズムを行いその経路を返す
    /// </summary>
    /// <param name="startP">キャラの位置</param>
    /// <param name="goalP">目的地</param>
    /// <returns>経路</returns>
    public List<Vector2Int> OnAStar(Vector2Int startP, Vector2Int goalP)
    {
        this.startP = startP;
        this.goalP = goalP;
        int x = startP.x;
        int y = startP.y;
        tilesI[x, y].tileState = TileState.close;
        tilesI[x, y].cost = 0;
        tilesI[x, y].parent = Vector2Int.zero;
        EightDirections(x, y);


        RouteRecord(goalP);
        route.Reverse();

        HalfwaySet();

        return route;
    }


    /// <summary>
    /// 渡された位置の八方向を調べる
    /// </summary>
    /// <param name="tileX">位置X</param>
    /// <param name="tileY">位置Y</param>
    void EightDirections(int tileX, int tileY)
    {
        // 目的地なら終了
        Vector2Int aaaaa = new Vector2Int(tileX, tileY);
        if (aaaaa == goalP)
        {
            return;
        }

        // 八方向を調べる
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                int xx = tileX + x;
                int yy = tileY + y;

                //　マップ外　＆　調べ終わった場所
                if (xx > 0 && xx < mapSizeX && yy > 0 && yy < mapSizeY &&
                    tilesI[xx, yy].tileState == TileState.close)
                {
                    continue;
                }

                //　まだ確定していないところ
                if (tilesI[xx, yy].tileState == TileState.yetOpen || tilesI[xx, yy].tileState == TileState.open)
                {
                    // 壁のとき
                    if (tiles[xx, yy] == Tile.wall)
                    {
                        tilesI[xx, yy].tileState = TileState.none;
                        continue;
                    }

                    // 斜め移動のとき横が壁ならできない
                    float cost;
                    if (xx * yy != 0) 
                    {
                        cost = tilesI[tileX, tileY].cost + 1.4f;
                        if (tiles[xx, tileY] == Tile.wall || tiles[tileX, yy] == Tile.wall)
                        {
                            continue;
                        }
                    }
                    else //　四方向のとき
                    {
                        cost = tilesI[tileX, tileY].cost + 1;

                    }

                    //　一度開けた場所でコストが前回よりも多いとき
                    if (tilesI[xx, yy].tileState == TileState.open && tilesI[xx, yy].cost < cost)
                    {
                        continue;
                    }

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

    /// <summary>
    /// 調べたなかで距離の最小値を探しEightDirections(）に座標を渡す
    /// </summary>
    void MinDist()
    {
        //　すべてのタイルを調べる
        float min = float.MaxValue;
        int xx = -1;
        int yy = -1;
        for (int y = 0; y < mapSizeY; y++)
        {
            for (int x = 0; x < mapSizeX; x++)
            {
                //　TileStateがopen以外は調べない
                if (tilesI[x, y].tileState == TileState.open)
                {
                    //　最小値を探す
                    float dist = tilesI[x, y].dist;
                    if (min > dist)
                    {
                        min = dist;
                        xx = x;
                        yy = y;
                    }
                    else if (min == dist && xx != -1)// 距離が同じ場合
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
            //最小値はcloseにする
            tilesI[xx, yy].tileState = TileState.close;
            EightDirections(xx, yy);
        }
    }


    /// <summary>
    /// たどってきたルートを記録する
    /// </summary>
    /// <param name="route">ルート</param>
    void RouteRecord(Vector2Int point)
    {
        Vector2Int aa = tilesI[point.x, point.y].parent;
        if (aa == Vector2Int.zero)
        {
            return;
        }
        route.Add(tilesI[point.x, point.y].parent);
        RouteRecord(aa);
    }


    /// <summary>
    /// 経路の最初に着く道を中間地点に設定しそこをゴールとしてA*を行う
    /// </summary>
    private void HalfwaySet()
    {
        // たどった経路の最初に着く道を中間地点に設定する
        Tile tile = Tile.other;
        foreach (var item in route)
        {
            if (tile == Tile.room && map.Tiles[item.x, item.y] == Tile.road)
            {
                halfwayPoint = new Vector2Int(item.x, item.y);
                this.goalP = halfwayPoint;
                break;
            }
            tile = map.Tiles[item.x, item.y];
        }

        // 中間地点が設定されていたら中間地点をゴールとしてA*を行う
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
            tilesI[startP.x, startP.y].tileState = TileState.close;
            tilesI[startP.x, startP.y].cost = 0;
            tilesI[startP.x, startP.y].parent = Vector2Int.zero;
            EightDirections(startP.x, startP.y);

            route.Clear();
            RouteRecord(this.goalP);
            route.Reverse();
        }
    }
}
