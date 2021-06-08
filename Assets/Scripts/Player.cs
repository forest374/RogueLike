using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    //[SerializeField] TestSystem testSystem = null;
    [SerializeField] Maps map = null;
    Tile current = Tile.room;

    float timer = 0;

    public override Maps Map { get => map; set { map = value; } }
    protected override Tile Current { get => current; set { current = value; } }

    private void Start()
    {
        RandomPlacement();
    }

    void Update()
    {
        //if (testSystem.Turn != Turn.myTurn)
        //{
        //    return;
        //}
        if (timer > 0.1f)
        {
            InputMG();
        }
        else
        {
            timer += Time.deltaTime;
        }

    }

    private void InputMG()
    {
        Vector2 a = Vector2.zero;
        if (Input.GetKeyDown("a"))
        {
            a += Vector2.left;
            //Move(Vector2.left);
        }
        if (Input.GetKeyDown("d"))
        {
            a += Vector2.right;
            //Move(Vector2.right);
        }
        if (Input.GetKeyDown("w"))
        {
            a += Vector2.up;
            //Move(Vector2.up);
        }
        if (Input.GetKeyDown("s"))
        {
            a += Vector2.down;
            //Move(Vector2.down);
        }
        if (Input.GetKeyDown("space"))
        {
            TurnEnd();
        }

        if (a != Vector2.zero)
        {
            Move(a);
            timer = 0;
        }
    }

    /// <summary>
    /// 入力方向に移動する
    /// </summary>
    /// <param name="dir">入力方向</param>
    private void Move(Vector2 dir)
    {
        Vector2 move = (Vector2)this.transform.position + dir;
        MoveCheck(move);
    }

    /// <summary>
    /// 移動先を受け取り移動できるなら移動する
    /// カメラも移動させる
    /// </summary>
    /// <param name="move">移動先</param>
    protected override void MoveCheck(Vector2Int move)
    {
        Tile tileState = Map.TileStateCheck(move);
        if (tileState == Tile.road || tileState == Tile.room)
        {
            Map.TileStateChange(transform.position, Current);
            Map.TileStateChange(move, Tile.other);

            Current = tileState;
            Vector2 dir = move - (Vector2)transform.position;
            this.transform.Translate(dir.x, dir.y, 0, Space.World);
            Camera.main.transform.Translate(dir.x, dir.y, 0, Space.World);

            TurnEnd();
        }
    }

    /// <summary>
    /// ランダムな部屋の位置に配置する
    /// カメラの位置も変更する
    /// </summary>
    public override void RandomPlacement()
    {
        base.RandomPlacement();
        Vector3 camera = transform.position;
        camera.z = -1;
        Camera.main.transform.position = camera;
    }

    /// <summary>
    /// enemyTurnに変える
    /// </summary>
    protected void TurnEnd()
    {
        Debug.Log("おわり");
        //testSystem.Turn = Turn.enemyTurn;
    }
}
