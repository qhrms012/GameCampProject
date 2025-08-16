using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletGrade { Normal, Rare, Epic, Legendary }
public enum BulletType
{
    Normal1, Normal2, Normal3,
    Rare1, Rare2, Rare3,
    Epic1, Epic2, Epic3,
    Legendary1, Legendary2, Legendary3
}

[CreateAssetMenu(fileName = "BulletData", menuName = "Game/BulletData")]
public class BulletData : ScriptableObject
{
    public PoolKey poolKey;      // Ǯ���� ���� �� ���� Ű
    public BulletGrade grade;    // ���
    public BulletType type;      // ���� (Normal1, Normal2, ��)
    public float baseDamage;
    public float baseDelay;
    public float damageIncrement; // �ߺ� �� ������
    public float delayDecrement;  // �ߺ� �� ���ҷ�
    public float speed;
    public Sprite bulletSprite;   // �̹��� (������ �ٸ�)

}
