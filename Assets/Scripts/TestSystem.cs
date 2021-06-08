using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn
{
    playerTurn, enemyTurn, other
}
public class TestSystem : MonoBehaviour
{
    [SerializeField] Manager manager = null;
    Turn turn = Turn.playerTurn;
    public Turn Turn { get => turn; set {turn = value;} }

    float timer = 0;

    void Update()
    {
        switch (Turn)
        {
            case Turn.playerTurn:
                //Debug.Log("player");

                if (timer > 0.15f)
                {
                    InputMG();
                }
                else
                {
                    timer += Time.deltaTime;
                }
                break;
            case Turn.enemyTurn:
                //Debug.Log("enemy");
                EnemyTurn();
                break;
            case Turn.other:
                break;
            default:
                break;
        }
    }

    void InputMG()
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

        manager.PlayerAction(dir);
    }

    void EnemyTurn()
    {
        manager.EnemyAction();
    }
}
