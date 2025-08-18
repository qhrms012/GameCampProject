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
        public Button promoteButton;              // �� root ����
        public TextMeshProUGUI levelText;         // ��� �ؽ�Ʈ�� ����
    }

    public List<BulletButtonPair> bulletButtons;
    Dictionary<BulletType, BulletButtonPair> _uiMap;

    void Awake()
    {
        // UI �� ����
        _uiMap = new Dictionary<BulletType, BulletButtonPair>();
        foreach (var pair in bulletButtons)
            _uiMap[pair.type] = pair;

        // onClick ���
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

    // ��ü ����ȭ (���ü� ���� ����)
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

    // �� �� UI�� ���¸� �ݿ� (SetActive ��� �� ��)
    void ApplyPairState(BulletButtonPair pair, bool unlocked, int level)
    {
        if (pair.levelText) pair.levelText.text = unlocked ? $"Lv.{level}" : "Lv.-";
        if (pair.promoteButton) pair.promoteButton.interactable = (level >= 3);
    }

    void UpdateLevelUI(BulletType type, int level)
    {
        var pair = bulletButtons.Find(p => p.type == type);
        if (pair == null) return;

        // �̺��� �� Lv.- �� ���̰�, ���� �� ���� ǥ��
        var player = GameManager.Instance.player;
        bool unlocked = player != null && player.HasBullet(type);

        if (pair.levelText) pair.levelText.text = unlocked ? $"Lv.{level}" : "Lv.-";
        if (pair.promoteButton) pair.promoteButton.interactable = (level >= 3);
    }

    // ������ UI
    void UpdateCoinUI(int coin) => coinText.text = $"Coin: {coin}";
    void UpdateRoundCountUI(int round) => roundText.text = $"���̺�: {round}";
    void UpdateRoundTimerUI(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        roundTimerText.text = $"{m:00}:{s:00}";
    }
}



