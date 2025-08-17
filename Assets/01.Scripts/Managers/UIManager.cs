using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI roundText;


    void OnEnable()
    {
        GameManager.OnCoinChanged += UpdateCoinUI;
        GameManager.OnRoundChanged += UpdateRoundCountUI;
    }

    void OnDisable()
    {
        GameManager.OnCoinChanged -= UpdateCoinUI;
        GameManager.OnRoundChanged -= UpdateRoundCountUI;
    }

    void UpdateCoinUI(int coin)
    {
        coinText.text = "Coin: " + coin.ToString();
    }

    void UpdateRoundCountUI(int round)
    {
        roundText.text = "Round: " + round.ToString();
    }
}
