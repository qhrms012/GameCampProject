using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletGrade { Normal, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "BulletData", menuName = "Game/BulletData")]
public class BulletData : ScriptableObject
{
    public PoolKey poolKey;
    public BulletGrade grade;
    public float baseDamage = 10f;
    public float baseDelay = 1f;
    public float damageIncrement = 2f;  // �ߺ��� ������
    public float delayDecrement = 0.1f; // �ߺ��� �߻�ӵ� ����
    public float speed = 10f;

}
