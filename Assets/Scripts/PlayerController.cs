using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Chara
{
    [SerializeField] string playerName = "勇者";
    Status status;

    int hp;
    int mp;


    [SerializeField] Maps map = null;
    [SerializeField] Manager manager = null;
    Tile current;
    public override string Name { get => playerName; set { playerName = value; } }
    public override Status Status { get => status; set { status = value; } }
    public override int HP { get => hp; set { hp = value; } }
    public override int MP { get => mp; set { mp = value; } }
    public override Maps Map { get => map; set { map = value; } }
    public override Manager Manager { get => manager; set { manager = value; } }
    protected override Tile Current { get => current; set { current = value; } }

    //float timer = 0;

    private void Start()
    {
        status.max_hp = 50;
        status.max_mp = 50;
        status.atk = 15;
        status.def = 5;

        hp = status.max_hp;
        mp = status.max_mp;
    }

    //void Update()
    //{
    //    if (timer > 0.15f)
    //    {
    //        InputMG();
    //    }
    //    else
    //    {
    //        timer += Time.deltaTime;
    //    }
    //}

    /// <summary>
    /// キー入力
    /// </summary>
    public void Action(Vector2Int dir)
    {
        //Vector2Int dir = Vector2Int.zero;
        //if (Input.GetKeyDown("a"))
        //{
        //    dir += Vector2Int.left;
        //}
        //if (Input.GetKeyDown("d"))
        //{
        //    dir += Vector2Int.right;
        //}
        //if (Input.GetKeyDown("w"))
        //{
        //    dir += Vector2Int.up;
        //}
        //if (Input.GetKeyDown("s"))
        //{
        //    dir += Vector2Int.down;
        //}
        //if (Input.GetKeyDown("space"))
        //{

        //}

        if (dir != Vector2.zero)
        {
            Vector2 position = this.transform.position;
            Vector2Int point = new Vector2Int((int)position.x + dir.x, (int)position.y + dir.y);
            //Debug.Log(point.x + ", " + point.y);

            //　移動するタイルを確認する
            if (MoveingCheck(point))
            {
                Vector2Int p = new Vector2Int((int)position.x, (int)position.y);
                    
                //移動したタイルの情報を変える
                manager.MoveStateChange(p, Current);
                manager.MoveStateChange(point, Tile.other);
                    
                //移動
                this.transform.Translate(dir.x, dir.y, 0, Space.World);
                //timer = 0;
                manager.PlayerEnd();
            }

        }
    }
}
