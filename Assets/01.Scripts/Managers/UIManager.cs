using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI roundTimerText;

    [System.Serializable]
    public class BulletButtonPair
    {
        public BulletType type;
        public Button promoteButton;              // ← root 제거
        public TextMeshProUGUI levelText;         // 등급 텍스트도 없음
    }

    public List<BulletButtonPair> bulletButtons;
    Dictionary<BulletType, BulletButtonPair> _uiMap;

    void Awake()
    {
        // UI 맵 구성
        _uiMap = new Dictionary<BulletType, BulletButtonPair>();
        foreach (var pair in bulletButtons)
            _uiMap[pair.type] = pair;

        // onClick 등록
        foreach (var pair in bulletButtons)
        {
            var capturedType = pair.type;
            var btn = pair.promoteButton;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                var player = GameManager.Instance.player;
                int lv = player != null ? player.GetBulletLevel(capturedType) : 0;
                if (lv < 3)
                {
                    Debug.Log($"[UI] Promote ignored: type={capturedType}, Lv={lv} (<3)");
                    return;
                }

                Debug.Log($"[UI] Promote Click! type={capturedType}");
                player.PromoteBulletManually(capturedType);
            });
        }
    }

    void OnEnable()
    {
        Player.OnBulletLevelChanged += UpdateLevelUI;
        GameManager.OnCoinChanged += UpdateCoinUI;
        GameManager.OnRoundChanged += UpdateRoundCountUI;
        GameManager.OnRoundTimeChanged += UpdateRoundTimerUI;

        SyncButtonsFromPlayer();
    }

    void OnDisable()
    {
        Player.OnBulletLevelChanged -= UpdateLevelUI;
        GameManager.OnCoinChanged -= UpdateCoinUI;
        GameManager.OnRoundChanged -= UpdateRoundCountUI;
        GameManager.OnRoundTimeChanged -= UpdateRoundTimerUI;
    }

    void Start()
    {
        var player = GameManager.Instance.player;
        foreach (var pair in bulletButtons)
        {
            int lv = player != null ? player.GetBulletLevel(pair.type) : 0;
            if (pair.levelText) pair.levelText.text = player != null && player.HasBullet(pair.type) ? $"Lv.{lv}" : "Lv.-";
            if (pair.promoteButton) pair.promoteButton.interactable = (lv >= 3);
        }
    }

    // 전체 동기화 (가시성 변경 없음)
    public void SyncButtonsFromPlayer()
    {
        var player = GameManager.Instance.player;
        if (player == null) return;

        foreach (var kv in _uiMap)
        {
            var type = kv.Key;
            var pair = kv.Value;

            bool unlocked = player.HasBullet(type);
            int level = unlocked ? player.GetBulletLevel(type) : 0;

            ApplyPairState(pair, unlocked, level);
        }
    }

    // 한 줄 UI에 상태를 반영 (SetActive 사용 안 함)
    void ApplyPairState(BulletButtonPair pair, bool unlocked, int level)
    {
        if (pair.levelText) pair.levelText.text = unlocked ? $"Lv.{level}" : "Lv.-";
        if (pair.promoteButton) pair.promoteButton.interactable = (level >= 3);
    }

    void UpdateLevelUI(BulletType type, int level)
    {
        var pair = bulletButtons.Find(p => p.type == type);
        if (pair == null) return;

        // 미보유 → Lv.- 로 보이고, 보유 시 레벨 표시
        var player = GameManager.Instance.player;
        bool unlocked = player != null && player.HasBullet(type);

        if (pair.levelText) pair.levelText.text = unlocked ? $"Lv.{level}" : "Lv.-";
        if (pair.promoteButton) pair.promoteButton.interactable = (level >= 3);
    }

    // 나머지 UI
    void UpdateCoinUI(int coin) => coinText.text = $"Coin: {coin}";
    void UpdateRoundCountUI(int round) => roundText.text = $"웨이브: {round}";
    void UpdateRoundTimerUI(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        roundTimerText.text = $"{m:00}:{s:00}";
    }
}



