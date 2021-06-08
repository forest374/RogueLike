using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Chara
{
    [SerializeField] string playerName = "勇者";
    [SerializeField] Maps map = null;
    [SerializeField] Manager manager = null;
    public override string Name { get => playerName; set { playerName = value; } }
    public override Maps Map => map;
    public override Manager Manager => manager;

    float timer = 0;


    void Update()
    {
        if (timer > 0.15f)
        {
            InputMG();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// キー入力
    /// </summary>
    private void InputMG()
    {
        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKeyDown("a"))
        {
            dir += Vector2Int.left;
        }
        if (Input.GetKeyDown("d"))
        {
            dir += Vector2Int.right;
        }
        if (Input.GetKeyDown("w"))
        {
            dir += Vector2Int.up;
        }
        if (Input.GetKeyDown("s"))
        {
            dir += Vector2Int.down;
        }
        if (Input.GetKeyDown("space"))
        {
        }

        if (dir != Vector2.zero)
        {
            Vector2 position = transform.position;
            Vector2Int point = new Vector2Int((int)position.x + dir.x, (int)position.y + dir.y);
            Debug.Log(point.x + ", " + point.y);

            if (MoveingCheck(point))
            {
                this.transform.Translate(dir.x, dir.y, 0, Space.World);
                timer = 0;
            }

        }
    }
}
