using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBlock : MonoBehaviour, IReset
{
    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
        CoinsManager.instance.AddCoin();
        //play sound
        Destroy(gameObject,2f);
    }
    public void Reset()
    {
        Destroy(this.gameObject);
    }
    
}
