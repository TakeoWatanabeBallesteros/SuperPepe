using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifesDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lifesText;
    private void OnEnable() {
        HealthSystem.OnLifesChanged += UpdateLifes;
    }
    private void OnDisable() {
        HealthSystem.OnLifesChanged -= UpdateLifes;
    }

    void UpdateLifes(int lifes)
    {
        string lifesString = lifes.ToString();
        string finalText = "";
        foreach (char letter in lifesString)
        {
            int index = int.Parse(letter.ToString()) == 0 ? 9 : int.Parse(letter.ToString()) - 1;
            finalText = finalText + "<sprite index=" + index.ToString() + ">";
        }
        lifesText.text = finalText;
    }
}
