using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinsText;
    private void OnEnable() {
        CoinsManager.OnCoinsChanged += UpdateCoins;
    }
    private void OnDisable() {
        CoinsManager.OnCoinsChanged -= UpdateCoins;
    }

    void UpdateCoins(int coins)
    {
        string coinsString = coins.ToString();
        string finalText = "";
        foreach (char letter in coinsString)
        {
            int index = int.Parse(letter.ToString()) == 0 ? 9 : int.Parse(letter.ToString()) - 1;
            finalText = finalText + "<sprite index=" + index.ToString() + ">";
        }
        coinsText.text = finalText;
    }
}
