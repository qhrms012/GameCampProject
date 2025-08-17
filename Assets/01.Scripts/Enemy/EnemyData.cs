using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObject/EnemyData" , order = 0)]

public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHp;
    public float speed;
    public int coinReward;
    public Sprite enemySprite;
}
