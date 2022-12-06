using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CoinBlock : MonoBehaviour, IReset
{
    [SerializeField] EventReference thisItemSoundEvent;
    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
        CoinsManager.instance.AddCoin();
        RuntimeManager.PlayOneShot(thisItemSoundEvent, transform.position);
        Destroy(gameObject,2f);
    }
    public void Reset()
    {
        Destroy(this.gameObject);
    }
    
}
