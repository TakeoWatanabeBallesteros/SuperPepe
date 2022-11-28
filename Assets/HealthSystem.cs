using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour, ITakeDamage, IReset
{
    [SerializeField]int maxHealth;
    [SerializeField] int startingLifes;
    int currentHealth;
    int currentLifes;
    bool isAlive;
    public delegate void SetHealthUI(int health,int maxHealth, int lifes);
    public delegate void HealthChanged(int actualHealth,int previousHealth,bool damage);
    public static event SetHealthUI OnSetUI;
    public static event HealthChanged OnHealthChanged;

    
    void Start()
    {
        isAlive = true;
        currentHealth = maxHealth;
        currentLifes = startingLifes;
        OnSetUI?.Invoke(currentHealth,maxHealth,currentLifes);
        GameManager.GetGameManager().AddResetObject(this);
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.B))
        {
            TakeDamage(1);
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            TakeDamage(4);
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            Heal(3);
        }
    }
    public void TakeDamage(int damage)
    {
        if(!isAlive) return;
        
        int previousHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth-damage,0,maxHealth);
        //Update UI
        OnHealthChanged?.Invoke(currentHealth,previousHealth,true);
        //Comprobación de posible final de partida
        isAlive = currentHealth > 0;
        if(!isAlive)GameManager.GetGameManager().GameOver(currentLifes);
    }
    public void Heal(int amount)
    {
        if(!isAlive) return;
        //Update UI
        OnHealthChanged?.Invoke(Mathf.Clamp(currentHealth + amount,0,maxHealth),currentHealth,false);
        //Logic
        currentHealth = Mathf.Clamp(currentHealth + amount,0,maxHealth);
    }
    public bool CanHeal()
    {
        return currentHealth < maxHealth;
    }
    public void Reset()
    {
        currentHealth = maxHealth;
        currentLifes--;
        isAlive = true;
        OnSetUI?.Invoke(currentHealth,maxHealth,currentLifes);
    }
}
