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


    private float fireDelay;
    public bool isDie;

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

        fireDelay += Time.deltaTime;
        if (fireDelay >= 1f && isDie == false)
        { 
            StartCoroutine(Shot()); 
            fireDelay = 0; 
        }
    }
    IEnumerator Shot()
    {
        GameObject bullet = ObjectPoolManager.Instance.Get(PoolKey.PlayerBullet);
        Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
        Transform spawnBulletPos = PlayerPosition;
        bullet.transform.position = spawnBulletPos.position;
        bulletRigid.velocity = Vector2.down * 10f; 
        yield return null;
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

