using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor.AssetImporters;
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

    public Animator animator;
    // ���庰 ��Ʈ�ѷ� (Inspector���� �ֱ�)
    public RuntimeAnimatorController[] controllers;

    // �̺�Ʈ ���� (�׾��� ��)
    public static event System.Action<int> OnEnemyDied;

    public EnemyData data;

    private int spawnRound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        GameManager.OnRoundChanged += OnRoundChanged;
    }

    private void OnDisable()
    {
        GameManager.OnRoundChanged -= OnRoundChanged;
    }

    private void OnRoundChanged(int round)
    {
        if (round == spawnRound) ApplyRound(round); // �ڱ� ������ ����
    }

    private void ApplyRound(int round)
    {
        int index = (round - 1) % controllers.Length;
        animator.runtimeAnimatorController = controllers[index];
    }

    public void Init(EnemyData enemyData, Vector2 spawnPos, Vector2 dir,int round)
    {
        spawnRound = round;
        ApplyRound(round);

        rb.simulated = true;
        bc.enabled = true;
        
        data = enemyData;
        curHp = data.maxHp;

        transform.position = spawnPos;


        if (spriteRenderer != null && data.enemySprite != null)
            spriteRenderer.sprite = data.enemySprite;

        animator.Play("Run");
        rb.velocity = dir * data.speed;
        
    }
    public void TakeDamage(float amount)
    {
        curHp -= amount;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Hit, 1);
        if (curHp <= 0)
        {
            Die();
            curHp = 0;
        }
    }

    public void Die()
    {
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        OnEnemyDied?.Invoke(coinReward);

        bc.enabled = false;
        rb.simulated = false;
        rb.velocity = Vector2.zero;

        // 0�����Ӻ��� ��� ����
        animator.Play("Die", 0, 0f);

        // ���� ��ȯ �ݿ��� ���� �� ������ ���
        yield return null;

        // "Die"�� ���� ������ ��� (normalizedTime >= 1)
        yield return new WaitUntil(() =>
        {
            var s = animator.GetCurrentAnimatorStateInfo(0);
            return s.IsName("Die") && s.normalizedTime >= 1f;
        });

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            player.TakeDamage(damage);
            rb.velocity = Vector2.zero;

        }

        if(collision.CompareTag("Wall"))
            gameObject.SetActive(false);
    }
}
