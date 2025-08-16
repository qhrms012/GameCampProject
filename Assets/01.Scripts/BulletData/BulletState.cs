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
        this.data = data;
        currentDamage = data.baseDamage;
        currentDelay = data.baseDelay;
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

