using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] Enemy enemy = null;
    [SerializeField] Maps map = null;
    Enemy[] enemies;

    int enemyNum = 5;
    void Start()
    {
        enemies = new Enemy[enemyNum];
        EnemyPlacement();
    }


    void EnemyPlacement()
    {
        for (int i = 0; i < enemyNum; i++)
        {
            enemies[i] = Instantiate(enemy);
            enemies[i].Map = map;
            enemies[i].RandomPlacement();
        }
    }

    public void EnemyAction()
    {
        foreach (var item in enemies)
        {
            item.Action();
        }
    }
}
