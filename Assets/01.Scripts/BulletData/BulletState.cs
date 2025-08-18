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
        level = 1; // ó�� �ر��� Lv.1
    }

    // ���(������) ��ü �� ������ �ش� ����� ���̽��� ����
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
        fireTimer = 0f; // �� �±� �� ��� �߻� Ÿ�̹��� �������ϰ� �ʹٸ� ����
    }


    public void Upgrade()
    {
        level++;
        currentDamage += data.damageIncrement;
        currentDelay = Mathf.Max(0.1f, currentDelay - data.delayDecrement); // �ּ� ������ ����
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

