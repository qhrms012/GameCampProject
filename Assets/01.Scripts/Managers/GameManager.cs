using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class RoundInfo
{
    public List<EnemyData> enemies; // 이 라운드에서 나올 몬스터들
}

public class GameManager : Singleton<GameManager>
{
    [Header("Spawn Settings")]
    public Transform enemySp;
    public float spawnInterval = 1f;   // 몬스터 스폰 주기
    public float roundDuration = 10f;  // 라운드 유지 시간
    public float roundDelay = 1.5f;    // 라운드 종료 후 대기 시간

    [Header("Game State")]
    public bool isStart;
    public int coin;
    public int roundCount;

    [Header("References")]
    public Player player;
    public BulletManager bulletManager;
    public List<RoundInfo> rounds;

    public static event System.Action<int> OnCoinChanged;
    public static event System.Action<int> OnRoundChanged;
    public static event System.Action<float> OnRoundTimeChanged;



    void Start()
    {
        if (isStart)
            StartCoroutine(RoundRoutine());
    }

    IEnumerator RoundRoutine()
    {
        while (isStart)
        {
            // 라운드 시작
            roundCount++;
            OnRoundChanged?.Invoke(roundCount);
            Debug.Log($"라운드 {roundCount} 시작!");

            float elapsed = 0f;

            // 라운드 시간 동안 계속 스폰
            while (elapsed < roundDuration)
            {
                OnRoundTimeChanged?.Invoke(roundDuration - elapsed);

                yield return StartCoroutine(MonsterSpawn());
                yield return new WaitForSeconds(spawnInterval);
                
                elapsed += spawnInterval;
            }

            Debug.Log($"라운드 {roundCount} 종료! {roundDelay}초 대기...");
            yield return new WaitForSeconds(roundDelay);
        }
    }

    IEnumerator MonsterSpawn()
    {
        GameObject monster = ObjectPoolManager.Instance.Get(PoolKey.Enemy);
        Enemy enemy = monster.GetComponent<Enemy>();

        EnemyData enemyData = GetEnemyDataByRound();
        enemy.Init(enemyData, enemySp.position, enemySp.up);

        yield return null;
    }

    EnemyData GetEnemyDataByRound()
    {
        int idx = Mathf.Min(roundCount - 1, rounds.Count - 1);
        List<EnemyData> availableEnemies = rounds[idx].enemies;

        int rand = Random.Range(0, availableEnemies.Count);
        return availableEnemies[rand];
    }

    public void RollBullet()
    {
        BulletData newBullet = bulletManager.GetRandomBullet();
        player.UnlockBullet(newBullet);
    }

    void OnEnable()
    {
        Enemy.OnEnemyDied += AddCoin;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDied -= AddCoin;
    }

    public void AddCoin(int amount)
    {
        coin += amount;
        OnCoinChanged?.Invoke(coin);
    }

    public bool TrySpendCoin(int cost)
    {
        if (coin < cost) return false;
        coin -= cost;
        OnCoinChanged?.Invoke(coin);
        return true;
    }
}

