using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletState
{
    public BulletData data;
    public int level = 1;
    public float currentDamage;
    public float currentDelay;
    private float fireTimer = 0f;

    public BulletState(BulletData data)
    {
        SetData(data);
        level = 1; // 처음 해금은 Lv.1
    }

    // 등급(데이터) 교체 시 스탯을 해당 등급의 베이스로 리셋
    public void SetData(BulletData newData)
    {
        if (newData == null)
        {
            Debug.LogError("[BulletState] SetData called with null");
            return;
        }
        data = newData;
        currentDamage = data.baseDamage;
        currentDelay = Mathf.Max(0.01f, data.baseDelay);
        fireTimer = 0f; // ← 승급 후 즉시 발사 타이밍을 재정렬하고 싶다면 유지
    }


    public void Upgrade()
    {
        level++;
        currentDamage += data.damageIncrement;
        currentDelay = Mathf.Max(0.1f, currentDelay - data.delayDecrement); // 최소 딜레이 제한
    }

    public bool CanFire(float deltaTime)
    {
        fireTimer += deltaTime;
        if (fireTimer >= currentDelay)
        {
            fireTimer = 0f;
            return true;
        }
        return false;
    }
}

