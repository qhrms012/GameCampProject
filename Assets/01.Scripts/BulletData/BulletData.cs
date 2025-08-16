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
    public PoolKey poolKey;      // 풀에서 뽑을 때 쓰는 키
    public BulletGrade grade;    // 등급
    public BulletType type;      // 종류 (Normal1, Normal2, …)
    public float baseDamage;
    public float baseDelay;
    public float damageIncrement; // 중복 시 증가량
    public float delayDecrement;  // 중복 시 감소량
    public float speed;
    public Sprite bulletSprite;   // 이미지 (종류별 다름)

}
