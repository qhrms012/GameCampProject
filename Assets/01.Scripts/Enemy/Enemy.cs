using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float curHp;
    public float maxHp = 200;
    public float speed = 1f;
    public float damage;

    public int coinReward = 1;

    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private PlayerBullet pb;
    private SpriteRenderer spriteRenderer;

    // �̺�Ʈ ���� (�׾��� ��)
    public static event System.Action<int> OnEnemyDied;

    public EnemyData data;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        pb = GetComponent<PlayerBullet>();
        curHp = maxHp;
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    public void Init(EnemyData enemyData, Vector2 spawnPos, Vector2 dir)
    {
        data = enemyData;
        curHp = data.maxHp;

        transform.position = spawnPos;

        if (spriteRenderer != null && data.enemySprite != null)
            spriteRenderer.sprite = data.enemySprite;

        rb.velocity = dir * data.speed;
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
        // �̺�Ʈ
        OnEnemyDied?.Invoke(coinReward);

        // �׾��� �� ó�� (������Ʈ ��Ȱ��ȭ)
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            player.TakeDamage(damage);
            rb.velocity = Vector2.zero;
            Debug.Log($"{gameObject.name} �� �÷��̾� �浹, �̵� ����");
        }
    }
}
