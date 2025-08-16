using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float curHp = 0;
    public float maxHp = 100;



    private Rigidbody2D rb;
    private BoxCollider2D bx;
    public Transform PlayerPosition;
    public List<Enemy> touchingEnemies = new List<Enemy>(); // 닿아있는 적들 저장
    private float damageDelay;
    private float damageInterval = 1f; // 1초마다 데미지 합산



    public bool isDie;

    private Dictionary<BulletType, BulletState> unlockedBullets = new Dictionary<BulletType, BulletState>();
    private void Awake()
    {
        bx = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        curHp = maxHp;
    }

    private void Update()
    {
        // 일정 시간마다 데미지 합산 적용
        damageDelay += Time.deltaTime;
        if (damageDelay >= damageInterval)
        {
            ApplyTotalDamage();
            damageDelay = 0;
        }

        float dt = Time.deltaTime;

        foreach (var bulletEntry in unlockedBullets)
        {
            BulletState state = bulletEntry.Value;

            if (state.CanFire(dt))
            {
                Fire(state);
            }
        }
    }
    void Fire(BulletState state)
    {
        GameObject bullet = ObjectPoolManager.Instance.Get(state.data.poolKey);
        bullet.transform.position = transform.position;

        // 스프라이트 교체
        SpriteRenderer sr = bullet.GetComponent<SpriteRenderer>();
        sr.sprite = state.data.bulletSprite;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.down * state.data.speed;

        PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();
        bulletScript.damage = state.currentDamage;
    }
    public void UnlockBullet(BulletData newBullet)
    {
        if (unlockedBullets.TryGetValue(newBullet.type, out BulletState state))
        {
            state.Upgrade(); // 중복이면 강화
            Debug.Log($"{newBullet.grade}-{newBullet.type} 총알 Lv.{state.level}, 데미지 {state.currentDamage}, 딜레이 {state.currentDelay}");
        }
        else
        {
            BulletState newState = new BulletState(newBullet);
            unlockedBullets.Add(newBullet.type, newState);
            Debug.Log($"{newBullet.grade}-{newBullet.type} 총알 해금됨!");
        }
    }
    private void ApplyTotalDamage()
    {
        float totalDamage = 0;

        foreach (Enemy enemy in touchingEnemies)
        {
            if (enemy != null) // 혹시 죽어서 풀링된 경우 방지
                totalDamage += enemy.damage; // Enemy 스크립트 안의 공격력 값
        }

        if (totalDamage > 0)
            TakeDamage(totalDamage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && !touchingEnemies.Contains(enemy))
        {
            touchingEnemies.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && touchingEnemies.Contains(enemy))
        {
            touchingEnemies.Remove(enemy);
        }
    }

    public void TakeDamage(float amount)
    {
        curHp -= amount;
        Debug.Log($"{gameObject.name} 피격! 남은 HP: {curHp}");

        if (curHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDie = true;
        gameObject.SetActive(false);
    }
}

