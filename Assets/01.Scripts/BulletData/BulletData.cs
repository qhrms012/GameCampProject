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
    public BulletGrade grade;      // 등급 (Normal → Rare → Epic → Legendary)
    public BulletType type;        // 기존 타입(표기용/분류용)

    [Header("Pool / Visual")]
    public PoolKey poolKey;        // 풀에서 뽑을 때 쓰는 키
    public Sprite bulletSprite;    // 총알 스프라이트

    [Header("Combat Stats (Base)")]
    public float baseDamage = 10f;     // 기본 데미지
    public float baseDelay = 1.0f;    // 기본 발사 지연(쿨다운)
    public float speed = 5f;      // 이동 속도

    [Header("Per Level Growth")]
    public float damageIncrement = 2f; // 중복(레벨업) 시 증가량
    public float delayDecrement = 0.05f; // 중복 시 감소량(지연 감소값)

    [Header("Promotion Chain")]
    [Tooltip("다음 등급 BulletData를 연결하세요. 최고 등급이면 비워둡니다.")]
    public BulletData nextGradeBullet; // 다음 등급으로 승급할 때 참조


}
