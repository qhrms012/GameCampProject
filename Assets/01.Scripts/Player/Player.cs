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
    public List<Enemy> touchingEnemies = new List<Enemy>(); // ����ִ� ���� ����
    private float damageDelay;
    private float damageInterval = 1f; // 1�ʸ��� ������ �ջ�



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
        // ���� �ð����� ������ �ջ� ����
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

        // ��������Ʈ ��ü
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
            state.Upgrade(); // �ߺ��̸� ��ȭ
            Debug.Log($"{newBullet.grade}-{newBullet.type} �Ѿ� Lv.{state.level}, ������ {state.currentDamage}, ������ {state.currentDelay}");
        }
        else
        {
            BulletState newState = new BulletState(newBullet);
            unlockedBullets.Add(newBullet.type, newState);
            Debug.Log($"{newBullet.grade}-{newBullet.type} �Ѿ� �رݵ�!");
        }
    }
    private void ApplyTotalDamage()
    {
        float totalDamage = 0;

        foreach (Enemy enemy in touchingEnemies)
        {
            if (enemy != null) // Ȥ�� �׾ Ǯ���� ��� ����
                totalDamage += enemy.damage; // Enemy ��ũ��Ʈ ���� ���ݷ� ��
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
        Debug.Log($"{gameObject.name} �ǰ�! ���� HP: {curHp}");

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

