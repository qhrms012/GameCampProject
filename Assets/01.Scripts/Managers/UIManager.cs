using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI roundTimerText;
    public Button coinBtn;
    public TextMeshProUGUI coinBtnText;

    [SerializeField] 
    private int basePrice = 20;

    private int price;

    [SerializeField] 
    private int increment = 2;

    [SerializeField]
    private Image playerHP;



    [System.Serializable]
    public class BulletButtonPair
    {
        public BulletType type;
        public Button promoteButton;              
        public TextMeshProUGUI levelText;         
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
        Player.onHpChanged += UpdateHPUI;
        GameManager.OnCoinChanged += UpdateCoinUI;
        GameManager.OnCoinChanged += UpdateCoinBtn;
        GameManager.OnRoundChanged += UpdateRoundCountUI;
        GameManager.OnRoundTimeChanged += UpdateRoundTimerUI;

        SyncButtonsFromPlayer();
    }

    void OnDisable()
    {
        Player.OnBulletLevelChanged -= UpdateLevelUI;
        Player.onHpChanged -= UpdateHPUI;
        GameManager.OnCoinChanged -= UpdateCoinUI;
        GameManager.OnCoinChanged -= UpdateCoinBtn;
        GameManager.OnRoundChanged -= UpdateRoundCountUI;
        GameManager.OnRoundTimeChanged -= UpdateRoundTimerUI;
    }


    void Start()
    {
        var player = GameManager.Instance.player;
        foreach (var pair in bulletButtons)
        {
            int lv = player != null ? player.GetBulletLevel(pair.type) : 0;
            if (pair.levelText) 
                pair.levelText.text = player != null && player.HasBullet(pair.type) ? $"Lv.{lv}" : "Lv.-";
            if (pair.promoteButton) 
                pair.promoteButton.interactable = (lv >= 3);
        }
    }

    // ��ü ����ȭ
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
    void UpdateCoinBtn(int coin)
    {
        coinBtn.interactable = coin >= price;
    }

    // ��ư Ŭ��
    public void OnRollButtonClicked()
    {
        if (GameManager.Instance.TrySpendCoin(price))
        {
            GameManager.Instance.RollBullet();
        }
    }
    void UpdateRoundCountUI(int round)
    {
        roundText.text = $"���̺�: {round}";
        price = basePrice + (round - 1) * increment;
        coinBtnText.text = $"�������� ��ȯ: {price}";
    }
    void UpdateRoundTimerUI(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        roundTimerText.text = $"{m:00}:{s:00}";
    }
    void UpdateHPUI(float Hp)
    {
        playerHP.fillAmount = Hp / GameManager.Instance.player.maxHp;
    }
}



