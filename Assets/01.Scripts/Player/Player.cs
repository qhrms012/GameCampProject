using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float curHp = 0;
    public float maxHp = 100;

    public List<Enemy> touchingEnemies = new List<Enemy>(); // 닿아있는 적들 저장
    private float damageDelay;
    private float damageInterval = 1f; // 1초마다 데미지 합산

    public bool isDie;

    private Dictionary<BulletType, BulletState> unlockedBullets = new Dictionary<BulletType, BulletState>();

    // UI에서 사용하는 이벤트 (레벨만 유지)
    public static event System.Action<BulletType, int> OnBulletLevelChanged;

    private void Awake()
    {
        curHp = maxHp;
    }

    private void Update()
    {
        // 일정 시간마다 데미지 합산 적용
        damageDelay += Time.deltaTime;
        if (damageDelay >= damageInterval)
        {
            ApplyTotalDamage();
            damageDelay = 0;
        }

        float dt = Time.deltaTime;

        // 승급(딕셔너리 키 이동)이 같은 프레임에 일어나도 안전하도록 스냅샷으로 순회
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

        // 스프라이트 교체
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

    // UI 버튼에서 호출: 레벨 3 이상일 때만 승급
    public void PromoteBulletManually(BulletType type)
    {
        if (!unlockedBullets.TryGetValue(type, out var state)) return;
        if (state.level < 3) return;

        // 스냅샷(로그용)
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

        // 현재 타입 레벨 0으로 (UI 갱신용)
        state.level = 0;
        OnBulletLevelChanged?.Invoke(oldType, 0);

        // 이미 다음 등급이 해금되어 있다면 → 레벨 합치기(= Upgrade 1회)
        if (unlockedBullets.TryGetValue(next.type, out var target))
        {
            target.Upgrade(); // 레벨/스탯 반영
            unlockedBullets.Remove(oldType);

            OnBulletLevelChanged?.Invoke(target.data.type, target.level);
            Debug.Log($"[Player] PROMOTED+MERGED: {oldName}({oldType},{oldGrade}) -> {target.data.name}({target.data.type},{target.data.grade}), merged Lv.{target.level}");
            return;
        }

        // 다음 등급이 아직 없다면 → 기존 state를 승급시켜서 이동, Lv.1로 시작
        state.SetData(next); // 참조 교체 + 스탯 리셋
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
            if (enemy != null) // 혹시 죽어서 풀링된 경우 방지
                totalDamage += enemy.damage; // Enemy 스크립트 안의 공격력 값
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
        Debug.Log($"{gameObject.name} 피격! 남은 HP: {curHp}");

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
