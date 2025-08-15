using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int currentHp = 0;
    public int maxHp = 100;

    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb;
    private Player player;
    public Transform PlayerPosition;
    private float fireDelay;
    private bool fireIsReady;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        player = GetComponent<Player>();
        currentHp = maxHp;
    }

    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        fireDelay += Time.deltaTime;
        if (fireDelay >= 1f)
        {
            
            StartCoroutine(Shot());
            fireDelay = 0;
        }
        
    }

    IEnumerator Shot()
    {
        GameObject bullet = ObjectPoolManager.Instance.Get(PoolKey.PlayerBullet);
        Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
        Transform spawnBulletPos = player.PlayerPosition;

        bullet.transform.position = spawnBulletPos.position;

        
        bulletRigid.velocity = Vector2.down * 10f;

        yield return null;
    }
}
