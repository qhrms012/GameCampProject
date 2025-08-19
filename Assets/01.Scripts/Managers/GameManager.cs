using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



[System.Serializable]
public class RoundInfo
{
    public List<EnemyData> enemies; // �� ���忡�� ���� ���͵�
}

public class GameManager : Singleton<GameManager>
{
    [Header("Spawn Settings")]
    public Transform enemySp;
    public float spawnInterval = 1f;   // ���� ���� �ֱ�
    public float roundDuration = 10f;  // ���� ���� �ð�
    public float roundDelay = 1.5f;    // ���� ���� �� ��� �ð�

    [Header("Game State")]
    public bool isStart;
    public int coin;
    public int roundCount;

    [Header("References")]
    public Player player;
    public BulletManager bulletManager;
    public List<RoundInfo> rounds;

    [Header("PlayerDead")]
    public GameObject deadPanel;

    public static event System.Action<int> OnCoinChanged;
    public static event System.Action<int> OnRoundChanged;
    public static event System.Action<float> OnRoundTimeChanged;



    void Start()
    {
        AudioManager.Instance.PlayBgm(true);

        if (isStart)
            StartCoroutine(RoundRoutine());
    }

    IEnumerator RoundRoutine()
    {
        while (isStart)
        {
            // ���� ����
            AddCoin(20);
            
            roundCount++;
            OnRoundChanged?.Invoke(roundCount);
            Debug.Log($"���� {roundCount} ����!");

            float elapsed = 0f;

            // ���� �ð� ���� ��� ����
            while (elapsed < roundDuration)
            {
                OnRoundTimeChanged?.Invoke(roundDuration - elapsed);

                yield return StartCoroutine(MonsterSpawn());
                yield return new WaitForSeconds(spawnInterval);
                
                elapsed += spawnInterval;
            }

            Debug.Log($"���� {roundCount} ����! {roundDelay}�� ���...");
            yield return new WaitForSeconds(roundDelay);
        }
    }

    IEnumerator MonsterSpawn()
    {
        GameObject monster = ObjectPoolManager.Instance.Get(PoolKey.Enemy);
        Enemy enemy = monster.GetComponent<Enemy>();

        EnemyData enemyData = GetEnemyDataByRound();
        enemy.Init(enemyData, enemySp.position, enemySp.up,roundCount);

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
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Select);
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

    public void Pause(bool Pause)
    {
        if (Pause)
        {
            Time.timeScale = 0;
            AudioManager.Instance.EffectBgm(Pause);
        }
        else
        {
            Time.timeScale = 1;
            AudioManager.Instance.EffectBgm(Pause);
        }
        
    }

    public void Restart()
    {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1;
    }
    public void GameOver()
    {
        AudioManager.Instance.PlayBgm(false);
        deadPanel.SetActive(true);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Lose);
    }
}

