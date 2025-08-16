using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public List<BulletData> allBullets; // ��� �Ѿ� ���
    public List<GradeProbability> gradeProbabilities; // ��޺� Ȯ��


    [System.Serializable]
    public class GradeProbability
    {
        public BulletGrade grade;
        public float probability; // %
    }
    public BulletData GetRandomBullet()
    {
        // 1�ܰ�: ��� �̱�
        BulletGrade selectedGrade = GetRandomGrade();

        // 2�ܰ�: �ش� ��� �ȿ��� ���� �Ѿ� ����
        List<BulletData> candidates = allBullets.FindAll(b => b.grade == selectedGrade);

        if (candidates.Count == 0)
        {
            Debug.LogError("�ش� ��� �Ѿ� ����!");
            return null;
        }

        int randomIndex = Random.Range(0, candidates.Count);
        return candidates[randomIndex];
    }

    private BulletGrade GetRandomGrade()
    {
        float total = 0f;
        foreach (var gp in gradeProbabilities)
            total += gp.probability;

        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var gp in gradeProbabilities)
        {
            cumulative += gp.probability;
            if (rand <= cumulative)
                return gp.grade;
        }

        return gradeProbabilities[0].grade; // fallback
    }
}




