using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletGrade { Normal, Rare, Epic, Legendary }
public enum BulletType
{
    Normal1, Normal2,
    Rare1, Rare2,
    Epic1, Epic2, 
    Legendary1, Legendary2
}

[CreateAssetMenu(fileName = "BulletData", menuName = "Game/BulletData")]
public class BulletData : ScriptableObject
{
    [Header("Identity")]
    public BulletGrade grade;      // ��� (Normal �� Rare �� Epic �� Legendary)
    public BulletType type;        // ���� Ÿ��(ǥ���/�з���)

    [Header("Pool / Visual")]
    public PoolKey poolKey;        // Ǯ���� ���� �� ���� Ű
    public Sprite bulletSprite;    // �Ѿ� ��������Ʈ

    [Header("Combat Stats (Base)")]
    public float baseDamage = 10f;     // �⺻ ������
    public float baseDelay = 1.0f;    // �⺻ �߻� ����(��ٿ�)
    public float speed = 5f;      // �̵� �ӵ�

    [Header("Per Level Growth")]
    public float damageIncrement = 2f; // �ߺ�(������) �� ������
    public float delayDecrement = 0.05f; // �ߺ� �� ���ҷ�(���� ���Ұ�)

    [Header("Promotion Chain")]
    [Tooltip("���� ��� BulletData�� �����ϼ���. �ְ� ����̸� ����Ӵϴ�.")]
    public BulletData nextGradeBullet; // ���� ������� �±��� �� ����


}
