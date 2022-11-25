using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] float timeForPortionDecay;
    [SerializeField] Image healthImageDisplay;
    [SerializeField] Image healthImageBackDisplay;
    int maxHealth;
    private void OnEnable() {
        HealthSystem.OnSetUI += InitializeAll;
        HealthSystem.OnHealthChanged += UpdateHealthBar;
    }
    private void OnDisable() {
        HealthSystem.OnSetUI -= InitializeAll;
        HealthSystem.OnHealthChanged -= UpdateHealthBar;
    }
    void SetHealth(int currentHealth)
    {
        int colorIndex = Mathf.CeilToInt(currentHealth * colors.Length/maxHealth) - 1;
        healthImageDisplay.fillAmount = (float) currentHealth/maxHealth;
        healthImageDisplay.color = colors[colorIndex];
        healthImageBackDisplay.color = colors[colorIndex];
        //sound cambiando el pitch en base a la vida a la que estas
    }
    void InitializeAll(int health,int maxHP)
    {
        maxHealth = maxHP;
        SetHealth(health);
    }
    void UpdateHealthBar(int actualHealth,int previousHealth,bool damage)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateHelath(actualHealth,previousHealth,damage));
    }
    IEnumerator UpdateHelath(int currentHealth, int previousHealth,bool damage)
    {
        int counter = Mathf.Abs(currentHealth - previousHealth);
        while(counter > 0)
        {
            SetHealth(currentHealth + (damage? counter : -counter));
            counter--;
            yield return new WaitForSeconds(timeForPortionDecay);
        }
        SetHealth(currentHealth);
    }
}
