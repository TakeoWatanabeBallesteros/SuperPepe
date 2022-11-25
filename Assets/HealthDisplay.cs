using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] float timeForPortionDecay;
    [SerializeField] float timeToHide;
    [SerializeField] Image healthImageDisplay;
    [SerializeField] Image healthImageBackDisplay;
    Animator anim;
    int maxHealth;
    private void OnEnable() {
        HealthSystem.OnSetUI += InitializeAll;
        HealthSystem.OnHealthChanged += UpdateHealthBar;
        anim = GetComponent<Animator>();
    }
    private void OnDisable() {
        HealthSystem.OnSetUI -= InitializeAll;
        HealthSystem.OnHealthChanged -= UpdateHealthBar;
    }
    void SetHealth(int currentHealth)
    {
        int colorIndex = Mathf.Clamp(Mathf.CeilToInt(currentHealth * colors.Length/maxHealth) - 1,0,colors.Length - 1);
        healthImageDisplay.fillAmount = (float) currentHealth/maxHealth;
        healthImageDisplay.color = colors[colorIndex];
        healthImageBackDisplay.color = colors[colorIndex];
        //sound cambiando el pitch en base a la vida a la que estas
    }
    void InitializeAll(int health,int maxHP)
    {
        maxHealth = maxHP;
        SetHealth(health);
        StartCoroutine(StartHealth());
    }
    void UpdateHealthBar(int actualHealth,int previousHealth,bool damage)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateHelath(actualHealth,previousHealth,damage));
    }
    IEnumerator UpdateHelath(int currentHealth, int previousHealth,bool damage)
    {
        anim.SetBool("Show",true);
        int counter = Mathf.Abs(currentHealth - previousHealth);
        yield return new WaitForSeconds(0.5f);
        while(counter > 0)
        {
            counter--;
            SetHealth(currentHealth + (damage? counter : -counter));
            yield return new WaitForSeconds(timeForPortionDecay);
        }
        yield return new WaitForSeconds(timeToHide);
        anim.SetBool("Show",false);
    }
    IEnumerator StartHealth()
    {
        anim.SetBool("Show",true);
        yield return new WaitForSeconds(2f);
        anim.SetBool("Show",false);
    }
}
