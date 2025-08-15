using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float curHp;
    public float maxHp = 100;

    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private PlayerBullet pb;

    public Transform spawnPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        pb = GetComponent<PlayerBullet>();
        curHp = maxHp;
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
        // �׾��� �� ó�� (������Ʈ ��Ȱ��ȭ)
        gameObject.SetActive(false);
    }
}
