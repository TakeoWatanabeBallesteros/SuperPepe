using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StarsDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI starsText;
    private void OnEnable() {
        StarsManager.OnStarsChanged += UpdateStars;
    }
    private void OnDisable() {
        StarsManager.OnStarsChanged -= UpdateStars;
    }

    void UpdateStars(int stars)
    {
        string coinsString = stars.ToString();
        string finalText = "";
        foreach (char letter in coinsString)
        {
            int index = int.Parse(letter.ToString()) == 0 ? 9 : int.Parse(letter.ToString()) - 1;
            finalText = finalText + "<sprite index=" + index.ToString() + ">";
        }
        starsText.text = finalText;
    }
}
