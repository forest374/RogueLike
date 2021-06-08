using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tile
{
    wall, room, road, other
}

public struct AriaRange
{
    public int top;
    public int bottom;
    public int right;
    public int left;
}
public struct RoomRange
{
    public int top;
    public int bottom;
    public int right;
    public int left;
}

public class Maps : MonoBehaviour
{
    [SerializeField] GameObject wallPrefab = null;
    [SerializeField] GameObject roadPrefab = null;
    [SerializeField] GameObject roomPrefab = null;
    [SerializeField] int mapSizeX = 60;
    [SerializeField] int mapSizeY = 30;
    [SerializeField] int splitNum = 6;
    Tile[,] tiles;

    public int MapSizeX { get => mapSizeX; }
    public int MapSizeY { get => mapSizeY; }
    public Tile[,] Tiles { get => tiles; }

    AriaRange[,] ariaRange;
    RoomRange[] roomRange;

    private void Awake()
    {
        splitNum = splitNum / 2 * 2; // 奇数をなくす
        tiles = new Tile[MapSizeX, MapSizeY];

        SplitMaps(splitNum);
        CreateRoad(ariaRange, roomRange);
        GenerateTiles();
    }


    /// <summary>
    /// mapSizeに合わせてtileを生成する
    /// </summary>
    void GenerateTiles()
    {
        for (int y = 0; y < MapSizeY; y++)
        {
            for (int x = 0; x < MapSizeX; x++)
            {
                GameObject obj; // 名前をつけなかったらいらない

                switch (tiles[x, y])
                {
                    case Tile.wall:
                        obj = Instantiate(wallPrefab, new Vector2(x, y), Quaternion.identity, transform);
                        obj.name = "wall (" + x + ", " + y + ")"; // いらないもの
                        break;
                    case Tile.room:
                        obj = Instantiate(roomPrefab, new Vector2(x, y), Quaternion.identity, transform);
                        obj.name = "room (" + x + ", " + y + ")"; // いらないもの
                        break;
                    case Tile.road:
                        obj = Instantiate(roadPrefab, new Vector2(x, y), Quaternion.identity, transform);
                        obj.name = "road (" + x + ", " + y + ")"; // いらないもの
                        break;
                    case Tile.other:
                        break;
                    default:
                        break;
                }
            }
        }
    }


    /// <summary>
    /// マップを分割してエリアを設定する
    /// </summary>
    void SplitMaps(int splitNum)
    {
        int ySplit = 2;
        int xSplit = splitNum / ySplit;
        ariaRange = new AriaRange[xSplit, ySplit];
        roomRange = new RoomRange[splitNum];

        // 分割した座標を設定する
        int[] yy = new int[ySplit + 1]; 
        int[] xx = new int[xSplit + 1]; // 端はいらないので-1する

        for (int i = 0; i < yy.Length; i++)
        {
            yy[i] = (MapSizeY / ySplit) * i;
        }
        for (int i = 0; i < xx.Length; i++)
        {
            xx[i] = (MapSizeX / xSplit) * i; 
        }

        // eriaの数だけCreateRoomする
        int count = 0;
        for (int y = 0; y < ySplit; y++)
        {
            for (int x = 0; x < xSplit; x++)
            {
                ariaRange[x, y].top = yy[y + 1];
                ariaRange[x, y].bottom = yy[y];
                ariaRange[x, y].right = xx[x + 1];
                ariaRange[x, y].left = xx[x];

                CreateRoom(ariaRange[x, y],count);
                count++;
            }
        }

    }
    /// <summary>
    /// eriaの範囲を受け取りその内部にroomを作る
    /// </summary>
    /// <param name="aria">eriaの範囲</param>
    void CreateRoom(AriaRange ariaRange, int count)
    {
        //　エリアのサイズ
        int width = ariaRange.right - ariaRange.left;
        int height = ariaRange.top - ariaRange.bottom;

        //　roomのサイズ
        int xX = Random.Range(width / 3, width - 2);
        int yY = Random.Range(height / 3, height - 2);

        //　roomの左下の座標 ・道を作る際に隣接しないように数値が違う
        int pointX = ariaRange.left + Random.Range( 2, width - xX);
        int pointY = ariaRange.bottom + Random.Range( 2, height - yY);

        //　roomの範囲を記録
        roomRange[count].top = pointY + yY;
        roomRange[count].bottom = pointY;
        roomRange[count].right = pointX + xX;
        roomRange[count].left = pointX;

        //　roomの範囲のtileを変更
        for (int y = 0; y < yY; y++)
        {
            for (int x = 0; x < xX; x++)
            {
                tiles[pointX + x, pointY + y] = Tile.room;
            }
        }
    }

    /// <summary>
    /// エリア範囲とroom範囲を受け取り道を作る
    /// </summary>
    /// <param name="aria">エリア範囲</param>
    /// <param name="room">room範囲</param>
    void CreateRoad(AriaRange[,] ariaRange, RoomRange[] roomRange)
    {
        int num = ariaRange.Length / ariaRange.GetLength(1);

        //Debug.Log("縦方向の部屋をつなぐ");
        for (int i = 0; i < num; i++)
        {
            int head = i + num;
            //Debug.Log(i + ", " + bottom); // roomの番号

            // 入口
            int bottomEntrance = Random.Range(roomRange[i].left, roomRange[i].right);
            int headEntrance = Random.Range(roomRange[head].left, roomRange[head].right);

            // 入口からエリア端までを道に変更する
            for (int y = roomRange[head].bottom - 1; y > ariaRange[i, 0].top; y--)
            {
                tiles[headEntrance, y] = Tile.road;
            }
            for (int y = roomRange[i].top; y < ariaRange[i, 0].top; y++)
            {
                tiles[bottomEntrance, y] = Tile.road;
            }

            // 道をつなげる
            if (bottomEntrance > headEntrance)
            {
                RoadConnect("縦", ariaRange, i, 0, bottomEntrance, headEntrance);
            }
            else
            {
                RoadConnect("縦", ariaRange, i, 0, headEntrance, bottomEntrance);
            }
        }


        //Debug.Log("横方向の部屋をつなぐ");
        for (int k = 0; k < ariaRange.GetLength(1); k++)
        {
            //Debug.Log("-----");
            for (int i = 0; i < num - 1; i++)
            {
                int a = i;
                if (k == 1)
                {
                    a += num; // 下側
                }
                //Debug.Log(a + ", " + (a + 1));

                // 入口
                int leftEntrance = Random.Range(roomRange[a].bottom, roomRange[a].top);
                int rightEntrance = Random.Range(roomRange[a + 1].bottom, roomRange[a + 1].top);

                // 入口からエリア端までを道に変更する
                for (int h = roomRange[a].right; h < ariaRange[i, k].right; h++)
                {
                    tiles[h, leftEntrance] = Tile.road;
                }
                for (int h = roomRange[a + 1].left - 1; h > ariaRange[i, k].right; h--)
                {
                    tiles[h, rightEntrance] = Tile.road;
                }

                // 道をつなげる
                if (leftEntrance > rightEntrance)
                {
                    RoadConnect("横", ariaRange, i, k, leftEntrance, rightEntrance);
                }
                else
                {
                    RoadConnect("横", ariaRange, i, k, rightEntrance, leftEntrance);
                }
            }
        }
    }

    private void RoadConnect(string direction, AriaRange[,] ariaRange, int x, int y, int large, int small)
    {
        if (direction == "縦")
        {
            for (int h = small; h <= large; h++)
            {
                tiles[h, ariaRange[x, y].top] = Tile.road;
            }
        }
        else
        {
            for (int h = small; h <= large; h++)
            {
                //Debug.Log(x + ", " + y);
                tiles[ariaRange[x, y].right, h] = Tile.road;
            }
        }
    }


    /// <summary>
    /// 座標を受け取りそこのTileStateを返す
    /// </summary>
    /// <param name="point">座標</param>
    /// <returns>TileState</returns>
    public Tile TileStateCheck(Vector2 point)
    {
        if (point.x >= 0 && point.x < MapSizeX &&
            point.y >= 0 && point.y < MapSizeY)
        {
            return tiles[(int)point.x, (int)point.y];
        }
        return Tile.other;
    }


    /// <summary>
    /// タイルの上に物（キャラ）を取り除く、置く
    /// </summary>
    /// <param name="point">座標</param>
    /// <param name="tile">変更後のtileState</param>
    public void TileStateChange(Vector2 point, Tile tile)
    {
        if (point.x >= 0 && point.x < MapSizeX && point.y >= 0 && point.y < MapSizeY)
        {
            tiles[(int)point.x, (int)point.y] = tile;
        }
    }

    /// <summary>
    /// roomTileのどこかの座標を返す
    /// </summary>
    /// <returns>座標</returns>
    public Vector2 RandomRoom()
    {
        int num = Random.Range(0, roomRange.Length);

        int x = Random.Range(roomRange[num].left, roomRange[num].right);
        int y = Random.Range(roomRange[num].bottom, roomRange[num].top);

        Vector2 point = new Vector2(x, y);

        return point;
    }
}
