using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Transform enemySp;       // 몬스터 스폰 위치
    public float spawnInterval = 1f; // 몬스터 스폰 주기(초)
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
        // 풀에서 몬스터 가져오기
        GameObject monster = ObjectPoolManager.Instance.Get(PoolKey.Enemy);

        // 몬스터 컴포넌트 참조
        Enemy enemy = monster.GetComponent<Enemy>();
        Rigidbody2D rb = monster.GetComponent<Rigidbody2D>();

        // 위치 초기화
        monster.transform.position = enemySp.position;
        // HP 초기화
        enemy.curHp = enemy.maxHp;

        // 속도 부여 (Enemy 스크립트의 speed 사용)
        rb.velocity = enemySp.up * enemy.speed;

        yield return null;
    }
}
