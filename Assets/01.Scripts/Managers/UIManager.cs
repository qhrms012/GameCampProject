using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI coinText;


    void OnEnable()
    {
        GameManager.OnCoinChanged += UpdateCoinUI;
    }

    void OnDisable()
    {
        GameManager.OnCoinChanged -= UpdateCoinUI;
    }

    void UpdateCoinUI(int coin)
    {
        coinText.text = "Coin: " + coin.ToString();
    }
}
