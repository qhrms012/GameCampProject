using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage;
    private BoxCollider2D bc;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            DisableBullet();
        }

        if (collision.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }
    private void DisableBullet()
    {
        gameObject.SetActive(false);
    }
}
