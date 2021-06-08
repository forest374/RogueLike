using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn
{
    myTurn, enemyTurn, other
}
public class TestSystem : MonoBehaviour
{
    [SerializeField] Manager manager = null;
    Turn turn = Turn.myTurn;
    public Turn Turn { get => turn; set {turn = value;} }


    void Update()
    {
        switch (Turn)
        {
            case Turn.myTurn:
                break;
            case Turn.enemyTurn:
                EnemyTurn();
                break;
            case Turn.other:
                break;
            default:
                break;
        }
    }

    void EnemyTurn()
    {
        manager.EnemyAction();
    }
}
