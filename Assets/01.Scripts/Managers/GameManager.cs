using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Transform enemySp;       // ���� ���� ��ġ
    public float spawnInterval = 1f; // ���� ���� �ֱ�(��)
    public bool isStart;

    private float enemySpawnTimer;

    void Update()
    {
        if (!isStart) return;

        enemySpawnTimer += Time.deltaTime;

        if (enemySpawnTimer >= spawnInterval)
        {
            StartCoroutine(MonsterSpawn());
            enemySpawnTimer = 0f;
        }
    }

    IEnumerator MonsterSpawn()
    {
        // Ǯ���� ���� ��������
        GameObject monster = ObjectPoolManager.Instance.Get(PoolKey.Enemy);

        // ���� ������Ʈ ����
        Enemy enemyScript = monster.GetComponent<Enemy>();
        Rigidbody2D rb = monster.GetComponent<Rigidbody2D>();

        // ��ġ �ʱ�ȭ
        monster.transform.position = enemySp.position;

        // �ӵ� �ο� (Enemy ��ũ��Ʈ�� speed ���)
        rb.velocity = enemySp.up * enemyScript.speed;

        yield return null;
    }
}
