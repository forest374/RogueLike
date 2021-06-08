using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    //[SerializeField] Enemy enemy = null;
    [SerializeField] EnemyController enemy = null;
    [SerializeField] Maps map = null;
    [SerializeField] TestSystem system = null;
    [SerializeField] PlayerController player = null;
    [SerializeField] Astar astar = null;
    //Enemy[] enemies;
    EnemyController[] enemies; 
    int endCount = 0;

    int maxEnemy = 10;
    int enemyNum = 1;
    void Start()
    {
        player.RandomPlacement();
        enemies = new EnemyController[maxEnemy];
        EnemyPlacement();

    }


    void EnemyPlacement()
    {
        for (int i = 0; i < enemyNum; i++)
        {
            enemies[i] = Instantiate(enemy);
            enemies[i].name = i.ToString();
            enemies[i].Map = map;
            enemies[i].Manager = this;
            enemies[i].RandomPlacement();
        }
    }

    public void EnemyAction()
    {
        for (int i = 0; i < enemyNum; i++)
        {
            enemies[i].Action();
        }
    }
    public void PlayerAction(Vector2Int dir)
    {
        player.Action(dir);
    }

    /// <summary>
    /// エネミーのターンエンド
    /// </summary>
    public void EnemyEnd()
    {
        endCount++;
        if (enemyNum == endCount)
        {
            endCount = 0;
            system.Turn = Turn.playerTurn;
        }
    }

    /// <summary>
    /// プレイヤーのターンエンド
    /// </summary>
    public void PlayerEnd()
    {
        system.Turn = Turn.enemyTurn;
    }

    /// <summary>
    /// 移動した際にtileの情報を更新する
    /// </summary>
    /// <param name="point">座標</param>
    /// <param name="tile">tileの情報</param>
    public void MoveStateChange(Vector2Int point, Tile tile)
    {
        map.TileStateChange(point, tile);
    }

    /// <summary>
    /// 座標の情報を返す
    /// </summary>
    /// <param name="point">座標</param>
    /// <returns>tileの情報</returns>
    public Tile TileStateCheck(Vector2Int point)
    {
        Tile tile = map.TileStateCheck(point);
        return tile;
    }

    public List<Vector2Int> OnAstar(Vector2Int position, Vector2Int targetP)
    {
        List<Vector2Int> route = astar.OnAStar(position, targetP);
        return route;
    }
}
