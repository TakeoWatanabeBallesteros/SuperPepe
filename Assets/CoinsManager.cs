using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager instance;
    int currentCoins;
    public delegate void CoinsChanged(int coins);
    public static event CoinsChanged OnCoinsChanged;
    private void Awake() {
        instance = this;
    }
    private void Start() {
        OnCoinsChanged.Invoke(currentCoins);
    }
    
    public void AddCoin()
    {
        currentCoins++;
        OnCoinsChanged.Invoke(currentCoins);
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.V))
        {
            AddCoin();
        }
    }
}
