using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public List<BulletData> allBullets; // 모든 총알 등록
    public List<GradeProbability> gradeProbabilities; // 등급별 확률


    [System.Serializable]
    public class GradeProbability
    {
        public BulletGrade grade;
        public float probability; // %
    }
    public BulletData GetRandomBullet()
    {
        // 1단계: 등급 뽑기
        BulletGrade selectedGrade = GetRandomGrade();

        // 2단계: 해당 등급 안에서 랜덤 총알 선택
        List<BulletData> candidates = allBullets.FindAll(b => b.grade == selectedGrade);

        if (candidates.Count == 0)
        {
            Debug.LogError("해당 등급 총알 없음!");
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




