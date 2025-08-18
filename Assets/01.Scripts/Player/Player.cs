using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float curHp = 0;
    public float maxHp = 100;

    public List<Enemy> touchingEnemies = new List<Enemy>(); // ����ִ� ���� ����
    private float damageDelay;
    private float damageInterval = 1f; // 1�ʸ��� ������ �ջ�

    public bool isDie;

    private Dictionary<BulletType, BulletState> unlockedBullets = new Dictionary<BulletType, BulletState>();

    // UI���� ����ϴ� �̺�Ʈ (������ ����)
    public static event System.Action<BulletType, int> OnBulletLevelChanged;

    private void Awake()
    {
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

        float dt = Time.deltaTime;

        // �±�(��ųʸ� Ű �̵�)�� ���� �����ӿ� �Ͼ�� �����ϵ��� ���������� ��ȸ
        var statesSnapshot = new List<BulletState>(unlockedBullets.Values);
        foreach (var state in statesSnapshot)
        {
            if (state.CanFire(dt))
            {
                Fire(state);
            }
        }
    }

    void Fire(BulletState state)
    {
        GameObject bullet = ObjectPoolManager.Instance.Get(state.data.poolKey);
        bullet.transform.position = transform.position;

        // ��������Ʈ ��ü
        SpriteRenderer sr = bullet.GetComponent<SpriteRenderer>();
        sr.sprite = state.data.bulletSprite;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.down * state.data.speed;

        PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();
        bulletScript.damage = state.currentDamage;
    }

    public int GetBulletLevel(BulletType type)
    {
        return unlockedBullets.TryGetValue(type, out var st) ? st.level : 0;
    }

    public bool HasBullet(BulletType type) => unlockedBullets.ContainsKey(type);

    public void UnlockBullet(BulletData bullet)
    {
        if (unlockedBullets.TryGetValue(bullet.type, out var state))
        {
            state.Upgrade();
            OnBulletLevelChanged?.Invoke(bullet.type, state.level);
        }
        else
        {
            var st = new BulletState(bullet);
            unlockedBullets.Add(bullet.type, st);
            OnBulletLevelChanged?.Invoke(bullet.type, st.level);
        }
    }

    // UI ��ư���� ȣ��: ���� 3 �̻��� ���� �±�
    public void PromoteBulletManually(BulletType type)
    {
        if (!unlockedBullets.TryGetValue(type, out var state)) return;
        if (state.level < 3) return;

        // ������(�α׿�)
        var oldData = state.data;
        var oldName = oldData.name;
        var oldType = oldData.type;
        var oldGrade = oldData.grade;

        var next = BulletManager.Instance.GetNextGradeBullet(oldData);
        if (next == null)
        {
            Debug.Log($"[Player] Max grade: {oldName}({oldType},{oldGrade})");
            return;
        }

        // ���� Ÿ�� ���� 0���� (UI ���ſ�)
        state.level = 0;
        OnBulletLevelChanged?.Invoke(oldType, 0);

        // �̹� ���� ����� �رݵǾ� �ִٸ� �� ���� ��ġ��(= Upgrade 1ȸ)
        if (unlockedBullets.TryGetValue(next.type, out var target))
        {
            target.Upgrade(); // ����/���� �ݿ�
            unlockedBullets.Remove(oldType);

            OnBulletLevelChanged?.Invoke(target.data.type, target.level);
            Debug.Log($"[Player] PROMOTED+MERGED: {oldName}({oldType},{oldGrade}) -> {target.data.name}({target.data.type},{target.data.grade}), merged Lv.{target.level}");
            return;
        }

        // ���� ����� ���� ���ٸ� �� ���� state�� �±޽��Ѽ� �̵�, Lv.1�� ����
        state.SetData(next); // ���� ��ü + ���� ����
        state.level = 1;
        unlockedBullets.Remove(oldType);
        unlockedBullets[state.data.type] = state;

        OnBulletLevelChanged?.Invoke(state.data.type, state.level);
        Debug.Log($"[Player] PROMOTED: {oldName}({oldType},{oldGrade}) -> {next.name}({next.type},{next.grade}), Lv.{state.level}");
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
